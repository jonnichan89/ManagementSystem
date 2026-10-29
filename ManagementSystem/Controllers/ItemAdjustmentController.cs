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
    public class ItemAdjustmentController : ControllerBase
    {
        //private readonly IStringLocalizer<ItemAdjustmentController> _res;
        private readonly SharedResources _resShared;
        private readonly ILogger<ItemAdjustmentController> _logger;
        private readonly ItemAdjustmentService _itemAdjustmentService;
        //private readonly IWebHostEnvironment _env;

        public ItemAdjustmentController(
            //IStringLocalizer<ItemAdjustmentController> res,
            SharedResources resShared,
            ILogger<ItemAdjustmentController> logger,
            ItemAdjustmentService itemAdjustmentService)
        //IWebHostEnvironment env)
        {
            //_res = res;
            _resShared = resShared;
            _logger = logger;
            _itemAdjustmentService = itemAdjustmentService;
        }

        [HttpGet]
        public ActionResult<ServiceActionResult> Get()
        {
            ServiceActionResult result = new ServiceActionResult();
            try
            {
                result.ActionResult = _itemAdjustmentService.GetItemAdjustments();
                result.ActionIsSuccess = true;
            }
            catch (Exception ex)
            {
                result.ActionIsSuccess = false;
                result.ErrorMessage = ExceptionHelper.GetErrorMessage(ex, true);
            }
            return Ok(result);
        }

        [Route("{IAId}")]
        [HttpGet]
        public ActionResult<ServiceActionResult> Get([FromRoute] int IAId)
        {
            ServiceActionResult result = new ServiceActionResult();
            try
            {
                result.ActionResult = _itemAdjustmentService.GetItemAdjustmentById(IAId);
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
        public ActionResult<ServiceActionResult> Add([FromBody] ItemAdjustment model)
        {
            try
            {
                ServiceActionResult result = new ServiceActionResult();
                result.ModelValidations = _itemAdjustmentService.CanContinueAction(ServiceAction.Add, model);

                if (result.ModelValidations.Count == 0)
                {
                    try
                    {
                        result.ActionResult = _itemAdjustmentService.CreateItemAdjustment(model);
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
                _logger.LogError(ex, "Failed to add item adjustment.");
                return StatusCode(500, "An error occurred while add item adjustment." + Environment.NewLine +
                    ExceptionHelper.GetErrorMessage(ex, true));
            }
        }

        [Route("{IAId}")]
        [HttpPut]
        public ActionResult<ServiceActionResult> Update([FromBody] ItemAdjustment model)
        {
            try
            {
                foreach (var item in model.ItemAdjustmentDetails)
                {
                    item.ItemAdjustment = model;
                }

                ServiceActionResult result = new ServiceActionResult();
                result.ModelValidations = _itemAdjustmentService.CanContinueAction(ServiceAction.Update, model);

                if (result.ModelValidations.Count == 0)
                {
                    try
                    {
                        result.ActionResult = _itemAdjustmentService.UpdateItemAdjustment(model);
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
                _logger.LogError(ex, "Failed to update item adjustment.");
                return StatusCode(500, "An error occurred while update item adjustment." + Environment.NewLine +
                    ExceptionHelper.GetErrorMessage(ex, true));
            }
        }

        [Route("{IAId}")]
        [HttpDelete]
        public ActionResult<ServiceActionResult> Delete([FromRoute] int IAId)
        {
            try
            {
                ServiceActionResult result = new ServiceActionResult();
                result.ModelValidations = _itemAdjustmentService.CanContinueAction(
                    ServiceAction.Delete,
                    new ItemAdjustment
                    {
                        IAId = IAId
                    });

                if (result.ModelValidations.Count == 0)
                {
                    try
                    {
                        result.ActionResult = _itemAdjustmentService.DeleteItemAdjustment(IAId);
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
                _logger.LogError(ex, "Failed to delete item adjustment.");
                return StatusCode(500, "An error occurred while delete item adjustment." + Environment.NewLine +
                    ExceptionHelper.GetErrorMessage(ex, true));
            }
        }
    }

}
