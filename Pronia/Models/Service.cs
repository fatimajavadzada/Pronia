using System.ComponentModel.DataAnnotations;

namespace Pronia.Models;

public class Service
{
    public int Id { get; set; }
    [Required]
    [MaxLength(100)]
    public string Title { get; set; }
    public string? Description { get; set; }
    [MaxLength(512), MinLength(3)]
    [Required]
    public string ImageUrl { get; set; }
}
