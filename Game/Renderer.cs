using System;
using System.Linq;
using System.Text;
using Common;
using Player;

namespace Game
{
    public sealed class Renderer
    {
        private const string MoveBoxTop = " -----------------------------------";
        private const string MoveBoxLeft = "| ";
        private const string MoveBoxRight = " |";
        private const string MoveBoxMiddle = " | ";

        private const string RenderPlayerBorder = "==============================================";
        private const string TurnMadeBorder = "***************************************************";

        private const ConsoleColor TurnMadeColour = ConsoleColor.DarkGreen;
        private const ConsoleColor Player1Colour = ConsoleColor.DarkCyan;
        private const ConsoleColor Player2Colour = ConsoleColor.DarkYellow;

        private readonly BattleSimulator _battleSimulator;
        private readonly int _moveRenderLength;

        public Renderer(BattleSimulator battleSimulator)
        {
            _battleSimulator = battleSimulator;

            _battleSimulator.OnAttackMade += RenderAttack;
            _battleSimulator.OnNeuromonChanged += RenderNeuromonChanged;
            _battleSimulator.OnGameOver += RenderGameOver;
            _battleSimulator.OnGameStateChanged += OnGameStateChanged;
            _battleSimulator.OnNeuromonDefeated += OnNeuromonDefeated;

            _moveRenderLength = CalculateMoveRenderLength();
        }

        private static void RenderAttack(Neuromon attacker, Move move, Neuromon target, int damage)
        {
            var sb = new StringBuilder();

            sb.AppendLine($"{attacker.Name} used {move.Name} and dealt {damage} damage!");

            if (move.Type.IsEffectiveAgainst(target.Type))
            {
                sb.AppendLine("It's super effective!");
            }
            else if (move.Type.IsWeakAgainst(target.Type))
            {
                sb.AppendLine("It wasn't very effective..");
            }

            RenderTurnMade(sb.ToString().Trim());
        }

        private static void RenderNeuromonChanged(IPlayerState playerState, Neuromon previousNeuromon, Neuromon newNeuromon)
        {
            var output = $"{playerState.Name} switched from {previousNeuromon.Name} to {newNeuromon.Name}!";
            RenderTurnMade(output);
        }

        private static void RenderTurnMade(string contents)
        {
            var sb = new StringBuilder();

            sb.AppendLine(TurnMadeBorder);
            sb.AppendLine(contents);
            sb.AppendLine(TurnMadeBorder);

            RenderTextWithColour(sb.ToString(), TurnMadeColour);
        }

        private void RenderGameOver(BattleResult battleResult)
        {
            RenderPlayerState(_battleSimulator.Player1.State);
            RenderPlayerState(_battleSimulator.Player2.State);

            var sb = new StringBuilder();

            sb.AppendLine(TurnMadeBorder);
            sb.AppendLine($"{battleResult.Loser.Name} has no remaining Neuromon...");
            sb.AppendLine($"{battleResult.Winner.Name} beat {battleResult.Loser.Name}!");
            sb.AppendLine(TurnMadeBorder);

            RenderTextWithColour(sb.ToString(), ConsoleColor.DarkGreen);
        }

        private void OnGameStateChanged(GameState previousState, GameState newState)
        {
            if (newState != GameState.GameOver)
            {
                RenderPlayerState(_battleSimulator.Player1.State);
                RenderPlayerState(_battleSimulator.Player2.State);
            }

            switch (newState)
            {
                case GameState.Player1Turn:
                    RenderChooseTurn(_battleSimulator.Player1.State);
                    break;
                case GameState.Player2Turn:
                    RenderChooseTurn(_battleSimulator.Player2.State);
                    break;
                default:
                    break;
            }
        }

        private void OnNeuromonDefeated(IPlayerState attackingPlayerState, Neuromon attacker, IPlayerState defendingPlayerState, Neuromon defeated)
        {
            Console.WriteLine($"{attackingPlayerState.Name}'s {attacker.Name} defeated {defendingPlayerState.Name}'s {defeated.Name}!\n");
            Console.WriteLine($"{defendingPlayerState.Name} must select a new active Neuromon:\n");
            RenderPlayerState(defendingPlayerState);
        }

        private void RenderPlayerState(IPlayerState playerState)
        {
            var output = FormatPlayerState(playerState);
            var colour = playerState == _battleSimulator.Player1.State ? Player1Colour : Player2Colour;

            RenderTextWithColour(output, colour);
        }

        private string FormatPlayerState(IPlayerState playerState)
        {
            var sb = new StringBuilder();

            sb.AppendLine(RenderPlayerBorder);
            sb.AppendLine($"Player: {playerState.Name}\n");

            sb.AppendLine($"Active Neuromon:");
            sb.AppendLine(FormatNeuromon(playerState.ActiveNeuromon));

            sb.AppendLine(RenderMoveSet(playerState.ActiveNeuromon.MoveSet));

            sb.AppendLine("Other Neuromon:");

            var formattedOtherNeuromon = playerState.NeuromonCollection.Where(n => n != playerState.ActiveNeuromon).Select(FormatNeuromon).ToList();

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

        private static void RenderChooseTurn(IPlayerState playerState)
        {
            Console.WriteLine($"{playerState.Name} select your move:\n");
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

        private static void RenderTextWithColour(string output, ConsoleColor colour)
        {
            Console.ForegroundColor = colour;
            Console.WriteLine(output);
            Console.ResetColor();
        }
    }
}