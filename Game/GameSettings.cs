using Newtonsoft.Json;

namespace Game
{
    internal sealed class GameSettings
    {
        public int NumberOfNeuromon { get; }
        public int InputNeuronCount { get; }
        public int OutputNeuronCount { get; }
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
        public string PlayerOneBrain { get; }
        public string PlayerTwoBrain { get; }
        public bool SimulateThinking { get; }
        public bool ShouldRender { get; }
        public bool NonDeterministic { get; }

        [JsonConstructor]
        public GameSettings(int numberOfNeuromon, int inputNeuronCount, int outputNeuronCount, 
            double effectiveMultiplier, double weakMultiplier, double minimumRandomMultiplier, 
            double maximumRandomMultiplier, string typesFileName, string movesFileName, 
            string neuromonFileName, string playerOneName, string playerTwoName, 
            string playerOneType, string playerTwoType, string playerOneBrain,
            string playerTwoBrain, bool simulateThinking, bool shouldRender, 
            bool nonDeterministic)
        {
            NumberOfNeuromon = numberOfNeuromon;
            InputNeuronCount = inputNeuronCount;
            OutputNeuronCount = outputNeuronCount;
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
            PlayerOneBrain = playerOneBrain;
            PlayerTwoBrain = playerTwoBrain;
            SimulateThinking = simulateThinking;
            ShouldRender = shouldRender;
            NonDeterministic = nonDeterministic;
            InputNeuronCount = inputNeuronCount;
            OutputNeuronCount = outputNeuronCount;
        }
    }
}