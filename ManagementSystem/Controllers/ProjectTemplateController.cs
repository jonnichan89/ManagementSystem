using ManagementSystem.Common.Models;
using ManagementSystem.Common.Utilities;
using ManagementSystem.Data.Models;
using ManagementSystem.DataServices.Resources;
using ManagementSystem.DataServices.Service;
using Microsoft.AspNetCore.Mvc;
using System.IO.Compression;
using System.Text.Json;
using static System.Net.Mime.MediaTypeNames;

namespace ManagementSystem.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProjectTemplateController : ControllerBase
    {
        //private readonly IStringLocalizer<ProjectTemplateController> _res;
        private readonly SharedResources _resShared;
        private readonly ILogger<ProjectTemplateController> _logger;
        private readonly ProjectTemplateService _projectTemplateService;
        private readonly IWebHostEnvironment _env;

        private string _uploadRootFolder;
        private string _uploadImageFolder;
        public ProjectTemplateController(
            //IStringLocalizer<ProjectTemplateController> res,
            SharedResources resShared,
            ILogger<ProjectTemplateController> logger,
            ProjectTemplateService projectTemplateService,
            IWebHostEnvironment env)
        {
            //_res = res;
            _resShared = resShared;
            _logger = logger;
            _projectTemplateService = projectTemplateService;
            _env = env;

            var webRoot = _env?.WebRootPath;
            if (string.IsNullOrWhiteSpace(webRoot))
                webRoot = Path.Combine(Directory.GetCurrentDirectory());

            _uploadRootFolder = Path.Combine(webRoot, "uploads");
            _uploadImageFolder = Path.Combine(_uploadRootFolder, "ProjectTemplates");
            Directory.CreateDirectory(_uploadImageFolder);

            _projectTemplateService.UploadRootFolder = _uploadRootFolder;
        }

        [Route("LastModified")]
        [HttpGet]
        public async Task<ActionResult<ServiceActionResult>> GetLastModified()
        {
            ServiceActionResult result = new ServiceActionResult();
            try
            {
                DateTime? lastModified = await _projectTemplateService.GetLastModifiedAsync();
                if (lastModified.HasValue)
                {
                    result.ActionResult = lastModified.Value.ToString("yyyy/MM/dd HH:mm");
                }
                else
                {
                    result.ActionResult = string.Empty;
                }

                result.ActionIsSuccess = true;
            }
            catch (Exception ex)
            {
                result.ActionIsSuccess = false;
                result.ErrorMessage = ExceptionHelper.GetErrorMessage(ex, true);
            }
            return Ok(result);
        }

        [Route("Count")]
        [HttpGet]
        public async Task<ActionResult<ServiceActionResult>> GetCount()
        {
            ServiceActionResult result = new ServiceActionResult();
            try
            {
                result.ActionResult = await _projectTemplateService.CountAsync();

                result.ActionIsSuccess = true;
            }
            catch (Exception ex)
            {
                result.ActionIsSuccess = false;
                result.ErrorMessage = ExceptionHelper.GetErrorMessage(ex, true);
            }
            return Ok(result);
        }

        [Route("All")]
        [HttpGet]
        public async Task<ActionResult<ServiceActionResult>> GetAll()
        {
            ServiceActionResult result = new ServiceActionResult();
            try
            {
                result.ActionResult = await _projectTemplateService.GetAllProjectTemplatesAsync();
                result.ActionIsSuccess = true;
            }
            catch (Exception ex)
            {
                result.ActionIsSuccess = false;
                result.ErrorMessage = ExceptionHelper.GetErrorMessage(ex, true);
            }
            return Ok(result);
        }

        [Route("AllImages")]
        [HttpGet]
        public async Task<IActionResult> GetAllImages()
        {
            try
            {
                if (!Directory.Exists(_uploadImageFolder))
                {
                    return NotFound("No images found.");
                }

                var files = Directory.GetFiles(_uploadImageFolder);
                if (files == null || files.Length == 0)
                {
                    return NotFound("No images found.");
                }

                using var mem = new MemoryStream();
                using (var archive = new ZipArchive(mem, ZipArchiveMode.Create, true))
                {
                    foreach (var filePath in files)
                    {
                        try
                        {
                            var entryName = Path.GetFileName(filePath);
                            var entry = archive.CreateEntry(entryName, CompressionLevel.Optimal);
                            using var entryStream = entry.Open();
                            using var fileStream = System.IO.File.OpenRead(filePath);
                            await fileStream.CopyToAsync(entryStream);
                        }
                        catch
                        {
                            // skip files that cause issues
                        }
                    }
                }

                mem.Seek(0, SeekOrigin.Begin);
                var zipName = $"ProjectTemplates.zip";
                var bytes = mem.ToArray();
                return File(bytes, "application/zip", zipName);
            }
            catch (Exception ex)
            {
                ServiceActionResult result = new ServiceActionResult();
                result.ActionIsSuccess = false;
                result.ErrorMessage = ExceptionHelper.GetErrorMessage(ex, true);
                return Ok(result);
            }
        }

        [Route("{PTId}")]
        [HttpGet]
        public ActionResult<ServiceActionResult> Get([FromRoute] int PTId)
        {
            ServiceActionResult result = new ServiceActionResult();
            try
            {
                result.ActionResult = _projectTemplateService.GetProjectTemplateById(PTId);
                result.ActionIsSuccess = (result.ActionResult != null);
            }
            catch (Exception ex)
            {
                result.ActionIsSuccess = false;
                result.ErrorMessage = ExceptionHelper.GetErrorMessage(ex, true);
            }
            return Ok(result);
        }

        [Route("")]
        [HttpPost]
        public ActionResult<ServiceActionResult> Add([FromForm] string formValue, IFormFile[]? images)
        {
            try
            {
                ProjectTemplate model = JsonSerializer.Deserialize<ProjectTemplate>(formValue);
                
                model.SetDefault();

                foreach (ProjectTemplate childModel in model.ProjectTemplates)
                {
                    childModel.SetDefault();

                    if (model.PTId > 0)
                    {
                        model.ParentPTId = childModel.PTId;
                        model.ParentProjectTemplate = childModel;
                    }

                    foreach (var item in childModel.ProjectTemplateDetails)
                    {
                        item.ProjectTemplate = childModel;
                    }
                }

                ServiceActionResult result = new ServiceActionResult();
                result.ModelValidations = _projectTemplateService.CanContinueAction(ServiceAction.Add, model);

                if (result.ModelValidations.Count == 0)
                {
                    ServiceActionResult saveFileResult = SaveFile(ref model, images);

                    if (!saveFileResult.ActionIsSuccess)
                    {
                        result.ErrorMessage = saveFileResult.ErrorMessage;
                        return Ok(result);
                    }

                    try
                    {
                        result.ActionResult = _projectTemplateService.CreateProjectTemplate(model);
                        result.ActionIsSuccess = ((int)result.ActionResult) > 0;
                    }
                    catch (Exception ex)
                    {
                        result.ErrorMessage = ExceptionHelper.GetErrorMessage(ex, true);
                    }
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to add project template.");
                return StatusCode(500, "An error occurred while add project template." + Environment.NewLine +
                    ExceptionHelper.GetErrorMessage(ex, true));
            }
        }

        [Route("{PTId}")]
        [HttpPut]
        public ActionResult<ServiceActionResult> Update([FromRoute] int PTId, [FromForm] string formValue, IFormFile[]? images)
        {
            try
            {
                ProjectTemplate model = JsonSerializer.Deserialize<ProjectTemplate>(formValue);

                model.SetDefault();

                foreach (ProjectTemplate childModel in model.ProjectTemplates)
                {
                    childModel.SetDefault();

                    if (model.PTId > 0)
                    {
                        model.ParentPTId = childModel.PTId;
                        model.ParentProjectTemplate = childModel;
                    }

                    foreach (var item in childModel.ProjectTemplateDetails)
                    {
                        item.ProjectTemplate = childModel;
                    }
                }

                ServiceActionResult result = new ServiceActionResult();
                result.ModelValidations = _projectTemplateService.CanContinueAction(ServiceAction.Update, model);

                if (result.ModelValidations.Count == 0)
                {
                    ServiceActionResult saveFileResult = SaveFile(ref model, images);

                    if (!saveFileResult.ActionIsSuccess)
                    {
                        result.ErrorMessage = saveFileResult.ErrorMessage;
                        return Ok(result);
                    }

                    try
                    {
                        result.ActionResult = _projectTemplateService.UpdateProjectTemplate(model);
                        result.ActionIsSuccess = ((int)result.ActionResult) > 0;
                    }
                    catch (Exception ex)
                    {
                        result.ErrorMessage = ExceptionHelper.GetErrorMessage(ex, true);
                    }
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update project template.");
                return StatusCode(500, "An error occurred while update project template." + Environment.NewLine +
                    ExceptionHelper.GetErrorMessage(ex, true));
            }
        }

        [Route("{PTId}")]
        [HttpDelete]
        public ActionResult<ServiceActionResult> Delete([FromRoute] int PTId)
        {
            try
            {
                ServiceActionResult result = new ServiceActionResult();
                result.ModelValidations = _projectTemplateService.CanContinueAction(
                    ServiceAction.Delete,
                    new ProjectTemplate
                    {
                        PTId = PTId
                    });

                if (result.ModelValidations.Count == 0)
                {
                    try
                    {
                        result.ActionResult = _projectTemplateService.DeleteProjectTemplate(PTId);
                        result.ActionIsSuccess = (result.ActionResult != null);
                    }
                    catch (Exception ex)
                    {
                        result.ErrorMessage = ExceptionHelper.GetErrorMessage(ex, true);
                    }
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete project template.");
                return StatusCode(500, "An error occurred while delete project template." + Environment.NewLine +
                    ExceptionHelper.GetErrorMessage(ex, true));
            }
        }

        private ServiceActionResult SaveFile(ref ProjectTemplate model, IFormFile[]? images)
        {
            ServiceActionResult result = new ServiceActionResult();
            result.ActionIsSuccess = true;

            // Image validation: allowed types and max size
            // Allowed extensions and content types
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
            var allowedContentTypes = new[] { "image/jpeg", "image/png" };
            // Max size 2 MB
            const long maxFileSize = 2 * 1024 * 1024;

            if (images != null && images.Length > 0)
            {
                foreach (IFormFile image in images)
                {
                    // Check size
                    if (image.Length > maxFileSize)
                    {
                        result.ErrorMessage = $"Image size must be <= {maxFileSize / 1024 / 1024} MB.";
                        result.ActionIsSuccess = false;
                        return result;
                    }

                    // Check content type
                    if (!string.IsNullOrWhiteSpace(image.ContentType) &&
                        !allowedContentTypes.Contains(image.ContentType.ToLowerInvariant()))
                    {
                        result.ErrorMessage = "Unsupported image content type. Allowed: jpg, jpeg, png.";
                        result.ActionIsSuccess = false;
                        return result;
                    }

                    // Check extension
                    var ext = Path.GetExtension(image.FileName);
                    if (string.IsNullOrWhiteSpace(ext) || !allowedExtensions.Contains(ext.ToLowerInvariant()))
                    {
                        result.ErrorMessage = "Unsupported image file extension. Allowed: .jpg, .jpeg, .png.";
                        result.ActionIsSuccess = false;
                        return result;
                    }

                    try
                    {
                        // Determine target folder: wwwroot/uploads/items
                        var fileName = System.IO.Path.GetFileName(image.FileName); // $"{model.ItemCode}{ext}";
                        var filePath = Path.Combine(_uploadImageFolder, fileName);
                        var ptName = System.IO.Path.GetFileNameWithoutExtension(image.FileName); // $"{model.ItemCode}{ext}";

                        ProjectTemplate subPT = model.ProjectTemplates.FirstOrDefault(pt => pt.PTName == ptName);
                        if (subPT != null)
                        {
                            using (var stream = System.IO.File.Create(filePath))
                            {
                                image.CopyTo(stream);
                            }

                            subPT.HasImageUpload = true;
                            subPT.ImagePath = $"/ProjectTemplates/{fileName}";
                            subPT.ImageFileSize = (int)image.Length;

                            if (subPT.PTNo == 1)
                            {
                                model.HasImageUpload = subPT.HasImageUpload;
                                model.ImagePath = subPT.ImagePath;
                                model.ImageFileSize = subPT.ImageFileSize;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        result.ErrorMessage = "Failed to save uploaded image. " + ExceptionHelper.GetErrorMessage(ex, true);
                        result.ActionIsSuccess = false;
                        return result;
                    }

                }
            }

            return result;
        }

    }

}
