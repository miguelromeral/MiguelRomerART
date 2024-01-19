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

        public string GetClassScore(int score)
        {
            if(score == 0)
            {
                return "";
            }else if (score < 50)
            {
                return "mr-score-bad";
            }
            else if (score < 70)
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
