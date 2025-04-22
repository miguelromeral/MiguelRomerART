using MRA.DTO.Models;

namespace MRA.Services.Models.Drawings;

public static class DrawingVoteManager
{
    public static void UpdateDrawingScore(this DrawingModel drawing, int newScore)
    {
        newScore = newScore.AdjustUserScore();
        if (drawing.VotesPopular > 0)
        {
            drawing.ScorePopular = CalculateUserScore(newScore, drawing.ScorePopular, drawing.VotesPopular);
            drawing.VotesPopular = drawing.VotesPopular + 1;
        }
        else
        {
            drawing.ScorePopular = newScore;
            drawing.VotesPopular = 1;
        }
    }

    public static double CalculateUserScore(int newScore, double scorePopular, int currentVotes)
    {
        return ((scorePopular * currentVotes) + newScore.AdjustUserScore()) / (currentVotes + 1);
    }

    private static int AdjustUserScore(this int score)
    {
        if (score > 100) score = 100;
        if (score < 0) score = 0;
        return score;
    }
}
