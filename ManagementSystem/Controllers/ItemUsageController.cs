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
    public class ItemUsageController : ControllerBase
    {
        //private readonly IStringLocalizer<ItemUsageController> _res;
        private readonly SharedResources _resShared;
        private readonly ILogger<ItemUsageController> _logger;
        private readonly ItemUsageService _itemUsageService;
        //private readonly IWebHostEnvironment _env;

        public ItemUsageController(
            //IStringLocalizer<ItemUsageController> res,
            SharedResources resShared,
            ILogger<ItemUsageController> logger,
            ItemUsageService itemUsageService)
        //IWebHostEnvironment env)
        {
            //_res = res;
            _resShared = resShared;
            _logger = logger;
            _itemUsageService = itemUsageService;
        }

        [HttpGet]
        public ActionResult<ServiceActionResult> Get()
        {
            ServiceActionResult result = new ServiceActionResult();
            try
            {
                result.ActionResult = _itemUsageService.GetItemUsages();
                result.ActionIsSuccess = true;
            }
            catch (Exception ex)
            {
                result.ActionIsSuccess = false;
                result.ErrorMessage = ExceptionHelper.GetErrorMessage(ex, true);
            }
            return Ok(result);
        }

        [Route("{itemUsageId}")]
        [HttpGet]
        public ActionResult<ServiceActionResult> Get([FromRoute] int itemUsageId)
        {
            ServiceActionResult result = new ServiceActionResult();
            try
            {
                result.ActionResult = _itemUsageService.GetItemUsageById(itemUsageId);
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
        public ActionResult<ServiceActionResult> Add([FromBody] ItemUsage model)
        {
            try
            {
                ServiceActionResult result = new ServiceActionResult();
                result.ModelValidations = _itemUsageService.CanContinueAction(ServiceAction.Add, model);

                if (result.ModelValidations.Count == 0)
                {
                    try
                    {
                        result.ActionResult = _itemUsageService.CreateItemUsage(model);
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
                _logger.LogError(ex, "Failed to add item usage.");
                return StatusCode(500, "An error occurred while add item usage." + Environment.NewLine +
                    ExceptionHelper.GetErrorMessage(ex, true));
            }
        }

        [Route("{itemUsageId}")]
        [HttpPut]
        public ActionResult<ServiceActionResult> Update([FromBody] ItemUsage model)
        {
            try
            {
                foreach (var item in model.ItemUsageDetails)
                {
                    item.ItemUsage = model;
                }

                ServiceActionResult result = new ServiceActionResult();
                result.ModelValidations = _itemUsageService.CanContinueAction(ServiceAction.Update, model);

                if (result.ModelValidations.Count == 0)
                {
                    try
                    {
                        result.ActionResult = _itemUsageService.UpdateItemUsage(model);
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
                _logger.LogError(ex, "Failed to update item usage.");
                return StatusCode(500, "An error occurred while update item usage." + Environment.NewLine +
                    ExceptionHelper.GetErrorMessage(ex, true));
            }
        }

        [Route("{itemUsageId}")]
        [HttpDelete]
        public ActionResult<ServiceActionResult> Delete([FromRoute] int itemUsageId)
        {
            try
            {
                ServiceActionResult result = new ServiceActionResult();
                result.ModelValidations = _itemUsageService.CanContinueAction(
                    ServiceAction.Delete,
                    new ItemUsage
                    {
                        ItemUsageId = itemUsageId
                    });

                if (result.ModelValidations.Count == 0)
                {
                    try
                    {
                        result.ActionResult = _itemUsageService.DeleteItemUsage(itemUsageId);
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
                _logger.LogError(ex, "Failed to delete item usage.");
                return StatusCode(500, "An error occurred while delete item usage." + Environment.NewLine +
                    ExceptionHelper.GetErrorMessage(ex, true));
            }
        }
    }

}
