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

namespace WebApi.Controllers
{
    [Authorize(Roles = "2,1")]
    [Route("api/[controller]")]
    [ApiController]
    public class ProductInfoController : ControllerBase
    {
        private readonly IProductInfoService _productInfoService;

        public ProductInfoController(IProductInfoService productInfoService)
        {
            _productInfoService = productInfoService;
        }

        /// <summary>
        /// Get all product info
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetAllProductInfo()
        {
            try
            {
                var products = await _productInfoService.GetProductInfos();
                
                string json = JsonConvert.SerializeObject(products, JsonSerializerSetting.FrontConvertSetting);
                return Ok(json);
            }
            catch (Exception e)
            {
                return BadRequest(new { message = e.Message });
            }
        }
        
        /// <summary>
        /// Upload an IMG from local path(Not in use)
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Add a new product
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Update a product's info. Return bad request if it is not exist.
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Delete product
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        [HttpDelete("{name}")]
        public async Task<IActionResult> DeleteProductInfo(string name)
        {
            try
            {
                await _productInfoService.DeleteProduct(name);
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(new { message = e.Message });
            }
        }
    }
}
