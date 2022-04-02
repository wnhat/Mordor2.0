﻿using AutoMapper;
using DICS_WebApi.Dtos;
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
    [Authorize(Roles = "2")]
    [Route("api/[controller]")]
    [ApiController]
    public class DefectCodeController : ControllerBase
    {
        private readonly IDefectCodeService _defectCodeService;
        private readonly IMapper _mapper;

        public DefectCodeController(
            IDefectCodeService defectCodeService,
            IMapper mapper)
        {
            _defectCodeService = defectCodeService;
            _mapper = mapper;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetAllDefectCodes()
        {
            try
            {
                var defectCodes = await _defectCodeService.GetAllDefectCodes();
                // var defectCodesDtos = _mapper.Map<List<DefectCodeDto>>(defectCodes);
                string json = JsonConvert.SerializeObject(defectCodes);
                return Ok(json);
            }
            catch (Exception e)
            {
                return BadRequest(new { message = e.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddDefectCode(DefectCodeDto param)
        {
            var _param = _mapper.Map<DefectCode>(param);

            try
            {
                await _defectCodeService.CreateDefectCode(_param);
            }
            catch (Exception e)
            {
                return BadRequest(new { message = e.Message });
            }

            return Ok();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(DefectCode param)
        {
            try
            {
                await _defectCodeService.UpdateDefectCode(param);
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(new { message = e.Message });
            }
        }

        // 删除单条Defect
        [HttpDelete("{code}")]
        public async Task<IActionResult> DeleteOne(string code)
        {
            try
            {
                await _defectCodeService.DeleteDefectCode(code);
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(new { message = e.Message });
            }
        }

        // 删除多条Defect
        [HttpDelete]
        public async Task<IActionResult> DeleteMany(string[] code_list)
        {
            try
            {
                await _defectCodeService.DeleteDefectCodeMany(code_list);
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(new { message = e.Message });
            }
        }
    }
}
