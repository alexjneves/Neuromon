using System.Linq;

namespace Game
{
    public sealed class ScoreCalculator
    {
        // Maximum Fitness = Win + N * (50 + H) + N * 50
        public double Calculate(string playerName, BattleResult result)
        {
            var score = 0.0;

            var players = new[] { result.Winner, result.Loser };
            var playerState = players.Single(p => p.Name == playerName);
            var opponentState = players.Single(p => p.Name != playerName);

            if (result.Winner == playerState)
            {
                score += 200;
            }

            score += playerState.AllNeuromon.Where(n => !n.IsDead).Sum(n => 50.0 + n.Health);

            score += opponentState.AllNeuromon.Where(n => n.IsDead).Sum(n => 50.0);
            score -= opponentState.AllNeuromon.Where(n => !n.IsDead).Sum(n => n.Health);

            return score >= 0.0 ? score : 0.0;
        }
        
        /*
        *   win += 500
        *   score += total health of neuromon
        *
        */
    }
}