using System;
using Common;

namespace Game.Damage
{
    internal class DamageCalculator : IDamageCalculator
    {
        private readonly double _effectiveMultipler;
        private readonly double _weakMultiplier;

        public DamageCalculator(double effectiveMultipler, double weakMultiplier)
        {
            _effectiveMultipler = effectiveMultipler;
            _weakMultiplier = weakMultiplier;
        }

        public virtual int CalculateDamage(Move attack, Neuromon target)
        {
            var damage = attack.Damage * 1.0;
            var multipler = 1.0;

            if (attack.Type.IsEffectiveAgainst(target.Type))
            {
                multipler = _effectiveMultipler;
            }
            else if (attack.Type.IsWeakAgainst(target.Type))
            {
                multipler = _weakMultiplier;
            }

            return (int) Math.Floor(damage * multipler);
        }
    }
}