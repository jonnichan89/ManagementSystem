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
    public class PurchaseOrderController : ControllerBase
    {
        //private readonly IStringLocalizer<PurchaseOrderController> _res;
        private readonly SharedResources _resShared;
        private readonly ILogger<PurchaseOrderController> _logger;
        private readonly PurchaseOrderService _PurchaseOrderService;
        //private readonly IWebHostEnvironment _env;

        public PurchaseOrderController(
            //IStringLocalizer<PurchaseOrderController> res,
            SharedResources resShared,
            ILogger<PurchaseOrderController> logger,
            PurchaseOrderService PurchaseOrderService)
        //IWebHostEnvironment env)
        {
            //_res = res;
            _resShared = resShared;
            _logger = logger;
            _PurchaseOrderService = PurchaseOrderService;
        }

        [HttpGet]
        public ActionResult<ServiceActionResult> Get()
        {
            ServiceActionResult result = new ServiceActionResult();
            try
            {
                result.ActionResult = _PurchaseOrderService.GetPurchaseOrders();
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
        public ActionResult<ServiceActionResult> Get([FromRoute] int POId)
        {
            ServiceActionResult result = new ServiceActionResult();
            try
            {
                result.ActionResult = _PurchaseOrderService.GetPurchaseOrderById(POId);
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
                result.ActionResult = _PurchaseOrderService.SearchPurchaseOrdersForOptions(searchPS);
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
                model.PSType = "PO";
                model.SetDefault();

                int id = 0;
                foreach (PSOrderDetail childModel in model.PSOrderDetails)
                {
                    id++;
                    childModel.PSDId = id;
                    childModel.SetDefault();
                    childModel.PSOrder = model;
                }

                ServiceActionResult result = new ServiceActionResult();
                result.ModelValidations = _PurchaseOrderService.CanContinueAction(ServiceAction.Add, model);

                if (result.ModelValidations.Count == 0)
                {
                    try
                    {
                        result.ActionResult = _PurchaseOrderService.CreatePurchaseOrder(model);
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
                _logger.LogError(ex, "Failed to add purchase order.");
                return StatusCode(500, "An error occurred while add purchase order." + Environment.NewLine +
                    ExceptionHelper.GetErrorMessage(ex, true));
            }
        }

        [Route("{PSId}")]
        [HttpPut]
        public ActionResult<ServiceActionResult> Update([FromRoute] int PSId, [FromBody] PSOrder model)
        {
            try
            {
                model.SetDefault();

                int id = 0;
                foreach (PSOrderDetail childModel in model.PSOrderDetails)
                {
                    id++;
                    childModel.PSDId = id;
                    childModel.SetDefault();
                    childModel.PSOrder = model;
                }

                ServiceActionResult result = new ServiceActionResult();
                result.ModelValidations = _PurchaseOrderService.CanContinueAction(ServiceAction.Update, model);

                if (result.ModelValidations.Count == 0)
                {
                    try
                    {
                        result.ActionResult = _PurchaseOrderService.UpdatePurchaseOrder(model);
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
                _logger.LogError(ex, "Failed to update purchase order.");
                return StatusCode(500, "An error occurred while update purchase order." + Environment.NewLine +
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
                result.ModelValidations = _PurchaseOrderService.CanContinueAction(
                    ServiceAction.Delete,
                    new PSOrder
                    {
                        PSId = PSId
                    });

                if (result.ModelValidations.Count == 0)
                {
                    try
                    {
                        result.ActionResult = _PurchaseOrderService.DeletePurchaseOrder(PSId);
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
                _logger.LogError(ex, "Failed to delete purchase order.");
                return StatusCode(500, "An error occurred while delete purchase order." + Environment.NewLine +
                    ExceptionHelper.GetErrorMessage(ex, true));
            }
        }
    }
}
