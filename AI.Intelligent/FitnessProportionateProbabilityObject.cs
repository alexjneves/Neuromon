namespace AI.Intelligent
{
    internal sealed class FitnessProportionateProbabilityObject<T>
    {
        public T Item { get; }
        public double Probability { get; }

        public FitnessProportionateProbabilityObject(T item, double fitnessProportionateProbability)
        {
            Item = item;
            Probability = fitnessProportionateProbability;
        }
    }
}
