using AI_Marketplace.Application.Reviews.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace AI_Marketplace.Application.Reviews.Commands
{
    public class CreateProductReviewCommand : IRequest<int>
    {
        public ProductReviewDto prodReviewDto;
        public int UserId;

        public CreateProductReviewCommand(ProductReviewDto prodReviewDto, int UserId)
        {
            this.prodReviewDto = prodReviewDto;
            this.UserId = UserId;
        }
    }
}
