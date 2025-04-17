using MRA.DTO.Models;

namespace MRA.WebApi.Models.Responses;

public class CollectionResponse : CollectionModel
{
    public new IEnumerable<DrawingModel> Drawings { get; set; }

    public CollectionResponse(CollectionModel collection)
    {
        this.Description = collection.Description;
        if (collection.Drawings.Any())
        {
            var drawingIds = collection.DrawingIds.ToList();

            this.Drawings = collection.Drawings
                .OrderBy(d => drawingIds.IndexOf(d.Id));
        }
        else
        {
            this.Drawings = new List<DrawingModel>();
        }
        if (collection is not null)
        {
            this.Id = collection.Id;
            this.Name = collection.Name;
            this.Order = collection.Order;
            this.DrawingIds = collection.DrawingIds;
        }
    }
}
