using MRA.Services.Firebase.Models;

namespace MRA.Web.Models.Art
{
    public class DetailsModel
    {
        public readonly string QueryId;
        public Drawing Drawing;

        public DetailsModel(string id)
        {
            QueryId = id;
        }
    }
}
