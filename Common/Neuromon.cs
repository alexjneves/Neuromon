namespace Common
{
    public sealed class Neuromon
    {
        public string Name { get; }
        public int Health { get; private set; }
        public GameType Type { get; }
        public MoveSet MoveSet { get; }

        public Neuromon(string name, int health, GameType type, MoveSet moveSet)
        {
            Type = type;
            Name = name;
            Health = health;
            MoveSet = moveSet;
        }

        public Neuromon(Neuromon other)
        {
            Name = other.Name;
            Health = other.Health;
            Type = other.Type;
            MoveSet = other.MoveSet;
        }

        public bool IsDead => Health <= 0;

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