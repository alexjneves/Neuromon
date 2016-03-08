using System.Xml;
using SharpNeat.Decoders;
using SharpNeat.Decoders.Neat;
using SharpNeat.Genomes.Neat;
using SharpNeat.Phenomes;

namespace Player.AI.Neat
{
    public sealed class NeatAiPlayerControllerFactory : IPlayerControllerFactory
    {
        private readonly IBlackBox _brain;
        private readonly GameStateSerializer _gameStateSerializer;

        public NeatAiPlayerControllerFactory(string brainFileName, int numberOfNeuromon, int inputCount, int outputCount) : 
            this(BrainFromFile(brainFileName, inputCount, outputCount), numberOfNeuromon)
        {
        }

        public NeatAiPlayerControllerFactory(IBlackBox brain, int numberOfNeuromon)
        {
            _brain = brain;
            _gameStateSerializer = new GameStateSerializer(numberOfNeuromon);
        }

        public IPlayerController CreatePlayer(IPlayerState initiaPlayerState)
        {
            return new NeatAiPlayerController(_brain, _gameStateSerializer);
        }

        private static IBlackBox BrainFromFile(string brainFileName, int inputCount, int outputCount)
        {
            // TODO: Refactor
            var genomeDecoder = new NeatGenomeDecoder(NetworkActivationScheme.CreateCyclicFixedTimestepsScheme(1));
            var genomeFactory = new NeatGenomeFactory(inputCount, outputCount, new NeatGenomeParameters());

            NeatGenome genome;
            using (var xr = XmlReader.Create(brainFileName))
            {
                genome = NeatGenomeXmlIO.ReadCompleteGenomeList(xr, false, genomeFactory)[0];
            }

            return genomeDecoder.Decode(genome);
        }
    }
}