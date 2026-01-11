using Microsoft.EntityFrameworkCore;
using Pronia.Models.Common;
using System.ComponentModel.DataAnnotations;

namespace Pronia.Models;

public class Product : BaseEntity
{
    public string Name { get; set; }
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public string MainImagePath { get; set; }
    public string HoverImagePath { get; set; }
    public string? SKU { get; set; }
    public int CategoryId { get; set; }
    public Category Category { get; set; }
    public int Rating { get; set; }
    public ICollection<ProductTag> ProductTags { get; set; } = [];
    public ICollection<ProductBrand> ProductBrands { get; set; } = [];
    public ICollection<ProductImage> ProductImages { get; set; } = [];
    public ICollection<BasketItem> BasketItems { get; set; } = [];

}

