using AI_Marketplace.Application.Common.Interfaces;
using AI_Marketplace.Application.Wishlists.DTOs;
using AutoMapper;
using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AI_Marketplace.Application.Wishlists.Queries
{
    public class GetWishlistByUserIdQueryHandler : IRequestHandler<GetWishlistByUserIdQuery, WishlistDto>
    {
        private readonly IWishlistRepository _wishlistRepository;
        private readonly IMapper _mapper;

        public GetWishlistByUserIdQueryHandler(IWishlistRepository wishlistRepository, IMapper mapper)
        {
            _wishlistRepository = wishlistRepository;
            _mapper = mapper;
        }

        public async Task<WishlistDto> Handle(GetWishlistByUserIdQuery request, CancellationToken cancellationToken)
        {
            var wishlistItems = await _wishlistRepository.GetWishlistByUserIdAsync(request.UserId, cancellationToken);

            var wishlistDto = new WishlistDto
            {
                UserId = request.UserId,
                TotalItems = wishlistItems.Count,
                Items = _mapper.Map<System.Collections.Generic.List<WishlistItemDto>>(wishlistItems)
            };

            return wishlistDto;
        }
    }
}
