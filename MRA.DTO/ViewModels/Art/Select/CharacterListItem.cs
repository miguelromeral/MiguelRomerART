using Google.Api.Gax.ResourceNames;
using MRA.DTO.Enums.Drawing;
using MRA.DTO.Models;

namespace MRA.DTO.ViewModels.Art.Select
{
    public class CharacterListItem
    {
        public string CharacterName { get; set; }
        public int ProductTypeId { get => (int)ProductType; }
        public DrawingProductTypes ProductType { get; set; }

        public CharacterListItem(string characterName, DrawingProductTypes productType)
        {
            CharacterName = characterName;
            ProductType = productType;
        }

        public static IEnumerable<CharacterListItem> GetCharactersFromDrawings(IEnumerable<DrawingModel> drawings)
        {
            return drawings
                .Where(x => !string.IsNullOrEmpty(x.Name))
                .Select(x => new CharacterListItem(x.Name, x.ProductType))
                .Distinct();
        }

        public override bool Equals(object obj)
        {
            return GetHashCode() == obj.GetHashCode();
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(CharacterName);
        }
    }
}
