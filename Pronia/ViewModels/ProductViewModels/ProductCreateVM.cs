using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Pronia.ViewModels.ProductViewModels
{
    public class ProductCreateVM
    {
        [Required]
        public string Name { get; set; }
        public string? Description { get; set; }
        [Required]
        [Precision(10, 2)]
        public decimal Price { get; set; }
        [Required]
        public string? SKU { get; set; }
        [Required]
        [Range(1, 5)]
        public int Rating { get; set; }
        [Required]
        public int CategoryId { get; set; }
        public List<int> TagIds { get; set; }
        public List<int> BrandIds { get; set; }
        public IFormFile MainImage { get; set; }
        [Required]
        public IFormFile HoverImage { get; set; }
        public List<IFormFile> Images { get; set; } = [];
    }
}
