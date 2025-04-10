
namespace MRA.DTO.Models;

public class InspirationModel : IModel
{

    public static Dictionary<int, string> INSPIRATION_TYPES = new Dictionary<int, string>()
        {
            {0, "Otros"},
            {1, "Profesionales"},
            {2, "Aficionados"},
            {3, "Modelos"},
        };
    public string Id { get; set; }
    public string Name { get; set; }
    public string Instagram { get; set; }
    public string Twitter { get; set; }
    public string YouTube { get; set; }
    public string Twitch { get; set; }
    public string Pinterest { get; set; }
    public int Type { get; set; }
    public string TypeName
    {
        get
        {
            if (INSPIRATION_TYPES.ContainsKey(Type))
            {

            return INSPIRATION_TYPES[Type];
            }
            return "Otros";
            
        }
    }
}
