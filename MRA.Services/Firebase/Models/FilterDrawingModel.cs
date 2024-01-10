namespace MRA.Web.Models.Art
{
    public class FilterDrawingModel
    {
        public int Type { get; set; }
        public int ProductType { get; set; }
        public string ProductName { get; set; }
        public string ModelName { get; set; }
        public string CharacterName { get; set; }
        public string Collection { get; set; }
        public int Software { get; set; }
        public int Paper { get; set; }
        public string Sortby { get; set; }
        public string TextQuery { get; set; }
        public List<string> Tags { get { return (TextQuery ?? "").Split(" ").Select(x => x.ToLower()).ToList(); } }
        public bool Favorites { get; set; }
    }
}
