using MRA.Services.Firebase.Models;

namespace MRA.WebApi.Models.Responses
{
    public class CollectionResponse : Collection
    {
        public List<string> DrawingsId { get; set; }
        public new List<Drawing> Drawings { get; set; }

        public CollectionResponse(Collection collection)
        {
            this.Description = collection.Description;
            if (collection?.Drawings.Count > 0)
            {
                this.Drawings = collection.Drawings;
                this.DrawingsId = collection.Drawings.Select(x => x.Id).ToList();
            }
            else
            {
                this.Drawings = new List<Drawing>();
                this.DrawingsId = new List<string>();
            }
            this.Id = collection.Id;
            this.Name = collection.Name;
            this.Order = collection.Order;
        }
    }
}
