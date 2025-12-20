using AI_Marketplace.Application.Reviews.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace AI_Marketplace.Application.Reviews.Queries
{
    public class GetAllProductReviewsQuery : IRequest<List<ReviewDto>?>
    {
        public int ProductId { get ; set ;}

        public GetAllProductReviewsQuery(int ProductId)
        {
            this.ProductId = ProductId;
        }
    }
}
