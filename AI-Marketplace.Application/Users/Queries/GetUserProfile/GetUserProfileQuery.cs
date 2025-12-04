using AI_Marketplace.Application.Users.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace AI_Marketplace.Application.Users.Queries.GetUserProfile
{
    public class GetUserProfileQuery : IRequest<GetUserProfileDto>
    {
        public int UserId { get; set; }
        public GetUserProfileQuery(int userId)
        {
            UserId = userId;
        }
    }
}
