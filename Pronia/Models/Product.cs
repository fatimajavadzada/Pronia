using Microsoft.EntityFrameworkCore;
using Pronia.Models.Common;
using System.ComponentModel.DataAnnotations;

namespace Pronia.Models;

public class Product : BaseEntity
{
    [Required]
    public string Name { get; set; }
    public string? Description { get; set; }
    [Required]
    [Precision(10,2)]
    public decimal Price { get; set; }
    [Required]
    public string MainImagePath { get; set; }
    [Required]
    public string HoverImagePath { get; set; }
    public string? SKU { get; set; }
    [Required]
    public int CategoryId { get; set; }
    public Category Category { get; set; }
}

