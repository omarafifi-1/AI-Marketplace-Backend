using AI_Marketplace.Application.Common.Interfaces;
using AI_Marketplace.Application.Reviews.DTOs;
using AI_Marketplace.Domain.Entities;
using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace AI_Marketplace.Application.Reviews.Commands
{
    public class CreateProductReviewCommandHandler : IRequestHandler<CreateProductReviewCommand, int>
    {
        private readonly IReviewsRepository _reviewsRepo;
        private readonly IMapper _mapper;


        public CreateProductReviewCommandHandler(IReviewsRepository reviewsRepo, IMapper mapper)
        {
            _reviewsRepo = reviewsRepo;
            _mapper = mapper;

        }
        public async Task<int> Handle(CreateProductReviewCommand request, CancellationToken cancellationToken)
        {
            var review = _mapper.Map<Review>(request.prodReviewDto);

            review.UserId = request.UserId;
            review.StoreId = null; 

            return await _reviewsRepo.AddProductReviewAsync(review, cancellationToken);
        }
    }
}
