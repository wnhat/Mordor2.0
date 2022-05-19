using CoreClass.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using System;
using System.Threading.Tasks;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HistoryController : ControllerBase
    {
        private readonly IHistoryService _historyService;
        public HistoryController(IHistoryService historyService)
        {
            _historyService = historyService;
        }

        [HttpPost]
        public async Task<IActionResult> GetByTimeEQP(HistorySearch history)
        {
            try
            {
                var result = await _historyService.GetHistory(history.start, history.end, history.eqplist);
                var finalresult = result.ToJson(new JsonWriterSettings() { OutputMode = JsonOutputMode.Strict });
                return Ok(finalresult);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        
        [HttpPost("ID")]
        public async Task<IActionResult> GetById(string[] idlist)
        {
            try
            {
                var result = await _historyService.GetHistoryByID(idlist);
                var finalresult = result.ToJson(new JsonWriterSettings() { OutputMode = JsonOutputMode.Strict });
                return Ok(finalresult);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
    public struct HistorySearch
    {
        public DateTime start { get; set; }
        public DateTime end { get; set; }
        public int[] eqplist { get; set; }
    }
    public struct PanelIDSearch
    {
        public string[] id { get; set; }
    }
}
