using AI_Marketplace.Application.Categories.Commands;
using AI_Marketplace.Application.Categories.DTOs;
using AI_Marketplace.Application.Categories.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AI_Marketplace.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CategoriesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetCategories()
        {
            var query = new GetCategoriesQuery();
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategoryById(int id)
        {
            var query = new GetCategoryByIdQuery(id);
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateCategory(CreateCategoryDto createCategoryDto)
        {
            var command = new CreateCategoryCommand
            {
                Name = createCategoryDto.Name,
                Description = createCategoryDto.Description,
                ParentCategoryId = createCategoryDto.ParentCategoryId,
            };
            var result = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetCategoryById), new { id = result.Id }, result);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateCategory(int id, UpdateCategoryDto updateCategoryDto)
        {
            var command = new UpdateCategoryCommand
            {
                Id = id,
                Name = updateCategoryDto.Name,
                Description = updateCategoryDto.Description,
                ParentCategoryId = updateCategoryDto.ParentCategoryId,
                RemoveParentCategory = updateCategoryDto.RemoveParentCategory
            };
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var command = new DeleteCategoryCommand(id);
            var result = await _mediator.Send(command);
            return Ok(result);
        }
    }
}
