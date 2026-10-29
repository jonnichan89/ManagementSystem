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
    public class MstAccountController : ControllerBase
    {
        //private readonly IStringLocalizer<MstAccountController> _res;
        private readonly SharedResources _resShared;
        private readonly ILogger<MstAccountController> _logger;
        private readonly MstAccountService _accountService;

        public MstAccountController(
            //IStringLocalizer<MstAccountController> res,
            SharedResources resShared,
            ILogger<MstAccountController> logger,
            MstAccountService accountService)
        {
            //_res = res;
            _resShared = resShared;
            _logger = logger;
            _accountService = accountService;
        }

        [Route("LastModified")]
        [HttpGet]
        public async Task<ActionResult<ServiceActionResult>> GetLastModified()
        {
            ServiceActionResult result = new ServiceActionResult();
            try
            {
                DateTime? lastModified = await _accountService.GetLastModifiedAsync();
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

        [HttpGet]
        public ActionResult<ServiceActionResult> Get()
        {
            ServiceActionResult result = new ServiceActionResult();
            try
            {
                result.ActionResult = _accountService.GetMstAccounts();
                result.ActionIsSuccess = true;
            }
            catch (Exception ex)
            {
                result.ActionIsSuccess = false;
                result.ErrorMessage = ExceptionHelper.GetErrorMessage(ex, true);
            }
            return Ok(result);
        }

        [Route("{accountId}")]
        [HttpGet]
        public ActionResult<ServiceActionResult> Get([FromRoute] int accountId)
        {
            ServiceActionResult result = new ServiceActionResult();
            try
            {
                result.ActionResult = _accountService.GetMstAccountById(accountId);
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
        public ActionResult<ServiceActionResult> Add([FromBody] MstAccount model)
        {
            try
            {
                ServiceActionResult result = new ServiceActionResult();
                result.ModelValidations = _accountService.CanContinueAction(ServiceAction.Add, model);

                if (result.ModelValidations.Count == 0)
                {
                    try
                    {
                        result.ActionResult = _accountService.CreateMstAccount(model);
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
                _logger.LogError(ex, "Failed to add account.");
                return StatusCode(500, "An error occurred while add account." + Environment.NewLine +
                    ExceptionHelper.GetErrorMessage(ex, true));
            }
        }

        [Route("{accountId}")]
        [HttpPut]
        public ActionResult<ServiceActionResult> Update([FromRoute] int accountId, [FromBody] MstAccount model)
        {
            try
            {
                ServiceActionResult result = new ServiceActionResult();
                result.ModelValidations = _accountService.CanContinueAction(ServiceAction.Update, model);

                if (result.ModelValidations.Count == 0)
                {
                    try
                    {
                        result.ActionResult = _accountService.UpdateMstAccount(model);
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
                _logger.LogError(ex, "Failed to update account.");
                return StatusCode(500, "An error occurred while update account." + Environment.NewLine +
                    ExceptionHelper.GetErrorMessage(ex, true));
            }
        }

        [Route("{accountId}")]
        [HttpDelete]
        public ActionResult<ServiceActionResult> Delete([FromRoute] int accountId)
        {
            try
            {
                ServiceActionResult result = new ServiceActionResult();
                result.ModelValidations = _accountService.CanContinueAction(
                    ServiceAction.Delete,
                    new MstAccount
                    {
                        AccountId = accountId
                    });

                if (result.ModelValidations.Count == 0)
                {
                    try
                    {
                        result.ActionResult = _accountService.DeleteMstAccount(accountId);
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
                _logger.LogError(ex, "Failed to delete account.");
                return StatusCode(500, "An error occurred while delete account." + Environment.NewLine +
                    ExceptionHelper.GetErrorMessage(ex, true));
            }
        }
    }

}
