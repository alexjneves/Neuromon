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
        private readonly string _brainFileName;
        private readonly int _inputCount;
        private readonly int _outputCount;
        private readonly GameStateSerializer _gameStateSerializer;

        public NeatAiPlayerControllerFactory(string brainFileName, int numberOfNeuromon, int inputCount, int outputCount) : this(numberOfNeuromon, null, brainFileName, inputCount, outputCount)
        {
        }

        public NeatAiPlayerControllerFactory(IBlackBox brain, int numberOfNeuromon) : this(numberOfNeuromon, brain, "", 0, 0)
        {
        }

        private NeatAiPlayerControllerFactory(int numberOfNeuromon, IBlackBox brain, string brainFileName, int inputCount, int outputCount)
        {
            _brain = brain;
            _brainFileName = brainFileName;
            _inputCount = inputCount;
            _outputCount = outputCount;
            _gameStateSerializer = new GameStateSerializer(numberOfNeuromon);
        }

        public IPlayerController CreatePlayer(IPlayerState initiaPlayerState)
        {
            if (_brain != null)
            {
                return new NeatAiPlayerController(_brain, _gameStateSerializer);
            }

            // TODO: Refactor
            var genomeDecoder = new NeatGenomeDecoder(NetworkActivationScheme.CreateCyclicFixedTimestepsScheme(1));
            var fact = new NeatGenomeFactory(_inputCount, _outputCount, new NeatGenomeParameters());

            NeatGenome genome;
            using (var xr = XmlReader.Create(_brainFileName))
            {
                genome = NeatGenomeXmlIO.ReadCompleteGenomeList(xr, false, fact)[0];
            }

            return new NeatAiPlayerController(genomeDecoder.Decode(genome), _gameStateSerializer);
        }
    }
}