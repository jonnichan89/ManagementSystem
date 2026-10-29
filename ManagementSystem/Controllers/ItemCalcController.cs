using ManagementSystem.Common.Models;
using ManagementSystem.Common.OtherModels;
using ManagementSystem.Common.Utilities;
using ManagementSystem.Data.Models;
using ManagementSystem.DataServices.Resources;
using ManagementSystem.DataServices.Service;
using Microsoft.AspNetCore.Mvc;
using System.IO.Compression;
using System.Text.Json;

namespace ManagementSystem.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ItemCalcController : ControllerBase
    {
        //private readonly IStringLocalizer<ItemCalcController> _res;
        private readonly SharedResources _resShared;
        private readonly ILogger<ItemCalcController> _logger;
        private readonly ItemCalcService _itemService;
        private readonly IWebHostEnvironment _env;

        public ItemCalcController(
            //IStringLocalizer<ItemCalcController> res,
            SharedResources resShared,
            ILogger<ItemCalcController> logger,
            ItemCalcService itemCalcService,
            IWebHostEnvironment env)
        {
            //_res = res;
            _resShared = resShared;
            _logger = logger;
            _itemService = itemCalcService;
            _env = env;
        }

        [Route("All/{itemCode}")]
        [HttpGet]
        public async Task<ActionResult<ServiceActionResult>> GetAll(string itemCode)
        {
            ServiceActionResult result = new ServiceActionResult();
            try
            {
                result.ActionResult = await _itemService.GetAllItemCalcs(itemCode);
                result.ActionIsSuccess = true;
            }
            catch (Exception ex)
            {
                result.ActionIsSuccess = false;
                result.ErrorMessage = ExceptionHelper.GetErrorMessage(ex, true);
            }
            return Ok(result);
        }

        [Route("page/{pageId}")]
        [HttpGet]
        public async Task<ActionResult<ServiceActionResult>> GetPage([FromRoute] int pageId, [FromForm] PageFilterItemCalc? pageFilter)
        {
            ServiceActionResult result = new ServiceActionResult();

            try
            {
                result.ActionResult = await _itemService.GetItemCalcs(pageId, pageFilter);
                result.ActionIsSuccess = true;
            }
            catch (Exception ex)
            {
                result.ActionIsSuccess = false;
                result.ErrorMessage = ExceptionHelper.GetErrorMessage(ex, true);
            }

            return Ok(result);
        }

    }

}
