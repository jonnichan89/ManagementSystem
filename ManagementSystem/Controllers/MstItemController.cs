using ManagementSystem.Common.Models;
using ManagementSystem.Common.OtherModels;
using ManagementSystem.Common.Utilities;
using ManagementSystem.Data.Models;
using ManagementSystem.DataServices.Resources;
using ManagementSystem.DataServices.Service;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.IO.Compression;
using System.Text.Json;

namespace ManagementSystem.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MstItemController : ControllerBase
    {
        //private readonly IStringLocalizer<MstItemController> _res;
        private readonly SharedResources _resShared;
        private readonly ILogger<MstItemController> _logger;
        private readonly MstItemService _itemService;
        private readonly IWebHostEnvironment _env;

        private string _uploadRootFolder;
        private string _uploadImageFolder;
        public MstItemController(
            //IStringLocalizer<MstItemController> res,
            SharedResources resShared,
            ILogger<MstItemController> logger,
            MstItemService itemService,
            IWebHostEnvironment env)
        {
            //_res = res;
            _resShared = resShared;
            _logger = logger;
            _itemService = itemService;
            _env = env;

            var webRoot = _env?.WebRootPath;
            if (string.IsNullOrWhiteSpace(webRoot))
                webRoot = Path.Combine(Directory.GetCurrentDirectory());

            _uploadRootFolder = Path.Combine(webRoot, "uploads");
            _uploadImageFolder = Path.Combine(_uploadRootFolder, "MstItems");
            Directory.CreateDirectory(_uploadImageFolder);

            _itemService.UploadRootFolder = _uploadRootFolder;
        }

        [Route("LastModified")]
        [HttpGet]
        public async Task<ActionResult<ServiceActionResult>> GetLastModified()
        {
            ServiceActionResult result = new ServiceActionResult();
            try
            {
                DateTime? lastModified = await _itemService.GetLastModifiedAsync();
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
                result.ActionResult = await _itemService.CountAsync();

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
                result.ActionResult = await _itemService.GetAllMstItems();
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
                //if (!Directory.Exists(_uploadImageFolder))
                //{
                //    return NotFound("No images found.");
                //}

                //var files = Directory.GetFiles(_uploadImageFolder);
                //if (files == null || files.Length == 0)
                //{
                //    return NotFound("No images found.");
                //}

                //using var mem = new MemoryStream();
                //using (var archive = new ZipArchive(mem, ZipArchiveMode.Create, true))
                //{
                //    foreach (var filePath in files)
                //    {
                //        try
                //        {
                //            var entryName = Path.GetFileName(filePath);
                //            var entry = archive.CreateEntry(entryName, CompressionLevel.Optimal);
                //            using var entryStream = entry.Open();
                //            using var fileStream = System.IO.File.OpenRead(filePath);
                //            await fileStream.CopyToAsync(entryStream);
                //        }
                //        catch
                //        {
                //            // skip files that cause issues
                //        }
                //    }
                //}

                //mem.Seek(0, SeekOrigin.Begin);
                //var zipName = $"MstItems.zip";
                //var bytes = mem.ToArray();
                //return File(bytes, "application/zip", zipName);

                string zipFileName = await ZipArchiveAsync(true);
                if (string.IsNullOrWhiteSpace(zipFileName))
                {
                    return NotFound("No images found.");
                }
                return PhysicalFile(zipFileName, "application/zip", "MstItems.zip");

            }
            catch (Exception ex)
            {
                ServiceActionResult result = new ServiceActionResult();
                result.ActionIsSuccess = false;
                result.ErrorMessage = ExceptionHelper.GetErrorMessage(ex, true);
                return Ok(result);
            }
        }

        private async Task<string> ZipArchiveAsync(bool getOnly = false)
        {
            DateTime? lastModified = await _itemService.GetLastModifiedAsync();
            if (!lastModified.HasValue)
            {
                foreach (string zipFile in System.IO.Directory.GetFiles(_uploadRootFolder, "MstItems*.zip"))
                {
                    System.IO.File.Delete(zipFile);
                }

                return string.Empty;
            }

            var zipFilePath = System.IO.Path.Combine(_uploadRootFolder, $"MstItems{lastModified?.ToString("yyyyMMddHHmmss")}.zip");
            if (System.IO.File.Exists(zipFilePath))
            {
                if (getOnly)
                {
                    return zipFilePath;
                }

                System.IO.File.Delete(zipFilePath);
            }

            var files = Directory.GetFiles(_uploadImageFolder);
            if (files == null || files.Length == 0)
            {
                return string.Empty;
            }

            byte[] zipBytes;
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

            zipBytes = mem.ToArray();
            await System.IO.File.WriteAllBytesAsync(zipFilePath, zipBytes);

            return zipFilePath;
        }

        [Route("page/{pageId}")]
        [HttpGet]
        public async Task<ActionResult<ServiceActionResult>> GetPage([FromRoute] int pageId, [FromForm] PageFilterMstItem? pageFilter)
        {
            ServiceActionResult result = new ServiceActionResult();

            try
            {
                result.ActionResult = await _itemService.GetMstItems(pageId, pageFilter);
                result.ActionIsSuccess = true;
            }
            catch (Exception ex)
            {
                result.ActionIsSuccess = false;
                result.ErrorMessage = ExceptionHelper.GetErrorMessage(ex, true);
            }

            return Ok(result);
        }

        [Route("{itemId}")]
        [HttpGet]
        public ActionResult<ServiceActionResult> Get([FromRoute] int itemId)
        {
            ServiceActionResult result = new ServiceActionResult();
            try
            {
                result.ActionResult = _itemService.GetMstItemById(itemId);
                result.ActionIsSuccess = (result.ActionResult != null);
            }
            catch (Exception ex)
            {
                result.ActionIsSuccess = false;
                result.ErrorMessage = ExceptionHelper.GetErrorMessage(ex, true);
            }
            return Ok(result);
        }

        private ServiceActionResult SaveFile(ref MstItem model, IFormFile[]? images)
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
                IFormFile image = images[0];
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
                    var fileName = System.IO.Path.GetFileName(image.FileName);
                    var filePath = Path.Combine(_uploadImageFolder, fileName);

                    using (var stream = System.IO.File.Create(filePath))
                    {
                        image.CopyTo(stream);
                    }

                    model.HasImageUpload = true;
                    model.ImagePath = $"/MstItems/{fileName}";
                    model.ImageFileSize = (int)image.Length;
                }
                catch (Exception ex)
                {
                    result.ErrorMessage = "Failed to save uploaded image. " + ExceptionHelper.GetErrorMessage(ex, true);
                    result.ActionIsSuccess = false;
                    return result;
                }
            }

            return result;
        }

        private void DeleteFile(MstItem model)
        {
            try
            {
                // Determine target folder: wwwroot/uploads/MstItems
                var webRoot = _env?.WebRootPath;
                if (string.IsNullOrWhiteSpace(webRoot))
                    webRoot = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");

                var filePath = Path.Combine(webRoot, "uploads", "MstItems", model.ItemCode);
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to delete image file." + Environment.NewLine + ExceptionHelper.GetErrorMessage(ex, true));
            }
        }

        [Route("")]
        [HttpPost]
        public async Task<ActionResult<ServiceActionResult>> Add([FromForm] string formValue, IFormFile[]? images)
        {
            try
            {
                MstItem model = JsonSerializer.Deserialize<MstItem>(formValue);

                //model.SetDefault();

                ServiceActionResult result = new ServiceActionResult();
                result.ModelValidations = _itemService.CanContinueAction(ServiceAction.Add, model);

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
                        result.ActionResult = _itemService.CreateMstItem(model);
                        result.ActionIsSuccess = ((int)result.ActionResult) > 0;

                        await ZipArchiveAsync();
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
                _logger.LogError(ex, "Failed to add item.");
                return StatusCode(500, "An error occurred while add item." + Environment.NewLine +
                    ExceptionHelper.GetErrorMessage(ex, true));
            }
        }

        [Route("{itemId}")]
        [HttpPut]
        public async Task<ActionResult<ServiceActionResult>> Update([FromForm] string formValue, IFormFile[]? images)
        {
            try
            {
                MstItem model = JsonSerializer.Deserialize<MstItem>(formValue);

                //model.SetDefault();

                ServiceActionResult result = new ServiceActionResult();
                result.ModelValidations = _itemService.CanContinueAction(ServiceAction.Update, model);

                if (result.ModelValidations.Count == 0)
                {
                    result = SaveFile(ref model, images);

                    if (!result.ActionIsSuccess)
                    {
                        return Ok(result);
                    }

                    try
                    {
                        result.ActionResult = _itemService.UpdateMstItem(model);
                        result.ActionIsSuccess = ((int)result.ActionResult) > 0;

                        await ZipArchiveAsync();
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
                _logger.LogError(ex, "Failed to update item.");
                return StatusCode(500, "An error occurred while update item." + Environment.NewLine +
                    ExceptionHelper.GetErrorMessage(ex, true));
            }
        }

        [Route("{itemId}")]
        [HttpDelete]
        public async Task<ActionResult<ServiceActionResult>> Delete([FromRoute] int itemId)
        {
            try
            {
                ServiceActionResult result = new ServiceActionResult();
                result.ModelValidations = _itemService.CanContinueAction(
                    ServiceAction.Delete,
                    new MstItem
                    {
                        ItemId = itemId
                    });

                if (result.ModelValidations.Count == 0)
                {
                    try
                    {
                        MstItem? item = _itemService.DeleteMstItem(itemId);
                        if (item != null)
                        {
                            DeleteFile(item);
                        }
                        result.ActionResult = true;
                        result.ActionIsSuccess = true;

                        await ZipArchiveAsync();
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
                _logger.LogError(ex, "Failed to delete item.");
                return StatusCode(500, "An error occurred while delete item." + Environment.NewLine +
                    ExceptionHelper.GetErrorMessage(ex, true));
            }
        }
    }

}
