using MRA.DTO.Models;

namespace MRA.DTO.ViewModels.Art
{
    public class VoteSubmittedModel
    {
        public bool Success { get; private set; }
        public double NewScore { get; private set; }
        public int NewScoreHuman { get { return DrawingModel.CalculateScorePopular(NewScore); } }
        public int NewVotes { get; private set; }

        public VoteSubmittedModel(DrawingModel drawing, bool success)
        {
            NewVotes = drawing.VotesPopular;
            NewScore = drawing.ScorePopular;
            Success = success;
        }
    }
}
