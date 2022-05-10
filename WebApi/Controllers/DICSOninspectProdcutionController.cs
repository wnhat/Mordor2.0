using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreClass.Model;
using CoreClass;
using CoreClass.Service;
using MongoDB.Bson;
using MongoDB.Bson.IO;

namespace WebApi.Controllers
{
    /// <summary>
    /// 控制器
    /// 控制DICS在检产品的种类，通过修改 ProductInfo 表中的 OnInspectTypes 字段来控制 DICS 检查的种类；
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class DICSOninspectProdcutionController :ControllerBase
    {
        private readonly IProductInfoService _productInfoService;

        public DICSOninspectProdcutionController(IProductInfoService productInfoService)
        {
            _productInfoService = productInfoService;
        }

        [HttpGet]
        public async Task<IActionResult> GetOninspectProduction()
        {
            try
            {
                var result = await _productInfoService.GetOninspectProduction();
                var final = result.ToJson(new JsonWriterSettings() { OutputMode = JsonOutputMode.Strict });
                return Ok(final);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        /// <summary>
        /// 修改 ProductInfo 中的信息，控制DICS在检型号信息；
        /// </summary>
        /// <param name="productid"></param>
        /// <param name="types"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPut("{productid}")]
        public async Task<IActionResult> ChangeOninspectProduction(string productid, string[] types)
        {
            try
            {
                await _productInfoService.UpdateOninspectProduct(productid, types);
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(new { message = e.Message });
            }
        }
    }
}
