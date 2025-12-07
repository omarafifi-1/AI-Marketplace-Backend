using AI_Marketplace.Application.Users.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace AI_Marketplace.Application.Users.Commands.UpdateUserProfile
{
    public class UpdateUserProfileCommand : IRequest<GetUserProfileDto>
    {
        public int UserId { get; set; }
        public UpdateUserProfileDto Dto { get; set; }

        public UpdateUserProfileCommand(int userId, UpdateUserProfileDto dto)
        {
            UserId = userId;
            Dto = dto;
        }

    }
}
