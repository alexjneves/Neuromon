using System;
using System.Collections.Generic;
using Common;
using Common.Turn;

namespace Player.AI.Neat
{
    internal sealed class GameStateSerializer
    {
        private const int NumberOfPlayers = 2;
        private const int NumberOfMoves = 4;

        // Health + Type
        private const int NeuromonStateLength = 2;
        // Damage + Type
        private const int MoveLength = 2;

        private const int NeuromonDataLength = NeuromonStateLength + MoveLength * NumberOfMoves;

        private readonly int _numberOfInputs;
        private readonly int _numberOfOutputs;

        public GameStateSerializer(int numberOfNeuromon)
        {
            _numberOfInputs = numberOfNeuromon * NeuromonDataLength * NumberOfPlayers;
            _numberOfOutputs = NumberOfMoves + numberOfNeuromon - 1;
        }

        /**
        * Input Format:
        * [n]       = Neuromon Health
        * [n + 1]   = Neuromon Type
        * [n + 2]   = M1 Damage
        * [n + 3]   = M1 Type
        * [n + 4]   = M2 Damage
        * [n + 5]   = M2 Type
        * [n + 6]   = M3 Damage
        * [n + 7]   = M3 Type
        * [n + 8]   = M4 Damage
        * [n + 9]   = M4 Type
        *
        * Total Inputs = Number of Neuromon * Data per Neuromon * Number of Players = n * 10 * 2 = 20n
        */
        public double[] Serialize(IPlayerState playerState, IPlayerState opponentState)
        {
            var inputSignals = new double[_numberOfInputs];
            var currentIndex = 0;

            SerializePlayerState(playerState, inputSignals, ref currentIndex);
            SerializePlayerState(opponentState, inputSignals, ref currentIndex);

            // TODO: Remove
            if (currentIndex != _numberOfInputs)
            {
                throw new Exception("Incorrect Serialize Length");
            }

            return inputSignals;
        }

        private static void SerializePlayerState(IPlayerState playerState, IList<double> inputSignals, ref int currentIndex)
        {
            SerializeNeuromon(playerState.ActiveNeuromon, inputSignals, ref currentIndex);

            foreach (var neuromon in playerState.InactiveNeuromon)
            {
                SerializeNeuromon(neuromon, inputSignals, ref currentIndex);
            }
        }

        private static void SerializeNeuromon(Neuromon neuromon, IList<double> outputArray, ref int index)
        {
            var moves = neuromon.MoveSet.Moves;

            outputArray[index++] = neuromon.Health;
            outputArray[index++] = neuromon.Type.Id;

            for (var i = 0; i < NumberOfMoves; ++i)
            {
                outputArray[index++] = moves[i].Damage;
                outputArray[index++] = moves[i].Type.Id;
            }
        }

        /**
        * Output Format:
        * [0]       = Attack Move 1
        * [n]       = Attack Move n
        * [n + 1]   = Switch Neuromon 1
        * [n + m]   = Switch Neuromon m
        *
        * Total Outputs = Number of Moves + Number of Neuromon - 1 = n + 3
        */
        public IList<NeatAiTurnChoice> Deserialize(double[] output)
        {
            var neatAiTurnChoices = new List<NeatAiTurnChoice>(_numberOfOutputs);
            var index = 0;

            for (; index < NumberOfMoves; ++index)
            {
                neatAiTurnChoices.Add(new NeatAiTurnChoice(output[index], index, TurnType.Attack));
            }

            for (; index < _numberOfOutputs; ++index)
            {
                var relativeIndex = index - NumberOfMoves;
                neatAiTurnChoices.Add(new NeatAiTurnChoice(output[index], relativeIndex, TurnType.SwitchActiveNeuromon));
            }

            // TODO: Remove
            if (neatAiTurnChoices.Count != _numberOfOutputs)
            {
                throw new Exception("Incorrect Deserialize Length");
            }

            return neatAiTurnChoices;
        }
    }
}