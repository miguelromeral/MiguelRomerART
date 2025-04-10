using MRA.DTO.Firebase.Models;
using MRA.DTO.Models;

namespace MRA.WebApi.Models.Responses
{
    public class CollectionResponse : CollectionModel
    {
        public List<string> DrawingsId { get; set; }
        public new List<DrawingModel> Drawings { get; set; }

        public CollectionResponse(CollectionModel collection)
        {
            this.Description = collection.Description;
            if (collection?.Drawings?.Count() > 0)
            {
                this.Drawings = collection.Drawings.ToList();
                this.DrawingsId = collection.Drawings.Select(x => x.Id).ToList();
            }
            else
            {
                this.Drawings = new List<DrawingModel>();
                this.DrawingsId = new List<string>();
            }
            this.Id = collection.Id;
            this.Name = collection.Name;
            this.Order = collection.Order;
        }
    }
}
