namespace Common
{
    public sealed class Move
    {
        public string Name { get; }
        public GameType Type { get; }
        public int Pp { get; }
        public int Damage { get; }

        public Move(string name, GameType type, int pp, int damage)
        {
            Name = name;
            Type = type;
            Pp = pp;
            Damage = damage;
        }
    }
}