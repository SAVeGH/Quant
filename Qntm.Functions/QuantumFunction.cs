using System;
using System.Threading.Tasks;

namespace Qntm.Functions
{    
    public abstract class QuantumFunction
    {
        /// <summary>
        /// Выдает смещение после выполнения функции для переданного кванта 
        /// </summary>
        /// <param name="q">Квант</param>
        /// <returns>Итоговое смещение после выполнения функции</returns>
        protected abstract Task<double> Call(Quantum q);

        public double CallFunction(Quantum q) 
        {
            // асинхронное выполнение функции на всех параметрах
            return Call(q).GetAwaiter().GetResult();
        }

        // Обертка для асинхронного вызова функции
        protected static Task<T> FunctionWrapper<T>(T prm, Func<T, T> blackBox)
        {
            return Task.Run(() => { T result = blackBox(prm); return result; });
        }
    }
}
