using Common;

namespace Game.Damage
{
    public interface IDamageCalculator
    {
        int CalculateDamage(Move attack, Neuromon target);
    }
}