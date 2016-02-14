using Common;
using Common.Turn;

namespace Player
{
    public interface IPlayerController
    {
        ITurn ChooseTurn(IPlayerState playerState, IPlayerState opponentState);
        Neuromon SelectActiveNeuromon(IPlayerState playerState, IPlayerState opponentState);
    }
}