using ManagementSystem.Common.Models;
using ManagementSystem.Common.Utilities;
using ManagementSystem.Data.Models;
using ManagementSystem.DataServices.Resources;
using ManagementSystem.DataServices.Service;
using Microsoft.AspNetCore.Mvc;

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
        }

        [HttpGet]
        public ActionResult<ServiceActionResult> Get()
        {
            ServiceActionResult result = new ServiceActionResult();
            try
            {
                result.ActionResult = _itemService.GetMstItems();
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

        private ServiceActionResult SaveFile(ref MstItem model, IFormFile? image)
        {
            ServiceActionResult result = new ServiceActionResult();
            result.ActionIsSuccess = true;

            // Image validation: allowed types and max size
            // Allowed extensions and content types
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
            var allowedContentTypes = new[] { "image/jpeg", "image/png" };
            // Max size 2 MB
            const long maxFileSize = 2 * 1024 * 1024;

            if (image != null && image.Length > 0)
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
                    var webRoot = _env?.WebRootPath;
                    if (string.IsNullOrWhiteSpace(webRoot))
                        webRoot = Path.Combine(Directory.GetCurrentDirectory());

                    var uploadsFolder = Path.Combine(webRoot, "uploads", "items");
                    Directory.CreateDirectory(uploadsFolder);

                    var fileName = $"{model.ItemCode}{ext}";
                    var filePath = Path.Combine(uploadsFolder, fileName);

                    using (var stream = System.IO.File.Create(filePath))
                    {
                        image.CopyTo(stream);
                    }

                    // Store relative path so client can access via static files
                    model.ImagePath = $"/items/{fileName}";
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
                // Determine target folder: wwwroot/uploads/items
                var webRoot = _env?.WebRootPath;
                if (string.IsNullOrWhiteSpace(webRoot))
                    webRoot = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");

                var filePath = Path.Combine(webRoot, "uploads", "items", model.ItemCode);
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
        public ActionResult<ServiceActionResult> Add([FromForm] MstItem model, IFormFile? image)
        {
            try
            {
                ServiceActionResult result = new ServiceActionResult();
                result.ModelValidations = _itemService.CanContinueAction(ServiceAction.Add, model);

                if (result.ModelValidations.Count == 0)
                {
                    ServiceActionResult saveFileResult = SaveFile(ref model, image);

                    if (!saveFileResult.ActionIsSuccess)
                    {
                        result.ErrorMessage = saveFileResult.ErrorMessage;
                        return Ok(result);
                    }

                    try
                    {
                        result.ActionResult = _itemService.CreateMstItem(model);
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
                _logger.LogError(ex, "Failed to add item.");
                return StatusCode(500, "An error occurred while add item." + Environment.NewLine +
                    ExceptionHelper.GetErrorMessage(ex, true));
            }
        }

        [Route("{itemId}")]
        [HttpPut]
        public ActionResult<ServiceActionResult> Update([FromForm] MstItem model, IFormFile? image)
        {
            try
            {
                ServiceActionResult result = new ServiceActionResult();
                result.ModelValidations = _itemService.CanContinueAction(ServiceAction.Update, model);

                if (result.ModelValidations.Count == 0)
                {
                    result = SaveFile(ref model, image);

                    if (!result.ActionIsSuccess)
                    {
                        return Ok(result);
                    }

                    try
                    {
                        result.ActionResult = _itemService.UpdateMstItem(model);
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
                _logger.LogError(ex, "Failed to update item.");
                return StatusCode(500, "An error occurred while update item." + Environment.NewLine +
                    ExceptionHelper.GetErrorMessage(ex, true));
            }
        }

        [Route("{itemId}")]
        [HttpDelete]
        public ActionResult<ServiceActionResult> Delete([FromRoute] int itemId)
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
