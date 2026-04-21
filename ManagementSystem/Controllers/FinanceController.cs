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
    public class FinanceController : ControllerBase
    {
        //private readonly IStringLocalizer<FinanceController> _res;
        private readonly SharedResources _resShared;
        private readonly ILogger<FinanceController> _logger;
        private readonly FinanceService _FinanceService;
        //private readonly IWebHostEnvironment _env;

        public FinanceController(
            //IStringLocalizer<FinanceController> res,
            SharedResources resShared,
            ILogger<FinanceController> logger,
            FinanceService FinanceService)
        //IWebHostEnvironment env)
        {
            //_res = res;
            _resShared = resShared;
            _logger = logger;
            _FinanceService = FinanceService;
        }

        [HttpGet]
        public ActionResult<ServiceActionResult> Get()
        {
            ServiceActionResult result = new ServiceActionResult();
            try
            {
                result.ActionResult = _FinanceService.GetFinances();
                result.ActionIsSuccess = true;
            }
            catch (Exception ex)
            {
                result.ActionIsSuccess = false;
                result.ErrorMessage = ExceptionHelper.GetErrorMessage(ex, true);
            }
            return Ok(result);
        }

        [Route("{financeId}")]
        [HttpGet]
        public ActionResult<ServiceActionResult> Get([FromRoute] int financeId)
        {
            ServiceActionResult result = new ServiceActionResult();
            try
            {
                result.ActionResult = _FinanceService.GetFinanceById(financeId);
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
        public ActionResult<ServiceActionResult> Add([FromBody] Finance model)
        {
            try
            {
                model.SetDefault();

                ServiceActionResult result = new ServiceActionResult();
                result.ModelValidations = _FinanceService.CanContinueAction(ServiceAction.Add, model);

                if (result.ModelValidations.Count == 0)
                {
                    try
                    {
                        result.ActionResult = _FinanceService.CreateFinance(model);
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
                _logger.LogError(ex, "Failed to add finance order.");
                return StatusCode(500, "An error occurred while add finance order." + Environment.NewLine +
                    ExceptionHelper.GetErrorMessage(ex, true));
            }
        }

        [Route("{financeId}")]
        [HttpPut]
        public ActionResult<ServiceActionResult> Update([FromBody] Finance model)
        {
            try
            {
                foreach (var item in model.FinanceDetails)
                {
                    item.Finance = model;
                }

                ServiceActionResult result = new ServiceActionResult();
                result.ModelValidations = _FinanceService.CanContinueAction(ServiceAction.Update, model);

                if (result.ModelValidations.Count == 0)
                {
                    try
                    {
                        result.ActionResult = _FinanceService.UpdateFinance(model);
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
                _logger.LogError(ex, "Failed to update finance order.");
                return StatusCode(500, "An error occurred while update finance order." + Environment.NewLine +
                    ExceptionHelper.GetErrorMessage(ex, true));
            }
        }

        [Route("{financeId}")]
        [HttpDelete]
        public ActionResult<ServiceActionResult> Delete([FromRoute] int financeId)
        {
            try
            {
                ServiceActionResult result = new ServiceActionResult();
                result.ModelValidations = _FinanceService.CanContinueAction(
                    ServiceAction.Delete,
                    new Finance
                    {
                        FinanceId = financeId
                    });

                if (result.ModelValidations.Count == 0)
                {
                    try
                    {
                        result.ActionResult = _FinanceService.DeleteFinance(financeId);
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
                _logger.LogError(ex, "Failed to delete finance order.");
                return StatusCode(500, "An error occurred while delete finance order." + Environment.NewLine +
                    ExceptionHelper.GetErrorMessage(ex, true));
            }
        }
    }
}
