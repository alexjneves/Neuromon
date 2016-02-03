using System;
using System.Collections.Generic;
using System.IO;
using Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Data
{
    public sealed class GameDatabase
    {
        public List<GameType> GameTypes { get; }
        public List<Move> Moves { get; }
        public List<Neuromon> Neuromon { get; }

        public static GameDatabase CreateFromFiles(string typesFileName, string movesFileName, string neuromonFileName)
        {
            var typesJson = File.ReadAllText(typesFileName);
            var movesJson = File.ReadAllText(movesFileName);
            var neuromonJson = File.ReadAllText(neuromonFileName);

            return new GameDatabase(typesJson, movesJson, neuromonJson);
        }

        public GameDatabase(string typesJson, string movesJson, string neuromonJson)
        {
            GameTypes = ParseGameTypes(typesJson);
            Moves = ParseMoves(movesJson);
            Neuromon = ParseNeuromon(neuromonJson);
        }

        private static List<GameType> ParseGameTypes(string typesJson)
        {
            var types = JsonConvert.DeserializeObject<List<GameType>>(typesJson);

            return types;
        }

        private List<Move> ParseMoves(string movesJson)
        {
            dynamic rawMoves = JArray.Parse(movesJson);

            var moves = new List<Move>();

            foreach (var rawMove in rawMoves)
            {
                var name = (string) rawMove.name.Value;
                var type = LookupGameType(rawMove.type.Value.ToString());
                var pp = (int) rawMove.pp.Value;
                var damage = (int) rawMove.damage.Value;

                moves.Add(new Move(name, type, pp, damage));
            }

            return moves;
        }

        private List<Neuromon> ParseNeuromon(string neuromonJson)
        {
            dynamic rawNeuromons = JArray.Parse(neuromonJson);

            var neuromon = new List<Neuromon>();

            foreach (var rawNeuromon in rawNeuromons)
            {
                var name = (string) rawNeuromon.name.Value;
                var health = (int) rawNeuromon.health.Value;
                var type = LookupGameType(rawNeuromon.type.Value.ToString());

                var moves = new List<Move>();

                foreach (var rawMove in rawNeuromon.moves.ToObject<List<string>>())
                {
                    moves.Add(LookupMove(rawMove));
                }

                var moveSet = new MoveSet(moves);

                neuromon.Add(new Neuromon(name, health, type, moveSet));
            }

            return neuromon;
        }

        private GameType LookupGameType(string gameType)
        {
            var type = GameTypes.Find(t => t.Name == gameType);

            if (type == null)
            {
                throw new Exception($"Could not find type {gameType} in GameTypes");
            }

            return type;
        }

        private Move LookupMove(string moveName)
        {
            var move = Moves.Find(m => m.Name == moveName);

            if (move == null)
            {
                throw new Exception($"Could not find move {moveName} in Moves");
            }

            return move;
        }
    }
}
