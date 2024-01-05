using System.Threading;

namespace Qntm
{
    public static class QuantumThreadWorker
    {
        private static Thread _quantumThread;        
        public static uint BasisDenominator { get; private set; } // Должен быть нечет - ширина матрицы по X
        
        private static int _currentRowIndex;
        private static void InitQuantumThread()
        {
            ThreadStart threadStart = new ThreadStart(RotateState);

            _quantumThread = new Thread(threadStart);
            _quantumThread.IsBackground = true;
            _quantumThread.Start();
        }

        public static void Run(uint size) 
        {
            if (_quantumThread != null)
                return;            

            BasisDenominator = size % 2 == 0 ? size + 1 : size; // всегда нечет. Ширина матрицы

            InitQuantumThread();
        }

        private static void RotateState()
        {
            uint rowIndex = 0;
            uint matrixHeight = BasisDenominator - 1; 

            while (true) // Делаем бесконечный цикл.
            {
                rowIndex = rowIndex % matrixHeight; // количество строк всегда чет. Для BasisDenominator = 5  rowIndex = 0..3
                Interlocked.Exchange(ref _currentRowIndex, (int)rowIndex);
                rowIndex++;
            }

        }

        public static bool Measure(uint columnIndex) 
        {
            //  Матрица для BasisDenominator = 5
            //
            //                    columnIndex
            //                  i 0 1 2 3 4
            // _currentRowIndex 0 0 1 1 1 1
            //                  1 0 0 1 1 1
            //                  2 0 0 0 1 1
            //                  3 0 0 0 0 1

            uint measurmentColumnIndex = columnIndex % BasisDenominator; // количество колонок всегда нечет. Для BasisDenominator = 5  columnIndex = 0..4

            bool result = measurmentColumnIndex - _currentRowIndex > 0;

            return result;
        }

        public static void Stop()
        {
            _quantumThread.Abort();
        }
    }
}
