using CoreClass;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
using WebApi.Service;

namespace WebApi.Controllers
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class RemainInsepctMissionController: ControllerBase
    {
        private readonly IDICSRemainInspectMissionService _dicsRemainInspectMissionService;
        public RemainInsepctMissionController(IDICSRemainInspectMissionService dicsRemainInspectMissionService)
        {
            _dicsRemainInspectMissionService = dicsRemainInspectMissionService;
        }

        //get all remain inspect mission;
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetAllRemainInspectMission()
        {
            try
            {
                var products = await _dicsRemainInspectMissionService.GetRemainMissionCount();
                string json = products.ToJson();
                return Ok(json);
            }
            catch (Exception e)
            {
                return BadRequest(new { message = e.Message });
            }
        }
    }
}
