using Newtonsoft.Json;

namespace Game
{
    internal sealed class GameSettings
    {
        public int NumberOfNeuromon { get; }
        public double EffectiveMultiplier { get; }
        public double WeakMultiplier { get; }
        public double MinimumRandomMultiplier { get; }
        public double MaximumRandomMultiplier { get; }
        public string TypesFileName { get; }
        public string MovesFileName { get; }
        public string NeuromonFileName { get; }
        public string PlayerOneName { get; }
        public string PlayerTwoName { get; }
        public string PlayerOneType { get; }
        public string PlayerTwoType { get; }
        public bool SimulateThinking { get; }
        public bool ShouldRender { get; }
        public bool NonDeterministic { get; }

        [JsonConstructor]
        public GameSettings(int numberOfNeuromon, double effectiveMultiplier, double weakMultiplier, 
            double minimumRandomMultiplier, double maximumRandomMultiplier, string typesFileName, 
            string movesFileName, string neuromonFileName, string playerOneName, 
            string playerTwoName, string playerOneType, string playerTwoType, 
            bool simulateThinking, bool shouldRender, bool nonDeterministic)
        {
            NumberOfNeuromon = numberOfNeuromon;
            EffectiveMultiplier = effectiveMultiplier;
            WeakMultiplier = weakMultiplier;
            MinimumRandomMultiplier = minimumRandomMultiplier;
            MaximumRandomMultiplier = maximumRandomMultiplier;
            TypesFileName = typesFileName;
            MovesFileName = movesFileName;
            NeuromonFileName = neuromonFileName;
            PlayerOneName = playerOneName;
            PlayerTwoName = playerTwoName;
            PlayerOneType = playerOneType;
            PlayerTwoType = playerTwoType;
            SimulateThinking = simulateThinking;
            ShouldRender = shouldRender;
            NonDeterministic = nonDeterministic;
        }
    }
}