namespace MRA.Web.Models.Art
{
    public class FilterDrawingModel
    {
        public int Type { get; set; }
        public int ProductType { get; set; }
        public string ProductName { get; set; }
        public int Software { get; set; }
        public int Paper { get; set; }
        public string Sortby { get; set; }

        public string Textquery { get; set; }
        public bool Favorites { get; set; }
    }
}
