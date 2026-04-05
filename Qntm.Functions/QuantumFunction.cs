using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Qntm;

namespace Qntm.Functions
{    
    public class QuantumFunction
    {
        protected virtual Task<double> Call(Quantum q) { return Task.FromResult(0.0); }
        public double CallFunction(Quantum q) 
        {
            return Call(q).GetAwaiter().GetResult();
        }

        protected static Task<T> FunctionWrapper<T>(T prm, Func<T, T> blackBox)
        {
            T result = blackBox(prm);

            return Task.FromResult(result);
        }
    }
}
