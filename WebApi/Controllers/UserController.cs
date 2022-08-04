using AutoMapper;
using WebApi.Dtos;
using CoreClass.Model;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using CoreClass.Service;

namespace WebApi.Controllers
{
    //AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        public static JwtSecurityTokenHandler JwtTokenHandler = new();
        public static SymmetricSecurityKey SecurityKey = new(Guid.NewGuid().ToByteArray());

        private readonly IUserService _userService;
        private readonly IMapper _mapper;

        public UserController( IUserService userService, IMapper mapper)
        {
            _userService = userService;
            _mapper = mapper;
        }

        // POST api/<UserController>/register
        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Post(UserDto userDto)
        {
            var user = _mapper.Map<User>(userDto);
            try
            {
                var _user = await _userService.CreateUser(user, userDto.Password);
                JsonResult json = new(_mapper.Map<UserDto>(_user))
                {
                    StatusCode = 200,
                    ContentType = "application/json"
                };
                return Ok(json);
            }
            catch (Exception e)
            {
                return BadRequest(new { message = e.Message });
            }
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login(UserDto userDto)
        {
            try {
                var _user = await _userService.Authenticate(userDto.Account, userDto.Password);

                if (_user == null)
                {
                    return BadRequest(new { message = "Username or password is incorrect" });
                }

                var claims = new[]
                {
                    new Claim(ClaimTypes.Name, _user.Account),
                    new Claim(ClaimTypes.Role, _user.UserWeight)
                };
                var credentials = new SigningCredentials(SecurityKey, SecurityAlgorithms.HmacSha256);
                var token = new JwtSecurityToken("DICS-WebAPI",
                                                "DICS-WebAPIClients",
                                                claims,
                                                expires: DateTime.Now.AddHours(1),
                                                signingCredentials: credentials);
                var tokenString = JwtTokenHandler.WriteToken(token);

                return Ok(new
                {
                    Message = "Success",
                    Type = "Bearer",
                    Token = tokenString,
                    _user.UserWeight
                });

            } catch (Exception e) {
                return BadRequest(new { message = e.Message });
            }

        }

        // GET: api/<UserController>
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var users = await _userService.GetAll();
            var userDtos = _mapper.Map<List<UserDto>>(users);
            return Ok(userDtos);
        }

        // GET api/<UserController>/{MongoDB Object Id}
        [HttpGet("{account}")]
        public async Task<IActionResult> Get(string account)
        {
            var user = await _userService.GetUserByAccount(account);
            var userDto = _mapper.Map<UserDto>(user);
            return Ok(userDto);
        }

        // PUT api/<UserController>/{MongoDB Object Id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, UserDto userDto)
        {
            var user = _mapper.Map<User>(userDto);
            user.Id = new ObjectId(id);

            try
            {
                await _userService.UpdateUserInfo(user, userDto.Password);
                return Ok();
            }
            catch (ApplicationException e)
            {
                return BadRequest(new { message = e.Message });
            }
        }

        // DELETE api/<UserController>/{account}
        [Authorize(Roles = "1")]
        [HttpDelete("{account}")]
        public async Task<IActionResult> Delete(string account)
        {
            await _userService.DeleteUser(account);
            return Ok(new { message = "Success" });
        }
    }
}
