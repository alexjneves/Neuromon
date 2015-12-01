namespace Common
{
    public sealed class Neuromon
    {
        public string Name { get; }
        public int Health { get; private set; }
        public MoveSet MoveSet { get; }

        public Neuromon(string name, MoveSet moveSet)
        {
            Name = name;
            Health = 10;
            MoveSet = moveSet;
        }

        public bool IsDead() => Health <= 0;

        public void TakeDamage(int damage)
        {
            Health -= damage;

            if (Health < 0)
            {
                Health = 0;
            }
        }
    }
}