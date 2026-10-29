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
    public class MstCustomerController : ControllerBase
    {
        //private readonly IStringLocalizer<MstSupplierController> _res;
        private readonly SharedResources _resShared;
        private readonly ILogger<MstSupplierController> _logger;
        private readonly MstSupplierService _supplierService;

        public MstCustomerController(
            //IStringLocalizer<MstSupplierController> res,
            SharedResources resShared,
            ILogger<MstSupplierController> logger,
            MstSupplierService supplierService)
        {
            //_res = res;
            _resShared = resShared;
            _logger = logger;
            _supplierService = supplierService;
        }

        [HttpGet]
        public ActionResult<ServiceActionResult> Get()
        {
            ServiceActionResult result = new ServiceActionResult();
            try
            {
                result.ActionResult = _supplierService.GetMstSuppliers();
                result.ActionIsSuccess = true;
            }
            catch (Exception ex)
            {
                result.ActionIsSuccess = false;
                result.ErrorMessage = ExceptionHelper.GetErrorMessage(ex, true);
            }
            return Ok(result);
        }

        [Route("{supplierId}")]
        [HttpGet]
        public ActionResult<ServiceActionResult> Get([FromRoute] int supplierId)
        {
            ServiceActionResult result = new ServiceActionResult();
            try
            {
                result.ActionResult = _supplierService.GetMstSupplierById(supplierId);
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
        public ActionResult<ServiceActionResult> Add([FromBody] MstSupplier model)
        {
            try
            {
                ServiceActionResult result = new ServiceActionResult();
                result.ModelValidations = _supplierService.CanContinueAction(ServiceAction.Add, model);

                if (result.ModelValidations.Count == 0)
                {
                    try
                    {
                        result.ActionResult = _supplierService.CreateMstSupplier(model);
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
                _logger.LogError(ex, "Failed to add supplier.");
                return StatusCode(500, "An error occurred while add supplier." + Environment.NewLine +
                    ExceptionHelper.GetErrorMessage(ex, true));
            }
        }

        [Route("{supplierId}")]
        [HttpPut]
        public ActionResult<ServiceActionResult> Update([FromRoute] int supplierId, [FromBody] MstSupplier model)
        {
            try
            {
                ServiceActionResult result = new ServiceActionResult();
                result.ModelValidations = _supplierService.CanContinueAction(ServiceAction.Update, model);

                if (result.ModelValidations.Count == 0)
                {
                    try
                    {
                        result.ActionResult = _supplierService.UpdateMstSupplier(model);
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
                _logger.LogError(ex, "Failed to update supplier.");
                return StatusCode(500, "An error occurred while update supplier." + Environment.NewLine +
                    ExceptionHelper.GetErrorMessage(ex, true));
            }
        }

        [Route("{supplierId}")]
        [HttpDelete]
        public ActionResult<ServiceActionResult> Delete([FromRoute] int supplierId)
        {
            try
            {
                ServiceActionResult result = new ServiceActionResult();
                result.ModelValidations = _supplierService.CanContinueAction(
                    ServiceAction.Delete,
                    new MstSupplier
                    {
                        SupplierId = supplierId
                    });

                if (result.ModelValidations.Count == 0)
                {
                    try
                    {
                        result.ActionResult = _supplierService.DeleteMstSupplier(supplierId);
                        result.ActionIsSuccess = ((bool)result.ActionResult);
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
                _logger.LogError(ex, "Failed to delete supplier.");
                return StatusCode(500, "An error occurred while delete supplier." + Environment.NewLine +
                    ExceptionHelper.GetErrorMessage(ex, true));
            }
        }
    }

}
