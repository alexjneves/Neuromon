namespace Game.Damage
{
    internal sealed class DamageCalculatorFactory
    {
        private readonly double _effectiveMultipler;
        private readonly double _weakMultiplier;
        private readonly double _minRandomMultiplier;
        private readonly double _maxRandomMultiplier;
        private readonly bool _nonDeterministic;

        public DamageCalculatorFactory(double effectiveMultipler, double weakMultiplier, double minRandomMultiplier,
            double maxRandomMultiplier, bool nonDeterministic)
        {
            _effectiveMultipler = effectiveMultipler;
            _weakMultiplier = weakMultiplier;
            _minRandomMultiplier = minRandomMultiplier;
            _maxRandomMultiplier = maxRandomMultiplier;
            _nonDeterministic = nonDeterministic;
        }

        public IDamageCalculator Create()
        {
            if (_nonDeterministic)
            {
                return new NonDeterministicDamageCalculator(_effectiveMultipler, _weakMultiplier, _minRandomMultiplier, _maxRandomMultiplier);
            }

            return new DamageCalculator(_effectiveMultipler, _weakMultiplier);
        }
    }
}