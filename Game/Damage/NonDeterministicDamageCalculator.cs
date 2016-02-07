using System;
using Common;

namespace Game.Damage
{
    internal sealed class NonDeterministicDamageCalculator : DamageCalculator
    {
        private readonly int _minimumRandomMultipler;
        private readonly int _maxRandomMultipler;
        private readonly Random _rand;

        public NonDeterministicDamageCalculator(double effectiveMultipler, double weakMultiplier,
            double minRandomMultipler, double maxRandomMultipler) : base(effectiveMultipler, weakMultiplier)
        {
            _minimumRandomMultipler = (int) (minRandomMultipler * 100.0);
            _maxRandomMultipler = (int) (maxRandomMultipler * 100.0);
            _rand = new Random();
        }

        public override int CalculateDamage(Move attack, Neuromon target)
        {
            var damage = base.CalculateDamage(attack, target);
            var randomMultipler = _rand.Next(_minimumRandomMultipler, _maxRandomMultipler + 1) * 1.0 / 100.0;

            return (int) Math.Floor(damage * randomMultipler);
        }
    }
}