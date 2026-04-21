using ManagementSystem.Common.Models;
using ManagementSystem.Common.OtherModels;
using ManagementSystem.Common.Utilities;
using ManagementSystem.Data.Models;
using ManagementSystem.DataServices.Resources;
using ManagementSystem.DataServices.Service;
using Microsoft.AspNetCore.Mvc;

namespace ManagementSystem.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SalesOrderController : ControllerBase
    {
        //private readonly IStringLocalizer<SalesOrderController> _res;
        private readonly SharedResources _resShared;
        private readonly ILogger<SalesOrderController> _logger;
        private readonly SalesOrderService _SalesOrderService;
        //private readonly IWebHostEnvironment _env;

        public SalesOrderController(
            //IStringLocalizer<SalesOrderController> res,
            SharedResources resShared,
            ILogger<SalesOrderController> logger,
            SalesOrderService SalesOrderService)
        //IWebHostEnvironment env)
        {
            //_res = res;
            _resShared = resShared;
            _logger = logger;
            _SalesOrderService = SalesOrderService;
        }

        [HttpGet]
        public ActionResult<ServiceActionResult> Get()
        {
            ServiceActionResult result = new ServiceActionResult();
            try
            {
                result.ActionResult = _SalesOrderService.GetSalesOrders();
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
                result.ActionResult = _SalesOrderService.GetSalesOrderById(PSId);
                result.ActionIsSuccess = (result.ActionResult != null);
            }
            catch (Exception ex)
            {
                result.ActionIsSuccess = false;
                result.ErrorMessage = ExceptionHelper.GetErrorMessage(ex, true);
            }
            return Ok(result);
        }

        [Route("Search")]
        [HttpGet]
        public ActionResult<ServiceActionResult> Get([FromBody] SearchPSOrderForOptions searchPS )
        {
            ServiceActionResult result = new ServiceActionResult();
            try
            {
                result.ActionResult = _SalesOrderService.SearchSalesOrdersForOptions(searchPS);
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
                result.ModelValidations = _SalesOrderService.CanContinueAction(ServiceAction.Add, model);

                if (result.ModelValidations.Count == 0)
                {
                    try
                    {
                        result.ActionResult = _SalesOrderService.CreateSalesOrder(model);
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
                _logger.LogError(ex, "Failed to add sales order.");
                return StatusCode(500, "An error occurred while add sales order." + Environment.NewLine +
                    ExceptionHelper.GetErrorMessage(ex, true));
            }
        }

        [Route("{PSId}")]
        [HttpPut]
        public ActionResult<ServiceActionResult> Update([FromRoute] int PSId, [FromBody] PSOrder model)
        {
            try
            {
                foreach (var item in model.PSOrderDetails)
                {
                    item.PSOrder = model;
                }

                ServiceActionResult result = new ServiceActionResult();
                result.ModelValidations = _SalesOrderService.CanContinueAction(ServiceAction.Update, model);

                if (result.ModelValidations.Count == 0)
                {
                    try
                    {
                        result.ActionResult = _SalesOrderService.UpdateSalesOrder(model);
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
                _logger.LogError(ex, "Failed to update sales order.");
                return StatusCode(500, "An error occurred while update sales order." + Environment.NewLine +
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
                result.ModelValidations = _SalesOrderService.CanContinueAction(
                    ServiceAction.Delete,
                    new PSOrder
                    {
                        PSId = PSId
                    });

                if (result.ModelValidations.Count == 0)
                {
                    try
                    {
                        result.ActionResult = _SalesOrderService.DeleteSalesOrder(PSId);
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
                _logger.LogError(ex, "Failed to delete sales order.");
                return StatusCode(500, "An error occurred while delete sales order." + Environment.NewLine +
                    ExceptionHelper.GetErrorMessage(ex, true));
            }
        }
    }
}
