using Microsoft.EntityFrameworkCore;
using Pronia.Abstraction;
using Pronia.Contexts;
using System.Security.Claims;

namespace Pronia.Services
{
    public class BasketService(IHttpContextAccessor _accessor, AppDbContext _context) : IBasketService
    {
        public async Task<List<BasketItem>> GetBasketItemsAsync()
        {
            var usedId = _accessor.HttpContext!.User.FindFirstValue(ClaimTypes.NameIdentifier);

            var isExistUser = await _context.Users.AnyAsync(u => u.Id == usedId);

            if (!isExistUser)
            {
                return [];
            }

            var basketItems = await _context.BasketItems
                .Include(b => b.Product)
                .Where(b => b.AppUserId == usedId)
                .ToListAsync();

            return basketItems;
        }
    }
}
