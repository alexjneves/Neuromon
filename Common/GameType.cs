using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace Common
{
    public sealed class GameType
    {
        private readonly List<string> _effective;
        private readonly List<string> _weak;

        public int Id { get; }
        public string Name { get; }

        [JsonConstructor]
        public GameType(int id, string name, List<string> effective, List<string> weak)
        {
            Id = id;
            Name = name;
            _effective = effective;
            _weak = weak;
        }

        public bool IsEffectiveAgainst(GameType otherGameType)
        {
            return _effective.Any(type => type == otherGameType.Name);
        }

        public bool IsWeakAgainst(GameType otherGameType)
        {
            return _weak.Any(type => type == otherGameType.Name);
        }
    }
}