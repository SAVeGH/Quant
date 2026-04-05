using Qntm.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;
using Qntm.Helpers;

namespace Qntm.Functions
{
    public class DeuthQuantumFunction : QuantumFunction
    {
        Func<bool, bool> deutchFunction;
        public DeuthQuantumFunction(Func<bool,bool> blackBox) 
        {
            deutchFunction = blackBox;
        }

        protected override async Task<double> Call(Quantum q) 
        {
            List<Tuple<bool, Task<bool>>> fnTasks = new List<Tuple<bool,Task<bool>>>();

            FillTasks(q, q, fnTasks);

            await Task.WhenAll(fnTasks.Select(item=> item.Item2));

            return TranslateResult(fnTasks);
        }

        private double TranslateResult(List<Tuple<bool, Task<bool>>> fnTasks) 
        {
            double result = 0.0;

            foreach (Tuple<bool, Task<bool>> item in fnTasks) 
            {
                bool inputValue = item.Item1;
                bool outputValue = item.Item2.Result;

                if (inputValue != outputValue)
                    result = result + Angles._180degree;
            }

            return AngleHelper.Positive360RangeAngle(result);
        }

        protected void FillTasks(Quantum q, Quantum nextQ, List<Tuple<bool, Task<bool>>> fnTasks)
        {
            bool fnInputParam = nextQ.Angle == q.Angle;

            Tuple<bool, Task<bool>> taskCallParams = new Tuple<bool, Task<bool>>(fnInputParam, FunctionWrapper(fnInputParam, deutchFunction));

            fnTasks.Add(taskCallParams);

            foreach (QuantumPointer qPointer in nextQ.QuantumPointers)
            {
                if (qPointer.Quantum == q)
                    return;

                FillTasks(q, qPointer.Quantum, fnTasks);
            }
        }
    }
}
