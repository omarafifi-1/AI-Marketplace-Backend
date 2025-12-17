using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace AI_Marketplace.Application.Admin.Commands
{
    public class UnbanUserCommand : IRequest<string>
    {
        public int UserId { get; set; }
    }
}
