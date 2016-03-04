namespace Player.AI.Neat
{
    internal sealed class NeatAiTurnChoice
    {
        public double Weight { get; }
        public int Index { get; }
        public TurnType TurnType { get; }

        public NeatAiTurnChoice(double weight, int index, TurnType turnType)
        {
            Weight = weight;
            Index = index;
            TurnType = turnType;
        }
    }
}