using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qntm
{
    public class QuantumPointer
    {
        private Quantum _quantum;

        public QuantumPointer(Quantum quantum) 
        {
            _quantum = quantum;
        }

        public Quantum Quantum { get { return _quantum; } }
    }
}
