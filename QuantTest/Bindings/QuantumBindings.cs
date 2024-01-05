using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Qntm;
namespace QuantTest.Bindings
{
    [Binding]
    public class QuantumBindings
    {
        [BeforeScenario]
        public void QuantumStart() 
        {
            QuantumThreadWorker.Run(129);
        }

        [AfterScenario]
        public void QuantumStop() 
        {
            //QuantumThreadWorker.Stop();
        }
    }
}
