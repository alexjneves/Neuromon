namespace Common
{
    public sealed class Move
    {
        public string Name { get; }
        public int Damage { get; }

        public Move(string name, int damage)
        {
            Name = name;
            Damage = damage;
        }
    }
}