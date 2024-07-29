using Microsoft.AspNetCore.Mvc;
using WebApplicationDotNET.Models;
using WebApplicationDotNET.Services;

namespace WebApplicationDotNET.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SalesController : ControllerBase
    {
        public class Sale
        {
            public int Id { get; set; }
            public DateTime Timestamp { get; set; }
            public string ProductCode { get; set; }
            public string ProductName { get; set; }
            public int Quantity { get; set; }
            public decimal Price { get; set; }
        }

        private readonly ISalesService _salesService;
        private readonly ILogger<SalesController> _logger;

        public SalesController(ILogger<SalesController> logger, ISalesService salesService)
        {
            _salesService = salesService;
            _logger = logger;
        }

        [HttpGet("sales", Name = "GetSales")]
        public IActionResult GetSales()
        {
            var response = new ApiResponse();
            var sales = _salesService.GetAllSales();
            if (sales == null || !sales.Any())
            {

                response.status = "fail";
                    response.count = 0;
                    response.error = "No sales found";
                    return NotFound(response);

            }

            else {
                response.status = "success";
                response.data = sales;
                response.count = sales.Count();
                    return Ok(response);
            };
        }

        [HttpDelete("delete/{id}", Name = "DeleteSale")]
        public IActionResult DeleteSale(int id)
        {
            var response = new ApiResponse();
            var deleted = _salesService.DeleteSale(id);
            if (!deleted)
            {
                response.status = "fail";
                response.count = 0;
                response.error = "Sale not found";
                return NotFound(response);
            }

            else
            {
                response.status = "success";
                response.count = 0;
                return Ok(response);

            }
        }
    }
}
