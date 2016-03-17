using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using Common.Turn;
using SharpNeat.Phenomes;

namespace Player.AI.Neat
{
    internal sealed class NeatAiPlayerController : IPlayerController
    {
        private readonly IBlackBox _brain;
        private readonly GameStateSerializer _gameStateSerializer;

        public NeatAiPlayerController(IBlackBox brain, GameStateSerializer gameStateSerializer)
        {
            _brain = brain;
            _gameStateSerializer = gameStateSerializer;
        }

        public ITurn ChooseTurn(IPlayerState playerState, IPlayerState opponentState)
        {
            _brain.ResetState();

            var serializedState = _gameStateSerializer.Serialize(playerState, opponentState);
            _brain.InputSignalArray.CopyFrom(serializedState, 0);

            _brain.Activate();

            var rawOutput = new double[_brain.OutputCount];
            _brain.OutputSignalArray.CopyTo(rawOutput, 0);

            var neatAiTurnChoices = _gameStateSerializer.Deserialize(rawOutput).ToList();

            var orderedChoices = neatAiTurnChoices.OrderByDescending(choice => choice.Weight);

            var validTurnTypes = DetermineValidTurnTypes(playerState).ToList();

            ITurn validTurn = null;
            foreach (var turnChoice in orderedChoices)
            {
                if (TryParseValidTurn(turnChoice, playerState, validTurnTypes, out validTurn))
                {
                    break;
                }
            }

            if (validTurn == null)
            {
                throw new Exception("Neat Ai Turn Choice is invalid");
            }

            return validTurn;
        }

        private static IEnumerable<TurnType> DetermineValidTurnTypes(IPlayerState playerState)
        {
            if (playerState.ActiveNeuromon.IsDead)
            {
                return new[] { TurnType.SwitchActiveNeuromon };
            }

            var validMoves = new List<TurnType> { TurnType.Attack };

            if (playerState.InactiveNeuromon.Any(n => !n.IsDead))
            {
                validMoves.Add(TurnType.SwitchActiveNeuromon);
            }

            return validMoves;
        }

        private static bool TryParseValidTurn(NeatAiTurnChoice turnChoice, IPlayerState playerState, IEnumerable<TurnType> validTurnTypes, out ITurn turn)
        {
            turn = null;

            if (!validTurnTypes.Contains(turnChoice.TurnType))
            {
                return false;
            }

            if (turnChoice.TurnType == TurnType.Attack)
            {
                var chosenMove = playerState.ActiveNeuromon.MoveSet.Moves.ElementAtOrDefault(turnChoice.Index);

                if (chosenMove != default(Move))
                {
                    turn = new Attack(chosenMove);
                    return true;
                }
            }
            else if (turnChoice.TurnType == TurnType.SwitchActiveNeuromon)
            {
                var chosenActiveNeuromon = playerState.InactiveNeuromon.ElementAtOrDefault(turnChoice.Index);

                if (chosenActiveNeuromon != default(Neuromon) && !chosenActiveNeuromon.IsDead)
                {
                    turn = new SwitchActiveNeuromon(chosenActiveNeuromon);
                    return true;
                }
            }

            return false;
        }

        public Neuromon SelectActiveNeuromon(IPlayerState playerState, IPlayerState opponentState)
        {
            var turn = ChooseTurn(playerState, opponentState) as SwitchActiveNeuromon;

            if (turn == null)
            {
                throw new Exception($"Chosen turn must be of type {typeof(Neuromon)}");
            }

            return turn.Neuromon;
        }
    }
}
