using Common;

namespace Game.Damage
{
    internal interface IDamageCalculator
    {
        int CalculateDamage(Move attack, Neuromon target);
    }
}