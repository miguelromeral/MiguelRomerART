using MRA.DTO.Models;

namespace MRA.DTO.ViewModels.Art
{
    public class VoteSubmittedModel
    {
        public bool Success { get; set; }
        public double NewScore { get; set; }
        public int NewScoreHuman { get { return DrawingModel.CalculateScorePopular(NewScore); } }
        public int NewVotes { get; set; }
    }
}
