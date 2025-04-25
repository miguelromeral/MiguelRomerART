using MRA.DTO.Enums.Drawing;
using MRA.DTO.Models;
using MRA.Infrastructure.Enums;

namespace MRA.DTO.ViewModels.Art.Select;

public class ProductListItem
{
    public string ProductName { get; set; }
    public int ProductTypeId { get => (int)ProductType; }
    public DrawingProductTypes ProductType { get; set; }

    public ProductListItem(string productName, DrawingProductTypes productType)
    {
        ProductName = productName;
        ProductType = productType;
    }

    public static IEnumerable<ProductListItem> GetProductsFromDrawings(IEnumerable<DrawingModel> drawings)
    {
        return drawings
            .Where(x => !string.IsNullOrEmpty(x.ProductName))
            .Select(x => new ProductListItem(x.ProductName, x.ProductType))
            .Distinct();
    }

    public override bool Equals(object obj)
    {
        return GetHashCode() == obj.GetHashCode();
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(ProductName);
    }
}
