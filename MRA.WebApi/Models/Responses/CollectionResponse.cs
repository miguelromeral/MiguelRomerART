using MRA.DTO.Models;

namespace MRA.WebApi.Models.Responses
{
    public class CollectionResponse : CollectionModel
    {
        // TODO: corregir errata, llamarlo DrawingIds en Front y eliminarlo de Back (ya lo tiene CollectionModel)
        public List<string> DrawingsId { get; set; }
        public new List<DrawingModel> Drawings { get; set; }

        public CollectionResponse(CollectionModel collection)
        {
            this.Description = collection.Description;
            if (collection?.Drawings?.Count() > 0)
            {
                var drawingIds = collection.DrawingIds.ToList();

                this.Drawings = collection.Drawings
                    .OrderBy(d => drawingIds.IndexOf(d.Id))
                    .ToList();
                this.DrawingsId = drawingIds;
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
