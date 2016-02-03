using Newtonsoft.Json;

namespace Game
{
    internal sealed class GameSettings
    {
        public int NumberOfNeuromon { get; }
        public string TypesFileName { get; }
        public string MovesFileName { get; }
        public string NeuromonFileName { get; }
        public string PlayerOneName { get; }
        public string PlayerTwoName { get; }
        public string PlayerOneType { get; }
        public string PlayerTwoType { get; }


        [JsonConstructor]
        public GameSettings(int numberOfNeuromon, string typesFileName, string movesFileName, 
            string neuromonFileName, string playerOneName, string playerTwoName, 
            string playerOneType, string playerTwoType)
        {
            NumberOfNeuromon = numberOfNeuromon;
            TypesFileName = typesFileName;
            MovesFileName = movesFileName;
            NeuromonFileName = neuromonFileName;
            PlayerOneName = playerOneName;
            PlayerTwoName = playerTwoName;
            PlayerOneType = playerOneType;
            PlayerTwoType = playerTwoType;
        }
    }
}