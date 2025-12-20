using AI_Marketplace.Application.Common.Interfaces;
using AI_Marketplace.Application.Reviews.DTOs;
using AI_Marketplace.Domain.Entities;
using AI_Marketplace.Infrastructure.Data;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AI_Marketplace.Infrastructure.Repositories.Reviews
{
    public class ReviewsRepository : IReviewsRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public ReviewsRepository(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<ReviewDto>> GetAllProductReviewsAsync(int productId, CancellationToken cancellationToken)
        {
            var reviews = await _context.Reviews
                .AsNoTracking()
                .Include(r => r.User)
                .Include(r => r.Product)
                .Where(r => r.ProductId == productId)
                .ToListAsync(cancellationToken);

            return _mapper.Map<List<ReviewDto>>(reviews);
        }

        public async Task<List<ReviewDto>> GetAllStoreReviewsAsync(int storeId, CancellationToken cancellationToken)
        {
            var reviews = await _context.Reviews
                .AsNoTracking()
                .Include(r => r.User)
                .Include(r => r.Store)
                .Where(r => r.StoreId == storeId)
                .ToListAsync(cancellationToken);

            return _mapper.Map<List<ReviewDto>>(reviews);
        }

        public async Task<int> AddStoreReviewAsync(StoreReviewDto storeReviewDto, CancellationToken cancellationToken)
        {
            var review = _mapper.Map<Review>(storeReviewDto);
            review.StoreId = storeReviewDto.StoreId;
            _context.Reviews.Add(review);
            await _context.SaveChangesAsync(cancellationToken);
            return review.Id;
        }

        public async Task<int> AddProductReviewAsync(Review review, CancellationToken cancellationToken)
        {
            await _context.Reviews.AddAsync(review, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return review.Id;
        }

        public async Task<bool> DeleteReviewByIdAsync(int reviewId, CancellationToken cancellationToken)
        {
            var review = await _context.Reviews.FirstOrDefaultAsync(r => r.Id == reviewId, cancellationToken);
            if (review == null)
                return false;

            _context.Reviews.Remove(review);
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
