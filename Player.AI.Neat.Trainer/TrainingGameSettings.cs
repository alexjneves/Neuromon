using Newtonsoft.Json;

namespace Player.AI.Neat.Trainer
{
    internal sealed class TrainingGameSettings
    {
        public int NumberOfNeuromon { get; }
        public double EffectiveMultiplier { get; }
        public double WeakMultiplier { get; }
        public double MinimumRandomMultiplier { get; }
        public double MaximumRandomMultiplier { get; }
        public string TypesFileName { get; }
        public string MovesFileName { get; }
        public string NeuromonFileName { get; }
        public string OpponentType { get; }
        public string OpponentBrainFileName { get; }
        public bool ShouldRender { get; }
        public bool NonDeterministic { get; }

        [JsonConstructor]
        public TrainingGameSettings(int numberOfNeuromon, double effectiveMultiplier, double weakMultiplier,
            double minimumRandomMultiplier, double maximumRandomMultiplier, string typesFileName,
            string movesFileName, string neuromonFileName, string opponentType,
            string opponentBrainFileName, bool shouldRender, bool nonDeterministic)
        {
            NumberOfNeuromon = numberOfNeuromon;
            EffectiveMultiplier = effectiveMultiplier;
            WeakMultiplier = weakMultiplier;
            MinimumRandomMultiplier = minimumRandomMultiplier;
            MaximumRandomMultiplier = maximumRandomMultiplier;
            TypesFileName = typesFileName;
            MovesFileName = movesFileName;
            NeuromonFileName = neuromonFileName;
            OpponentType = opponentType;
            OpponentBrainFileName = opponentBrainFileName;
            ShouldRender = shouldRender;
            NonDeterministic = nonDeterministic;
        }
    }
}