using AI_Marketplace.Application.Orders.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace AI_Marketplace.Application.Orders.Commands
{
    public class ChangeOrderStatusCommand : IRequest<OrderDto>
    {
        public int OrderId { get; set; }
        public string Status { get; set; }
        public int UserId { get; set; }
        public ChangeOrderStatusCommand(int orderId, string status, int userId)
        {
            OrderId = orderId;
            Status = status;
            UserId = userId;
        }
    }
}
