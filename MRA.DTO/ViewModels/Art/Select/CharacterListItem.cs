using MRA.DTO.Enums.Drawing;

namespace MRA.DTO.ViewModels.Art.Select
{
    public class CharacterListItem
    {
        public string CharacterName { get; set; }
        public DrawingProductTypes ProductTypeId { get; set; }
        public string ProductType { get; set; }
    }
}
