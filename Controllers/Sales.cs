using Microsoft.AspNetCore.Mvc;
using WebApplicationDotNET.Interfaces;
using WebApplicationDotNET.Models;
using WebApplicationDotNET.Services;

namespace WebApplicationDotNET.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SalesController : ControllerBase
    {
        private readonly ISalesService _salesService;
        private readonly IUserService _userService;
        private readonly ILogger<SalesController> _logger;

        public SalesController(ILogger<SalesController> logger, ISalesService salesService, IUserService userService)
        {
            _salesService = salesService;
            _userService = userService;
            _logger = logger;
        }

        [HttpGet("sales", Name = "GetSales")]
        public IActionResult GetSales()
        {
            var response = new ApiResponse();
            try
            {
                var sales = _salesService.GetAllSales();
                if (sales == null || !sales.Any())
                {
                    response.status = "fail";
                    response.count = 0;
                    response.error = "No sales found.";
                    return NotFound(response);
                }
                else
                {
                    response.status = "success";
                    response.data = sales;
                    response.count = sales.Count();
                    return Ok(response);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving sales.");
                response.status = "fail";
                response.error = "An error occurred while retrieving sales.";
                return BadRequest(response);
            }
        }

        [HttpDelete("delete/{id}", Name = "DeleteSale")]
        public IActionResult DeleteSale(int id, [FromHeader] string username)
        {
            var response = new ApiResponse();

            try
            {
                var isAdmin = _userService.IsUserInRole(username, "Admin");
                if (isAdmin==null)
                {
                    response.status = "fail";
                    response.error = "Unauthorized: Only admins can delete sales.";
                    return Unauthorized(response);
                }

                var deleted = _salesService.DeleteSale(id);
                if (!deleted)
                {
                    response.status = "fail";
                    response.count = 0;
                    response.error = "Sale not found.";
                    return NotFound(response);
                }
                else
                {
                    response.status = "success";
                    response.count = 0;
                    return Ok(response);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting sale.");
                response.status = "fail";
                response.error = "An error occurred while deleting the sale.";
                return BadRequest(response);
            }
        }
    }
}
