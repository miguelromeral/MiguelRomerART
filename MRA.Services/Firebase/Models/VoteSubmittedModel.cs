using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MRA.Services.Firebase.Models
{
    public class VoteSubmittedModel
    {
        public double NewScore { get; set; }
        public int NewScoreHuman { get { return Drawing.CalculateScorePopular(NewScore); } }
        public int NewVotes { get; set; }
    }
}
