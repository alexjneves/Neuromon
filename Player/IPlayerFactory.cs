using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;

namespace Player
{
    public interface IPlayerFactory
    {
        IPlayer CreatePlayer(string name, NeuromonCollection neuromon);
    }
}
