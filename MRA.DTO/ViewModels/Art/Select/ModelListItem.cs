using MRA.DTO.Enums.Drawing;
using MRA.DTO.Models;

namespace MRA.DTO.ViewModels.Art.Select;

public class ModelListItem
{
    public string ModelName { get; set; }

    public ModelListItem(string modelName)
    {
        ModelName = modelName;
    }

    public static IEnumerable<ModelListItem> GetModelsFromDrawings(IEnumerable<DrawingModel> drawings)
    {
        return drawings
            .Where(x => !string.IsNullOrEmpty(x.ModelName))
            .Select(x => new ModelListItem(x.ModelName))
            .Distinct();
    }
}
