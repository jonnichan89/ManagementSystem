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
    public class ProjectSalesOrderController : ControllerBase
    {
        //private readonly IStringLocalizer<ProjectSalesOrderController> _res;
        private readonly SharedResources _resShared;
        private readonly ILogger<ProjectSalesOrderController> _logger;
        private readonly ProjectSalesOrderService _ProjectSalesOrderService;
        //private readonly IWebHostEnvironment _env;

        public ProjectSalesOrderController(
            //IStringLocalizer<ProjectSalesOrderController> res,
            SharedResources resShared,
            ILogger<ProjectSalesOrderController> logger,
            ProjectSalesOrderService ProjectSalesOrderService)
        //IWebHostEnvironment env)
        {
            //_res = res;
            _resShared = resShared;
            _logger = logger;
            _ProjectSalesOrderService = ProjectSalesOrderService;
        }

        [HttpGet]
        public ActionResult<ServiceActionResult> Get()
        {
            ServiceActionResult result = new ServiceActionResult();
            try
            {
                result.ActionResult = _ProjectSalesOrderService.GetProjectSalesOrders();
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
                result.ActionResult = _ProjectSalesOrderService.GetProjectSalesOrderById(PSId);
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
                result.ActionResult = _ProjectSalesOrderService.SearchProjectSalesOrdersForOptions(searchPS);
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
                model.PSType = "PPO";
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
                result.ModelValidations = _ProjectSalesOrderService.CanContinueAction(ServiceAction.Add, model);

                if (result.ModelValidations.Count == 0)
                {
                    try
                    {
                        result.ActionResult = _ProjectSalesOrderService.CreateProjectSalesOrder(model);
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
                _logger.LogError(ex, "Failed to add project sales order.");
                return StatusCode(500, "An error occurred while add project sales order." + Environment.NewLine +
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
                result.ModelValidations = _ProjectSalesOrderService.CanContinueAction(ServiceAction.Update, model);

                if (result.ModelValidations.Count == 0)
                {
                    try
                    {
                        result.ActionResult = _ProjectSalesOrderService.UpdateProjectSalesOrder(model);
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
                _logger.LogError(ex, "Failed to update project sales order.");
                return StatusCode(500, "An error occurred while update project sales order." + Environment.NewLine +
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
                result.ModelValidations = _ProjectSalesOrderService.CanContinueAction(
                    ServiceAction.Delete,
                    new PSOrder
                    {
                        PSId = PSId
                    });

                if (result.ModelValidations.Count == 0)
                {
                    try
                    {
                        result.ActionResult = _ProjectSalesOrderService.DeleteProjectSalesOrder(PSId);
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
                _logger.LogError(ex, "Failed to delete project sales order.");
                return StatusCode(500, "An error occurred while delete project sales order." + Environment.NewLine +
                    ExceptionHelper.GetErrorMessage(ex, true));
            }
        }
    }
}
