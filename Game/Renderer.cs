using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common;
using Common.Turn;
using Player;

namespace Game
{
    internal sealed class Renderer
    {
        private const string MoveBoxTop = " -----------------------------------";
        private const string MoveBoxLeft = "| ";
        private const string MoveBoxRight = " |";
        private const string MoveBoxMiddle = " | ";

        private const string RenderPlayerBorder = "==============================================";

        private const string TurnMadeBorder = "***************************************************";
        private const ConsoleColor TurnMadeColour = ConsoleColor.DarkRed;

        private readonly BattleSimulator _battleSimulator;
        private readonly int _moveRenderLength;

        public Renderer(BattleSimulator battleSimulator)
        {
            _battleSimulator = battleSimulator;

            _battleSimulator.OnAttackMade += RenderAttack;
            _battleSimulator.OnNeuromonChanged += RenderNeuromonChanged;
            _battleSimulator.OnGameOver += RenderGameOver;
            _battleSimulator.OnGameStateChanged += OnGameStateChanged;

            _moveRenderLength = CalculateMoveRenderLength();
        }

        private static void RenderAttack(Neuromon attacker, Move move, Neuromon target, int damage)
        {
            var output = $"{attacker.Name} used {move.Name} and dealt {damage} damage!";
            RenderTurnMade(output);
        }

        private static void RenderNeuromonChanged(IPlayer player, Neuromon previousNeuromon, Neuromon newNeuromon)
        {
            var output = $"{player.Name} switched from {previousNeuromon.Name} to {newNeuromon.Name}!";
            RenderTurnMade(output);
        }

        private static void RenderTurnMade(string contents)
        {
            Console.ForegroundColor = TurnMadeColour;

            Console.WriteLine(TurnMadeBorder);
            Console.WriteLine(contents);
            Console.WriteLine(TurnMadeBorder);
        }

        private static void RenderGameOver(IPlayer winner, IPlayer loser)
        {
            var output = $"{winner.Name} beat {loser.Name}!";
            Console.WriteLine(output);
        }

        private void OnGameStateChanged(GameState previousState, GameState newState)
        {
            if (previousState == GameState.Player1Turn || previousState == GameState.Player2Turn)
            {
                RenderPlayerState();
            }

            switch (newState)
            {
                case GameState.Player1Turn:
                    RenderChooseTurn(_battleSimulator.Player1);
                    break;
                case GameState.Player2Turn:
                    RenderChooseTurn(_battleSimulator.Player2);
                    break;
                default:
                    break;
            }
        }

        private void RenderPlayerState()
        {
            Console.WriteLine();

            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.WriteLine(RenderPlayer(_battleSimulator.Player1));

            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine(RenderPlayer(_battleSimulator.Player2));

            Console.ResetColor();

            Console.WriteLine();
        }

        private string RenderPlayer(IPlayer player)
        {
            var sb = new StringBuilder();

            sb.AppendLine(RenderPlayerBorder);
            sb.AppendLine($"Player: {player.Name}\n");

            sb.AppendLine($"Active Neuromon:");
            sb.AppendLine(FormatNeuromon(player.ActiveNeuromon));

            sb.AppendLine(RenderMoveSet(player.ActiveNeuromon.MoveSet));

            sb.AppendLine("Other Neuromon:");

            var formattedOtherNeuromon = player.Neuromon.Where(n => n != player.ActiveNeuromon).Select(FormatNeuromon).ToList();

            for (var i = 0; i < formattedOtherNeuromon.Count; ++i)
            {
                sb.AppendLine($"{i + 1}: {formattedOtherNeuromon[i]}");
            }

            sb.AppendLine(RenderPlayerBorder);

            return sb.ToString();
        }

        private static string FormatNeuromon(Neuromon neuromon)
        {
            return $"Name: {neuromon.Name} | Type: {neuromon.Type.Name} | Health: {neuromon.Health}";
        }

        private static void RenderChooseTurn(IPlayer player)
        {
            Console.WriteLine($"{player.Name} select your move:\n");
        }

        private string RenderMoveSet(MoveSet moveSet)
        {
            var sb = new StringBuilder();

            sb.AppendLine(MoveBoxTop);

            FormatMoves(sb, moveSet.MoveOne(), moveSet.MoveTwo(), 1);
            FormatMoves(sb, moveSet.MoveThree(), moveSet.MoveFour(), 3);

            sb.AppendLine(MoveBoxTop);

            return sb.ToString();
        }

        private void FormatMoves(StringBuilder sb, Move first, Move second, int moveNumber)
        {
            sb.Append(MoveBoxLeft);
            FormatMove(sb, first, moveNumber);
            sb.Append(MoveBoxMiddle);
            FormatMove(sb, second, moveNumber + 1);
            sb.AppendLine(MoveBoxRight);
        }

        private void FormatMove(StringBuilder sb, Move move, int moveNumber)
        {
            var moveRender = $"{moveNumber}. {move.Name}";

            sb.Append(moveRender);

            var padding = _moveRenderLength - moveRender.Length;

            if (padding > 0)
            {
                for (var i = 0; i < padding; ++i)
                {
                    sb.Append(" ");
                }
            }
        }

        private static int CalculateMoveRenderLength()
        {
            var totalLength = MoveBoxTop.Length;
            var singleLength = totalLength / 2;

            return singleLength - MoveBoxMiddle.Length;
        }
    }
}