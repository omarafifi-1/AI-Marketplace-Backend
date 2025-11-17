using AI_Marketplace.Application.Users.DTOs;
using MediatR;
using System.Collections.Generic;

namespace AI_Marketplace.Application.Users.Queries.GetAllUsers
{
    public class GetAllUsersQuery : IRequest<List<UserResponseDto>>
    {
    }
}