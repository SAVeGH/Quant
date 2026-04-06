using Qntm.Constants;
using Qntm.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
            List<DeuthTaskData> fnTasks = new List<DeuthTaskData>();

            // создаем таски для выполнения по всем возможным параметрам функции
            FillTasks(q, q, fnTasks);

            // ждем параллельного выполнения всех тасок ("квантовый параллелизм")
            await Task.WhenAll(fnTasks.Select(item=> item.CallTask));

            return TranslateResult(q, fnTasks);
        }

        private double TranslateResult(Quantum basisQuantum, List<DeuthTaskData> fnTasks) 
        {
            double result = 0.0;

            foreach (DeuthTaskData callItem in fnTasks) 
            {
                bool inputValue = !(callItem.CallQuantum.Angle == basisQuantum.Angle); // параметр вызова функции
                bool outputValue = callItem.CallTask.Result; // результат функции для данного параметра

                outputValue = inputValue ? XOR(inputValue, outputValue) : outputValue; // если параметр функции 1 (т.е. это "нижний" кубит), то делается XOR (исключающее ИЛИ).

                if (inputValue != outputValue)
                    result = result + Angles._180degree;
            }

            return AngleHelper.Positive360RangeAngle(result);
        }

        private void FillTasks(Quantum q, Quantum nextQ, List<DeuthTaskData> fnTasks)
        {
            bool fnInputParam = !(nextQ.Angle == q.Angle); // угол стартового кванта используется как "базис функции" - направление на 0 
                                                           // поэтому если углы совпали - считаем что квант в этом базисе имеет состояние 0 (т.е. параметр false)

            DeuthTaskData taskData = new DeuthTaskData() 
            {
                CallQuantum = nextQ, // на каком кванте вызвана функция
                CallTask = FunctionWrapper(fnInputParam, deutchFunction) // вызов с парамтром в базисе функции
            };            

            fnTasks.Add(taskData);

            foreach (QuantumPointer qPointer in nextQ.QuantumPointers)
            {
                if (qPointer.Quantum == q)
                    return; // обошли цепь по кругу - выходим

                FillTasks(q, qPointer.Quantum, fnTasks);
            }
        }

        private static bool XOR(bool a, bool b) 
        {
            return a != b;
        }
    }

    public class DeuthTaskData 
    {
        public Quantum CallQuantum { get; set; } // квант относительно которого выполняется функция
        public Task<bool> CallTask { get; set; } // задача, которая выполняет функцию для данного кванта
    }
}
