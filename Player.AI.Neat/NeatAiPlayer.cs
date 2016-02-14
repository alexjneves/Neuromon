using System;
using Common;
using Common.Turn;
using SharpNeat.Phenomes;

namespace Player.AI.Neat
{
    internal sealed class NeatAiPlayerController : IPlayerController
    {
        private readonly IBlackBox _brain;

        public NeatAiPlayerController(IBlackBox brain)
        {
            _brain = brain;
        }

        public ITurn ChooseTurn(IPlayerState playerState, IPlayerState opponentState)
        {
            throw new NotImplementedException();
        }

        public Neuromon SelectActiveNeuromon(IPlayerState playerState, IPlayerState opponentState)
        {
            throw new NotImplementedException();
        }
    }

}
