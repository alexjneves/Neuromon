using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using SharpNeat.Domains;
using SharpNeat.Genomes.Neat;

namespace Trainer
{
    internal sealed class GenomeIo
    {
        private readonly INeatExperiment _neatExperiment;

        private readonly MemoryStream _championGenomeMemoryStream;
        private readonly XmlWriterSettings _xmlWriterSettings;

        public GenomeIo(INeatExperiment neatExperiment)
        {
            _neatExperiment = neatExperiment;

            _championGenomeMemoryStream = new MemoryStream();
            _xmlWriterSettings = new XmlWriterSettings { Indent = true, Encoding = Encoding.UTF8 };
        }

        public void CacheChampion(NeatGenome championGenome)
        {
            _championGenomeMemoryStream.SetLength(0);

            using (var xmlWriter = XmlWriter.Create(_championGenomeMemoryStream, _xmlWriterSettings))
            {
                _neatExperiment.SavePopulation(xmlWriter, new List<NeatGenome> { championGenome });
            }
        }

        public void WriteChampion(string filePath)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(filePath));

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                _championGenomeMemoryStream.WriteTo(fileStream);
            }
        }

        public void Write(string filePath, List<NeatGenome> genomePopulation)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(filePath));

            using (var xmlWriter = XmlWriter.Create(filePath, _xmlWriterSettings))
            {
                _neatExperiment.SavePopulation(xmlWriter, genomePopulation);
            }
        }

        public List<NeatGenome> Read(string filePath)
        {
            using (var xmlReader = XmlReader.Create(filePath))
            {
                return _neatExperiment.LoadPopulation(xmlReader);
            }
        } 
    }
}