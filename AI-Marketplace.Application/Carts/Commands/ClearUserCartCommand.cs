using AI_Marketplace.Application.Carts.DTOs;
using MediatR;


namespace AI_Marketplace.Application.Carts.Commands
{
    public record ClearUserCartCommand(int UserId) : IRequest<CartDto?>;
}
