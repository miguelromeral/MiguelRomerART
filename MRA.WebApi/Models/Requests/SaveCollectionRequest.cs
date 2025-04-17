using MRA.DTO.Models;

namespace MRA.WebApi.Models.Requests
{
    public class SaveCollectionRequest
    {
        public string Id { get; set; }
        public string Description { get; set; }
        public string Name { get; set; }
        public int Order{ get; set; }
        public string[] DrawingsIds { get; set; }

        
        public CollectionModel GetModel()
        {
            return  new CollectionModel()
            {
                Id = Id,
                Description = Description,
                Name = Name,
                Order = Order,
                DrawingIds = DrawingsIds,
                Drawings = []
            };
        }
    }
}
