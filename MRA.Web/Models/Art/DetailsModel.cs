using MRA.Services.Firebase.Models;

namespace MRA.Web.Models.Art
{
    public class DetailsModel
    {
        public readonly string QueryId;
        public Drawing Drawing;
        public bool IsAdmin;

        public DetailsModel(string id, bool isAdmin)
        {
            QueryId = id;
            IsAdmin = isAdmin;
        }

        public static string GetClassScore(int score)
        {
            if(score == 0)
            {
                return "";
            }else if (score < 50)
            {
                return "mr-score-bad";
            }
            else if (score < 65)
            {
                return "mr-score-mild";
            }
            else if (score < 95)
            {
                return "mr-score-good";
            }
            else if (score < 101)
            {
                return "mr-score-platinum";
            }
            return "";
        }
    }
}
