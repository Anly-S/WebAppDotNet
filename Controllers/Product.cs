using Microsoft.AspNetCore.Mvc;
using WebApplicationDotNET.Interfaces;
using WebApplicationDotNET.Models;
using System.Linq;
using WebApplicationDotNET.Services;

namespace WebApplicationDotNET.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly IUserService _userService;

        public ProductController(IProductService productService, IUserService userService)
        {
            _productService = productService;
            _userService = userService;
        }

        [HttpGet("products", Name = "GetProducts")]
        public IActionResult GetProducts()
        {
            var response = new ApiResponse();
            var products = _productService.GetAllProducts();
            if (products == null || !products.Any())
            {
                response.status = "fail";
                response.count = 0;
                response.error = "No products found.";
                return BadRequest(response);
            }
            else
            {
                response.status = "success";
                response.data = products;
                response.count = products.Count();
                return Ok(response);
            }
        }

        [HttpPost("add", Name = "AddProduct")]
        public IActionResult AddProduct([FromBody] ProductDetails product, [FromHeader] string username)
        {
            var response = new ApiResponse();
            try
            {
                var userRoleResponse = _userService.IsUserInRole(username, "Admin");
                if (userRoleResponse.status == "fail")
                {
                    response.status = "fail";
                    response.error = "Unauthorized: Only admins can add products.";
                    return Unauthorized(response);
                }

                _productService.AddProduct(product);

                response.status = "success";
                response.data = product;
                response.count = 1;
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.status = "fail";
                response.error = ex.Message;
                return BadRequest(response);
            }
        }

        [HttpPost("buy", Name = "BuyProduct")]
        public IActionResult BuyProduct([FromBody] BuyProductRequest request)
        {
            var response = new ApiResponse();

            try
            {
                var product = _productService.BuyProduct(request.ProductCode, request.Quantity);

                if (product != null)
                {
                    response.status = "success";
                    response.data = product;
                    response.count = 1;
                    return Ok(response);
                }
                else
                {
                    response.status = "fail";
                    response.error = "Product not found or insufficient stock.";
                    return BadRequest(response);
                }
            }
            catch (Exception ex)
            {
                response.status = "fail";
                response.error = ex.Message;
                return BadRequest(response);
            }
        }

        [HttpPut("update", Name = "UpdateProduct")]
        public IActionResult UpdateProduct([FromBody] ProductDetails product, [FromHeader] string username)
        {
            var response = new ApiResponse();

            try
            {
                var userRoleResponse = _userService.IsUserInRole(username, "Admin");
                if (userRoleResponse.status == "fail")
                {
                    response.status = "fail";
                    response.error = "Unauthorized: Only admins can update products.";
                    return Unauthorized(response);
                }

                var updated = _productService.UpdateProduct(product);
                if (!updated)
                {
                    response.status = "fail";
                    response.count = 0;
                    response.error = "Product not found.";
                    return NotFound(response);
                }
                else
                {
                    response.status = "success";
                    response.data = product;
                    response.count = 1;
                    return Ok(response);
                }
            }
            catch (Exception ex)
            {
                response.status = "fail";
                response.error = ex.Message;
                return BadRequest(response);
            }
        }

        [HttpDelete("delete/{productCode}", Name = "DeleteProduct")]
        public IActionResult DeleteProduct(string productCode, [FromHeader] string username)
        {
            var response = new ApiResponse();

            try
            {
                // Check if the user is in the Admin role
                var userRoleResponse = _userService.IsUserInRole(username, "Admin");
                if (userRoleResponse.status == "fail")
                {
                    response.status = "fail";
                    response.error = "Unauthorized: Only admins can delete products.";
                    return Unauthorized(response);
                }

                var deleted = _productService.DeleteProduct(productCode);
                if (!deleted)
                {
                    response.status = "fail";
                    response.count = 0;
                    response.error = "Product not found.";
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
                response.status = "fail";
                response.error = ex.Message;
                return BadRequest(response);
            }
        }
    }
}
