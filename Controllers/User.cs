using Microsoft.AspNetCore.Mvc;
using WebApplicationDotNET.Interfaces;
using WebApplicationDotNET.Models;

namespace WebApplicationDotNET.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("create", Name = "CreateUser")]
        public IActionResult CreateUser([FromBody] UserDetails user)
        {
            var response = new ApiResponse();
            if (user == null || string.IsNullOrEmpty(user.Username) || string.IsNullOrEmpty(user.Password))
            {
                response.status = "fail";
                response.error = "Invalid user details.";
                return BadRequest(response);
            }

            try
            {
                var apiResponse = _userService.CreateUser(user.Username, user.Password, user.Role);
                if (apiResponse.status == "success")
                {
                    return Ok(apiResponse);
                }
                else
                {
                    return Conflict(apiResponse);
                }
            }
            catch (Exception ex)
            {
                response.status = "fail";
                response.error = ex.Message;
                return StatusCode(500, response);
            }
        }

        [HttpPost("login", Name = "LoginUser")]
        public IActionResult LoginUser([FromBody] Models.LoginRequest request)
        {
            var response = new ApiResponse();
            if (request == null || string.IsNullOrEmpty(request.Username) || string.IsNullOrEmpty(request.Password))
            {
                response.status = "fail";
                response.error = "Invalid login request.";
                return BadRequest(response);
            }

            try
            {
                var apiResponse = _userService.AuthenticateUser(request.Username, request.Password);
                if (apiResponse.status == "success")
                {
                    return Ok(apiResponse);
                }
                else
                {
                    return Unauthorized(apiResponse);
                }
            }
            catch (Exception ex)
            {
                response.status = "fail";
                response.error = ex.Message;
                return StatusCode(500, response);
            }
        }

        [HttpGet("role/{username}", Name = "CheckUserRole")]
        public IActionResult CheckUserRole(string username, [FromQuery] string role)
        {
            var response = new ApiResponse();
            try
            {
                var apiResponse = _userService.IsUserInRole(username, role);
                if (apiResponse.status == "success")
                {
                    return Ok(apiResponse);
                }
                else
                {
                    return NotFound(apiResponse);
                }
            }
            catch (Exception ex)
            {
                response.status = "fail";
                response.error = ex.Message;
                return StatusCode(500, response);
            }
        }
    }
}
