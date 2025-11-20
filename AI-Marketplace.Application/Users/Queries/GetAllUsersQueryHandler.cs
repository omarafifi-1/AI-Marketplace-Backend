using AI_Marketplace.Application.Common.Interfaces;
using AI_Marketplace.Application.Users.DTOs;
using AI_Marketplace.Application.Users.Queries.GetAllUsers;
using AI_Marketplace.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AI_Marketplace.Application.Features.Users.Queries.GetAllUsers
{
    public class GetAllUsersQueryHandler : IRequestHandler<GetAllUsersQuery, List<UserResponseDto>>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IStoreRepository _storerepo;

        public GetAllUsersQueryHandler(UserManager<ApplicationUser> userManager, IStoreRepository storerepo)
        {
            _userManager = userManager;
            _storerepo = storerepo;
        }

        public async Task<List<UserResponseDto>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
        {
            var users = await _userManager.Users.ToListAsync(cancellationToken);

            var userDtos = new List<UserResponseDto>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                var store = await _storerepo.GetByOwnerIdAsync(user.Id, cancellationToken);

                userDtos.Add(new UserResponseDto
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    Email = user.Email,
                    Role = roles.FirstOrDefault() ?? string.Empty,
                    CreatedAt = user.CreatedAt,
                    StoreId = store?.Id,
                    StoreName = user.Store?.StoreName
                });
            }

            return userDtos;
        }
    }
}