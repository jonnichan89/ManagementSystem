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
    public class PSOrderController : ControllerBase
    {
        //private readonly IStringLocalizer<PSOrderController> _res;
        private readonly SharedResources _resShared;
        private readonly ILogger<PSOrderController> _logger;
        private readonly PSOrderService _PSOrderService;
        //private readonly IWebHostEnvironment _env;

        public PSOrderController(
            //IStringLocalizer<PSOrderController> res,
            SharedResources resShared,
            ILogger<PSOrderController> logger,
            PSOrderService PSOrderService)
        //IWebHostEnvironment env)
        {
            //_res = res;
            _resShared = resShared;
            _logger = logger;
            _PSOrderService = PSOrderService;
        }

        [HttpGet]
        public ActionResult<ServiceActionResult> Get()
        {
            ServiceActionResult result = new ServiceActionResult();
            try
            {
                result.ActionResult = _PSOrderService.GetPSOrders();
                result.ActionIsSuccess = true;
            }
            catch (Exception ex)
            {
                result.ActionIsSuccess = false;
                result.ErrorMessage = ExceptionHelper.GetErrorMessage(ex, true);
            }
            return Ok(result);
        }

        [Route("{PSId}")]
        [HttpGet]
        public ActionResult<ServiceActionResult> Get([FromRoute] int PSId)
        {
            ServiceActionResult result = new ServiceActionResult();
            try
            {
                result.ActionResult = _PSOrderService.GetPSOrderById(PSId);
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
        public ActionResult<ServiceActionResult> Add([FromBody] PSOrder model)
        {
            try
            {
                model.SetDefault();

                ServiceActionResult result = new ServiceActionResult();
                result.ModelValidations = _PSOrderService.CanContinueAction(ServiceAction.Add, model);

                if (result.ModelValidations.Count == 0)
                {
                    try
                    {
                        result.ActionResult = _PSOrderService.CreatePSOrder(model);
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
                _logger.LogError(ex, "Failed to add PS order.");
                return StatusCode(500, "An error occurred while add PS order." + Environment.NewLine +
                    ExceptionHelper.GetErrorMessage(ex, true));
            }
        }

        [Route("{PSId}")]
        [HttpPut]
        public ActionResult<ServiceActionResult> Update([FromBody] PSOrder model)
        {
            try
            {
                foreach (var item in model.PSOrderDetails)
                {
                    item.PSOrder = model;
                }

                ServiceActionResult result = new ServiceActionResult();
                result.ModelValidations = _PSOrderService.CanContinueAction(ServiceAction.Update, model);

                if (result.ModelValidations.Count == 0)
                {
                    try
                    {
                        result.ActionResult = _PSOrderService.UpdatePSOrder(model);
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
                _logger.LogError(ex, "Failed to update PS order.");
                return StatusCode(500, "An error occurred while update PS order." + Environment.NewLine +
                    ExceptionHelper.GetErrorMessage(ex, true));
            }
        }

        [Route("{PSId}")]
        [HttpDelete]
        public ActionResult<ServiceActionResult> Delete([FromRoute] int PSId)
        {
            try
            {
                ServiceActionResult result = new ServiceActionResult();
                result.ModelValidations = _PSOrderService.CanContinueAction(
                    ServiceAction.Delete,
                    new PSOrder
                    {
                        PSId = PSId
                    });

                if (result.ModelValidations.Count == 0)
                {
                    try
                    {
                        result.ActionResult = _PSOrderService.DeletePSOrder(PSId);
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
                _logger.LogError(ex, "Failed to delete PS order.");
                return StatusCode(500, "An error occurred while delete PS order." + Environment.NewLine +
                    ExceptionHelper.GetErrorMessage(ex, true));
            }
        }
    }
}
