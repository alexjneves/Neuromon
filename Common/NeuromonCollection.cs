using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Common
{
    public sealed class NeuromonCollection : IEnumerable<Neuromon>
    {
        private readonly IList<Neuromon> _neuromon;

        public int Size { get; }

        public NeuromonCollection(IEnumerable<Neuromon> neuromon)
        {
            var neuromonList = neuromon as IList<Neuromon> ?? neuromon.ToList();

            Size = neuromonList.Count;
            _neuromon = neuromonList;
        }

        public Neuromon this[int key] => _neuromon[key];

        public IEnumerator<Neuromon> GetEnumerator()
        {
            return _neuromon.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}