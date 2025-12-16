using AI_Marketplace.Application.Products.DTOs;
using AI_Marketplace.Application.Products.Queries.GetAllProducts;
using AI_Marketplace.Application.Products.Queries.GetProductById;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
        public async Task<ActionResult> GetAllProducts
            (
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? sortBy = "date",
        [FromQuery] string? sortDirection = "desc"
            )
        {
            var query = new GetAllProductsQuery
            {
                Page = page,
                PageSize = pageSize,
                SortBy = sortBy,
                SortDirection = sortDirection
            };

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
    }
}
