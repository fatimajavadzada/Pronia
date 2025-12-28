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
        public IFormFile? MainImage { get; set; }
        public IFormFile? HoverImage { get; set; }
        public string? SKU { get; set; }
        [Required]
        public int CategoryId { get; set; }
        [Range(1, 5)]
        public int Rating { get; set; }
    }
}
