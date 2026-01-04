using Pronia.Models.Common;

namespace Pronia.Models;

public class Brand:BaseEntity
{
    public string Name { get; set; }
    public ICollection<ProductBrand> ProductBrands { get; set; } = [];
}
