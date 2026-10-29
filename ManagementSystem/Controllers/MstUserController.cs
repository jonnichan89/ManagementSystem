using ManagementSystem.Common.Models;
using ManagementSystem.Common.Utilities;
using ManagementSystem.Data.CustomModels;
using ManagementSystem.Data.Models;
using ManagementSystem.DataServices.Resources;
using ManagementSystem.DataServices.Service;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ManagementSystem.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MstUserController : ControllerBase
    {
        private readonly IStringLocalizer<MstUserController> _res;
        private readonly SharedResources _resShared;
        private readonly ILogger<MstUserController> _logger;
        private readonly MstUserService _userService;
        private readonly IConfiguration _configuration;

        public MstUserController(
            IStringLocalizer<MstUserController> res,
            SharedResources resShared,
            ILogger<MstUserController> logger,
            MstUserService userService,
            IConfiguration configuration)
        {
            _res = res;
            _resShared = resShared;
            _logger = logger;
            _userService = userService;
            _configuration = configuration;
        }

        [Authorize(Roles = UserRoles.Administrator)]
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

        [Authorize(Roles = UserRoles.Administrator)]
        [Route("{userName}")]
        [HttpGet]
        public ActionResult<ServiceActionResult> Get([FromRoute] string userName)
        {
            ServiceActionResult result = new ServiceActionResult();
            try
            {
                result.ActionResult = _userService.GetMstUserByUserName(userName);
                result.ActionIsSuccess = (result.ActionResult != null);
            }
            catch (Exception ex)
            {
                result.ActionIsSuccess = false;
                result.ErrorMessage = ExceptionHelper.GetErrorMessage(ex, true);
            }
            return Ok(result);
        }

        [Authorize(Roles = UserRoles.Administrator)]
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

        [Authorize(Roles = UserRoles.Administrator)]
        [Route("{userName}")]
        [HttpPut]
        public ActionResult<ServiceActionResult> Update([FromRoute] string userName, [FromBody] MstUser model)
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

        [Authorize(Roles = UserRoles.Administrator)]
        [Route("{userName}")]
        [HttpDelete]
        public ActionResult<ServiceActionResult> Delete([FromRoute] string userName)
        {
            try
            {
                ServiceActionResult result = new ServiceActionResult();
                result.ModelValidations = _userService.CanContinueAction(
                    ServiceAction.Delete,
                    new MstUser
                    {
                        UserName = userName
                    });

                if (result.ModelValidations.Count == 0)
                {
                    try
                    {
                        result.ActionResult = _userService.DeleteMstUser(userName);
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

        public class LoginUser
        {
            public string UserName { get; set; }
            public string UserPassword { get; set; }
        }

        public class LoginUserResponse
        {
            public string Token { get; set; }
            public string UserName { get; set; }
            public string AccessList { get; set; }
        }

        [Route("Login")]
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult<ServiceActionResult>> LoginAsync([FromBody] LoginUser loginUser)
        {
            try
            {
                ServiceActionResult result = new ServiceActionResult();

                try
                {
                    MstUser? user = _userService.CheckLogin(loginUser.UserName, loginUser.UserPassword);
                    
                    if (user == null)
                    {
                        result.ActionIsSuccess = false;
                        result.ErrorMessage = _res["UserLoginFailed"];
                    }
                    else
                    {
                        // 1. Create user identity claims
                        var claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.Name, user.UserName),
                            new Claim(ClaimTypes.NameIdentifier, user.UserId),
                            new Claim(ClaimTypes.Role, (user.UserName == UserRoles.Administrator?UserRoles.Administrator:UserRoles.User))
                        };

                        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);


                        // 2. Generate the signing key
                        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
                        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                        // 4. Create the token properties
                        var token = new JwtSecurityToken(
                            issuer: _configuration["Jwt:Issuer"],
                            audience: _configuration["Jwt:Audience"],
                            claims: claims,
                            expires: DateTime.UtcNow.AddDays(1), // Token lifetime
                            signingCredentials: creds
                        );

                        // 5. Serialize the JWT and return it to the WPF app
                        var jwtToken = new JwtSecurityTokenHandler().WriteToken(token);

                        result.ActionIsSuccess = true;
                        result.ActionResult = new LoginUserResponse() {
                            Token = jwtToken,
                            UserName = user.UserName,
                            AccessList = user.AccessList
                        };
                    }
                }
                catch (Exception ex)
                {
                    result.ErrorMessage = ExceptionHelper.GetErrorMessage(ex, true);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to authenticate user.");
                return StatusCode(500, "An error occurred while authenticate user." + Environment.NewLine +
                    ExceptionHelper.GetErrorMessage(ex, true));
            }
        }
    }

}
