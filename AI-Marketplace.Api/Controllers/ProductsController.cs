using AI_Marketplace.Application.Products.Commands;
using AI_Marketplace.Application.Products.DTOs;
using AI_Marketplace.Application.Products.Queries.GetAllProducts;
using AI_Marketplace.Application.Products.Queries.GetProductById;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AI_Marketplace.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {

        private readonly IMediator _mediator;

        public ProductsController(IMediator mediator)
        {
            _mediator = mediator;

        }

        [HttpGet]
        public async Task<ActionResult> GetAllProducts(
     [FromQuery] int page = 1,
     [FromQuery] int pageSize = 20,
     [FromQuery] string? sortBy = "date",
     [FromQuery] string? sortDirection = "desc",
     [FromQuery] int? categoryId = null,
     [FromQuery] decimal? minPrice = null,
     [FromQuery] decimal? maxPrice = null,
     [FromQuery] string? keyword = null
 )
        {
            var query = new GetAllProductsQuery
            (
                Page: page,
                PageSize: pageSize,
                SortBy: sortBy,
                SortDirection: sortDirection,
                CategoryId: categoryId,
                MinPrice: minPrice,
                MaxPrice: maxPrice,
                Keyword: keyword
            );

            var result = await _mediator.Send(query);

            return Ok(result);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult> GetProductById(int id)
        {

            var query = new GetProductByIdQuery
            {
                Id = id
            };

            var result = await _mediator.Send(query);

            if (result == null)
                return NotFound();

            return Ok(result);
        }

        [HttpPost("Create")]
        public async Task<IActionResult> CreateProduct(CreateProductDto createProductDto)
        {
            var UserIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (UserIdString == null)
            {
                return Unauthorized();
            }
            var UserIdInt = int.Parse(UserIdString);
            if (!int.TryParse(UserIdString, out UserIdInt))
            {
                return BadRequest("Invalid User ID");
            }
            var command = new CreateProductCommand
            {
                UserId = UserIdInt,
                Name = createProductDto.Name,
                Description = createProductDto.Description,
                Price = createProductDto.Price,
                Stock = createProductDto.Stock,
                CategoryId = createProductDto.CategoryId,
                IsActive = true
            };
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpPut("EditProduct/{id:int}")]
        public async Task<IActionResult> EditProduct(int id, EditProductDto editProductDto)
        {
            var UserIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (UserIdString == null)
            {
                return Unauthorized();
            }
            var UserIdInt = int.Parse(UserIdString);
            if (!int.TryParse(UserIdString, out UserIdInt))
            {
                return BadRequest("Invalid User ID");
            }
            var command = new EditProductCommand
            {
                ProductId = id,
                UserId = UserIdInt,
                Name = editProductDto.Name,
                Description = editProductDto.Description,
                Price = editProductDto.Price,
                Stock = editProductDto.Stock,
                CategoryId = editProductDto.CategoryId,
                IsActive = editProductDto.IsActive
            };
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpDelete("DeleteProduct/{id:int}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var UserIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (UserIdString == null)
            {
                return Unauthorized();
            }
            var UserIdInt = int.Parse(UserIdString);
            if (!int.TryParse(UserIdString, out UserIdInt))
            {
                return BadRequest("Invalid User ID");
            }
            var command = new DeleteProductCommand
            {
                ProductId = id,
                UserId = UserIdInt
            };
            var result = await _mediator.Send(command);
            return Ok(result);
        }
    }
}