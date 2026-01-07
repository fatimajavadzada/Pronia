using Microsoft.EntityFrameworkCore;
using Pronia.Models.Common;
using System.ComponentModel.DataAnnotations;

namespace Pronia.Models;

public class Slider : BaseEntity
{
    public string Title { get; set; }
    public string? Description { get; set; }
    public string ImageUrl { get; set; }
    [Range(0, 100)]
    public decimal DiscountPercentage { get; set; }
}
