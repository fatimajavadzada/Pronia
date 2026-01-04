using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Pronia.ViewModels.ProductViewModels
{
    public class ProductUpdateVM
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string? Description { get; set; }
        [Required]
        [Precision(10, 2)]
        public decimal Price { get; set; }
        public string? SKU { get; set; }
        [Required]
        public int CategoryId { get; set; }
        public List<int> TagIds { get; set; }
        public List<int> BrandIds { get; set; }
        [Range(1, 5)]
        public int Rating { get; set; }
        public IFormFile? MainImage { get; set; }
        public IFormFile? HoverImage { get; set; }
        public List<IFormFile>? Images { get; set; } = [];
        public string? MainImagePath { get; set; }
        public string? HoverImagePath { get; set; }
        public List<string>? ImageUrls { get; set; } = [];
        public List<int>? ImageIds { get; set; } = [];
    }
}
