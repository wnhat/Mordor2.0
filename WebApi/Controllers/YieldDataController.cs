using CoreClass;
using CoreClass.Model;
using CoreClass.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class YieldDataController : ControllerBase
    {
        private readonly IYieldService _yieldService;

        public YieldDataController(IYieldService yieldService)
        {
            _yieldService = yieldService;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetThisShiftYieldData()
        {
            try
            {
                var results = await _yieldService.GetThisShiftYield();
                string json = JsonConvert.SerializeObject(results, JsonSerializerSetting.FrontConvertSetting);
                return Ok(json);
            }
            catch (Exception e)
            {
                return BadRequest(new { message = e.Message });
            }
        }
    }
}
