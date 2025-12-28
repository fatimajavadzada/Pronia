using Microsoft.AspNetCore.Mvc;
using Pronia.Contexts;
using Pronia.ViewModels.ProductViewModels;

namespace Pronia.Controllers
{
    public class ShopController(AppDbContext _context) : Controller
    {
        public IActionResult Index()
        {
            var products = _context.Products.Select(product => new ProductGetVM()
            {
                Id = product.Id,
                Name = product.Name,
                Price = product.Price,
                SKU = product.SKU,
                Rating = product.Rating,
                Description = product.Description,
                CategoryName = product.Category.Name,
                MainImageUrl = product.MainImagePath,
                HoverImageUrl = product.HoverImagePath
            }).ToList();
            return View(products);
        }
    }
}
