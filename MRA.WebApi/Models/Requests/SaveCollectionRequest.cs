namespace MRA.WebApi.Models.Requests
{
    public class SaveCollectionRequest
    {
        public bool IsEditing { get; set; }
        public string Id { get; set; }
        public string Description { get; set; }
        public string Name { get; set; }
        public int Order{ get; set; }
        public string[] DrawingsIds { get; set; }
    }
}
