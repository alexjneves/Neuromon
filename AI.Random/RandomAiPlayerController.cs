using System.Linq;
using Common;
using Common.Turn;

namespace Player.AI.Random
{
    internal sealed class RandomAiPlayerController : IPlayerController
    {
        private readonly System.Random _rand;

        public RandomAiPlayerController()
        {
            _rand = new System.Random();
        }

        public ITurn ChooseTurn(IPlayerState playerState, IPlayerState opponentState)
        {
            var move = _rand.Next(1, 5);
            var selectedMove = playerState.ActiveNeuromon.MoveSet[move];

            return new Attack(selectedMove);
        }

        public Neuromon SelectActiveNeuromon(IPlayerState playerState, IPlayerState opponentState)
        {
            var aliveNeuromon = playerState.NeuromonCollection.Where(n => !n.IsDead).ToList();

            var neuromonIndex = _rand.Next(0, aliveNeuromon.Count);
            return aliveNeuromon[neuromonIndex];
        }
    }
}
