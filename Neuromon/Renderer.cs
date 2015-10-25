using System;
using System.Text;

namespace Neuromon
{
    internal sealed class Renderer
    {
        private const string MoveBoxTop = " -----------------------------";
        private const string MoveBoxLeft = "| ";
        private const string MoveBoxRight = " |";
        private const string MoveBoxMiddle = " | ";

        private readonly BattleSimulator _battleSimulator;
        private readonly int _moveRenderLength;

        public Renderer(BattleSimulator battleSimulator)
        {
            _battleSimulator = battleSimulator;

            _battleSimulator.OnTurnChosen += RenderTurn;
            _battleSimulator.OnGameOver += RenderGameOver;
            _battleSimulator.OnGameStateChanged += OnGameStateChanged;

            _moveRenderLength = CalculateMoveRenderLength();
        }

        private static void RenderTurn(IPlayer player, Turn turn)
        {
            var output = $"{player.Name} used {turn.Move.Name} and dealt {turn.Move.Damage} damage!";
            Console.WriteLine(output);
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
            var sb = new StringBuilder("\n");

            sb.AppendLine("======================");
            sb.AppendLine($"Player: {_battleSimulator.Player1.Name}");
            sb.AppendLine(FormatNeuromon(_battleSimulator.Player1.Neuromon));
            sb.AppendLine("======================");

            sb.AppendLine("======================");
            sb.AppendLine($"Player: {_battleSimulator.Player2.Name}");
            sb.AppendLine(FormatNeuromon(_battleSimulator.Player2.Neuromon));
            sb.AppendLine("======================");

            Console.WriteLine(sb.ToString());
        }

        private static string FormatNeuromon(Neuromon neuromon)
        {
            var sb = new StringBuilder();

            sb.AppendLine($"Neuromon: {neuromon.Name}");
            sb.Append($"Health: {neuromon.Health}");

            return sb.ToString();
        }

        private void RenderChooseTurn(IPlayer player)
        {
            var sb = new StringBuilder();

            sb.AppendLine($"{player.Name} select your move:");

            var moveSet = player.Neuromon.MoveSet;

            sb.AppendLine(MoveBoxTop);

            FormatMoves(sb, moveSet.MoveOne(), moveSet.MoveTwo());
            FormatMoves(sb, moveSet.MoveThree(), moveSet.MoveFour());

            sb.AppendLine(MoveBoxTop);

            Console.WriteLine(sb);
        }

        private void FormatMoves(StringBuilder sb, Move first, Move second)
        {
            sb.Append(MoveBoxLeft);
            FormatMove(sb, first);
            sb.Append(MoveBoxMiddle);
            FormatMove(sb, second);
            sb.AppendLine(MoveBoxRight);
        }

        private void FormatMove(StringBuilder sb, Move move)
        {
            sb.Append(move.Name);

            var padding = _moveRenderLength - move.Name.Length;

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