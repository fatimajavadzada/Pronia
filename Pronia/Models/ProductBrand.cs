using Pronia.Models.Common;

namespace Pronia.Models;

public class ProductBrand : BaseEntity
{
    public int BrandId { get; set; }
    public Brand Brand { get; set; }
    public int ProductId { get; set; }
    public Product Product { get; set; }
}
