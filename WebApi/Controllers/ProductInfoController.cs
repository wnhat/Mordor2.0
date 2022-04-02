using DICS_WebApi.Models;
using WebApi.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DICS_WebApi.Controllers
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class ProductInfoController : ControllerBase
    {
        private readonly IProductInfoService _productInfoService;

        public ProductInfoController(IProductInfoService productInfoService)
        {
            _productInfoService = productInfoService;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetAllProductInfo()
        {
            try
            {
                var products = await _productInfoService.GetProductInfos();
                string json = JsonConvert.SerializeObject(products);
                return Ok(json);
            }
            catch (Exception e)
            {
                return BadRequest(new { message = e.Message });
            }
        }

        [HttpPut]
        public async Task<IActionResult> UploadImg(ProductInfo product)
        {
            try
            {
                await _productInfoService.UploadIMG(product);
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(new { message = e.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddNewProduct(ProductInfo product)
        {
            try
            {
                await _productInfoService.AddNewProduct(product);
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(new { message = e.Message });
            }
        }

        [HttpPut("{name}")]
        public async Task<IActionResult> UpdateProductInfo(ProductInfo product)
        {
            try
            {
                await _productInfoService.UpdateProduct(product);
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(new { message = e.Message });
            }
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteProductInfo(ProductInfo product)
        {
            try
            {
                await _productInfoService.DeleteProduct(product);
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(new { message = e.Message });
            }
        }
    }
}
