using Qntm.Constants;
using System;
using System.Diagnostics;

namespace Qntm.Helpers
{
    public static class MeasurmentHelper
    {
        /// <summary>
        /// Измеряет квант в заднном базисе со сдвигом квантовой цепи на величину изменения вероятности и отсоединением кванта из цепи (коллапс).
        /// </summary>
        /// <param name="quantum">Измеряемый квант (угол кванта в радианах)</param>
        /// <param name="measurmentAngle">Угол измерения в радианах (направление на 0 установки)</param>
        /// <returns>Результат измерения кванта true/false в заданном базисе</returns>
        private static bool Measure(Quantum quantum, double measurmentAngle /*заданный базис измерения - поворот установки*/, bool? setResult)
        {
            //Debug.WriteLine("Measure: --------------------------------------------- ");
            // положение полюса 0 на шкале от 0 до 360 в радианах
            double actualMeasureAngle0 = AngleHelper.Positive360RangeAngle(measurmentAngle); // чистый угол поворота установки (без лишних оборотов) в радианах
            // положение полюса 1 на шкале от 0 до 360 в радианах
            double actualMeasureAngle1 = AngleHelper.Positive360RangeAngle(actualMeasureAngle0 + Angles._180degree);
            
            // вероятности при текущем положении вектора
            double unityProbability = ProbabilityHelper.UnityProbabilityInBasis(quantum.Angle, actualMeasureAngle0);
            double zeroProbability = 1.0 - unityProbability;

            // какой линии измерения соответсвует вероятность единицы
            uint BasisNumerator = (uint)Math.Round((double)(QuantumThreadWorker.BasisDenominator - 1) * unityProbability);

            // результат измерения относительно заданного базиса
            bool result = !setResult.HasValue ? QuantumThreadWorker.Measure(BasisNumerator) : setResult.Value;
            //Debug.WriteLine("Measure: result: " + result.ToString());

            bool? isZeroClockwise = ProbabilityHelper.IsZeroClockwise(quantum.Angle, actualMeasureAngle0);

            //// изменение против часовой +, по часовой -
            double probabilityChange =
                isZeroClockwise == null ? 0 : isZeroClockwise.Value ?  // 0 т.к. вектор строго на оси измерения. Изменения на будет.
                (result ? zeroProbability : -unityProbability) : // 0 по часовой 
                (result ? -zeroProbability : unityProbability); // 0 против часовой

            // изменение угла кванта после измерения в заданном базисе
            // вектор кванта 'ложиться' ('прилипает') на ось 1 или 0 базиса измерения
            double measuredBasisAngle = result ? actualMeasureAngle1 : actualMeasureAngle0;
            //Debug.WriteLine("Measure: set new quantum.Angle: " + Grad(quantum.Angle));

            Quantum.Distribute(quantum, probabilityChange, measuredBasisAngle);

            // отсоединяем квант из цепи
            EntangleHelper.Collapse(quantum);

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="quantum">Измеряемый квант (угол кванта в радианах)</param>
        /// <param name="measurmentAngle">Угол измерения в радианах (поворот нуля установки)</param>
        /// <returns>Результат измерения кванта true/false в заданном базисе</returns>
        public static bool Measure(Quantum quantum, double measurmentAngle /*заданный базис измерения - поворот установки*/)
        {
            return Measure(quantum, measurmentAngle, null);
        }

        public static bool MeasureTo(Quantum quantum, double measurmentAngle /*заданный базис измерения - поворот установки*/, bool mResult)
        {
            return Measure(quantum, measurmentAngle, mResult);
        }

        public static string Grad(double rad)
        {
            return ((180.0 / Math.PI) * rad).ToString("0.000000");
        }
    }
}
