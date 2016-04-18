namespace Trainer
{
    public sealed class TrainingGameSettings
    {
        public int NumberOfNeuromon { get; set; }
        public double EffectiveMultiplier { get; set; }
        public double WeakMultiplier { get; set; }
        public double MinimumRandomMultiplier { get; set; }
        public double MaximumRandomMultiplier { get; set; }
        public string TypesFileName { get; set; }
        public string MovesFileName { get; set; }
        public string NeuromonFileName { get; set; }
        public string OpponentType { get; set; }
        public string OpponentBrainFileName { get; set; }
        public bool ShouldRender { get; set; }
        public bool NonDeterministic { get; set; }
    }
}