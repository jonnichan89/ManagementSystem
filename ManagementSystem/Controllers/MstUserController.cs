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
    public class MstUserController : ControllerBase
    {
        //private readonly IStringLocalizer<MstUserController> _res;
        private readonly SharedResources _resShared;
        private readonly ILogger<MstUserController> _logger;
        private readonly MstUserService _userService;

        public MstUserController(
            //IStringLocalizer<MstUserController> res,
            SharedResources resShared,
            ILogger<MstUserController> logger,
            MstUserService userService)
        {
            //_res = res;
            _resShared = resShared;
            _logger = logger;
            _userService = userService;
        }

        [HttpGet]
        public ActionResult<ServiceActionResult> Get()
        {
            ServiceActionResult result = new ServiceActionResult();
            try
            {
                result.ActionResult = _userService.GetMstUsers();
                result.ActionIsSuccess = true; // ((IEnumerable<MstUser>)result.ActionResult).Count() > 0;
            }
            catch (Exception ex)
            {
                result.ActionIsSuccess = false;
                result.ErrorMessage = ExceptionHelper.GetErrorMessage(ex, true);
            }
            return Ok(result);
        }

        [Route("{userId}")]
        [HttpGet]
        public ActionResult<ServiceActionResult> Get([FromRoute] string userId)
        {
            ServiceActionResult result = new ServiceActionResult();
            try
            {
                result.ActionResult = _userService.GetMstUserById(userId);
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
        public ActionResult<ServiceActionResult> Add([FromBody] MstUser model)
        {
            try
            {
                ServiceActionResult result = new ServiceActionResult();
                result.ModelValidations = _userService.CanContinueAction(ServiceAction.Add, model);

                if (result.ModelValidations.Count == 0)
                {
                    try
                    {
                        result.ActionResult = _userService.CreateMstUser(model);
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
                _logger.LogError(ex, "Failed to add user.");
                return StatusCode(500, "An error occurred while add user." + Environment.NewLine +
                    ExceptionHelper.GetErrorMessage(ex, true));
            }
        }

        [Route("{userId}")]
        [HttpPut]
        public ActionResult<ServiceActionResult> Update([FromRoute] string userId, [FromBody] MstUser model)
        {
            try
            {
                ServiceActionResult result = new ServiceActionResult();
                result.ModelValidations = _userService.CanContinueAction(ServiceAction.Update, model);

                if (result.ModelValidations.Count == 0)
                {
                    try
                    {
                        result.ActionResult = _userService.UpdateMstUser(model);
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
                _logger.LogError(ex, "Failed to update user.");
                return StatusCode(500, "An error occurred while update user." + Environment.NewLine +
                    ExceptionHelper.GetErrorMessage(ex, true));
            }
        }

        [Route("{userId}")]
        [HttpDelete]
        public ActionResult<ServiceActionResult> Delete([FromRoute] string userId)
        {
            try
            {
                ServiceActionResult result = new ServiceActionResult();
                result.ModelValidations = _userService.CanContinueAction(
                    ServiceAction.Delete,
                    new MstUser
                    {
                        UserId = userId
                    });

                if (result.ModelValidations.Count == 0)
                {
                    try
                    {
                        result.ActionResult = _userService.DeleteMstUser(userId);
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
                _logger.LogError(ex, "Failed to delete user.");
                return StatusCode(500, "An error occurred while delete user." + Environment.NewLine +
                    ExceptionHelper.GetErrorMessage(ex, true));
            }
        }
    }

}
