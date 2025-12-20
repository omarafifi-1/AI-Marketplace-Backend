using AI_Marketplace.Application.Reviews.DTOs;
using AI_Marketplace.Domain.Entities;

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace AI_Marketplace.Application.Common.Interfaces
{
    public interface IReviewsRepository
    {
        //CRUD operations for review
        Task<List<ReviewDto>> GetAllProductReviewsAsync(int productId, CancellationToken cancellationToken);
        Task<List<ReviewDto>> GetAllStoreReviewsAsync(int storeId, CancellationToken cancellationToken);
        Task<int> AddStoreReviewAsync(StoreReviewDto storeReviewDto, CancellationToken cancellationToken);
        Task<int> AddProductReviewAsync(Review review, CancellationToken cancellationToken);
        Task<bool> DeleteReviewByIdAsync(int reviewId, CancellationToken cancellationToken);
    }
}
