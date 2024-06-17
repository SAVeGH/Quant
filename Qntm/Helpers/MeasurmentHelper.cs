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
        /// <param name="measurmentAngle">Угол измерения в радианах (поворот установки)</param>
        /// <returns>Результат измерения кванта true/false в заданном базисе</returns>
        public static bool Measure(Quantum quantum, double measurmentAngle /*заданный базис измерения - поворот установки*/, bool? setResult)
        {
            //Debug.WriteLine("Measure: --------------------------------------------- ");
            // положение полюса 0 на шкале от 0 до 360 в радианах
            double actualMeasureAngle0 = AngleHelper.Positive360RangeAngle(measurmentAngle); // чистый угол поворота установки (без лишних оборотов) в радианах
            // положение полюса 1 на шкале от 0 до 360 в радианах
            double actualMeasureAngle1 = AngleHelper.Positive360RangeAngle(actualMeasureAngle0 + Angles._180degree);

            // определяет поворот вектора кваната к полюсу 0 произойдет по часовой стрелке или против
            bool? isZeroClockwise = ProbabilityHelper.IsZeroClockwise(quantum.Angle, actualMeasureAngle0);

            // для нахождения синуса используем половинный угол т.к. 0 - 1 это разворот на 180 градусов, а sin 0..1 это углы от 0 до 90.
            // вероятности при текущем положении вектора
            double unityProbability = ProbabilityHelper.UnityProbabilityInBasis(quantum.Angle, actualMeasureAngle0);
            double zeroProbability = 1.0 - unityProbability;

            // какой линии измерения соответсвует вероятность единицы
            uint BasisNumerator = (uint)Math.Round((double)(QuantumThreadWorker.BasisDenominator - 1) * unityProbability);

            // результат измерения относительно заданного базиса
            bool result = !setResult.HasValue ? QuantumThreadWorker.Measure(BasisNumerator) : setResult.Value;
            //Debug.WriteLine("Measure: result: " + result.ToString());

            // изменение угла кванта после измерения в заданном базисе
            // вектор кванта 'ложиться' ('прилипает') на ось 1 или 0 базиса измерения
            quantum.Angle = result ? actualMeasureAngle1 : actualMeasureAngle0;
            //Debug.WriteLine("Measure: set new quantum.Angle: " + Grad(quantum.Angle));

            // изменение вероятностей: '+' - против часовой стрелки (углы увеличиваются от 0), '-' - по часовой стрелке (углы уменьшаются)
            // Пример: вероятность 1 - 0.6, 0 - 0.4. 
            // Если случится 1 - то вектор прошел от 0.6 до 1 т.е. изменился на 0.4 (zeroProbability)
            // Если случится 0 - то вектор прошел от 0.6 до 0 т.е. изменился на 0.6 (unityProbability)

            // на сколько поменялась бы вероятность при измерении в 0
            double toZeroProbabilityChange = !isZeroClockwise.HasValue ? 0 : (isZeroClockwise.Value ? -unityProbability : unityProbability);
            // на сколько поменялась бы вероятность при измерении в 1
            double toUnityProbabilityChange = !isZeroClockwise.HasValue ? 0 : (isZeroClockwise.Value ? zeroProbability : -zeroProbability);

            // изменение вероятности в результате измерения
            double probabilityChange = result ? toUnityProbabilityChange : toZeroProbabilityChange;

            // сдвигаем связи на угол смещения вероятности кванта
            EntangleHelper.Distribute(quantum, actualMeasureAngle0, probabilityChange);

            // отсоединяем квант из цепи
            EntangleHelper.Collapse(quantum);

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="quantum">Измеряемый квант (угол кванта в радианах)</param>
        /// <param name="measurmentAngle">Угол измерения в радианах (поворот установки)</param>
        /// <returns>Результат измерения кванта true/false в заданном базисе</returns>
        public static bool Measure(Quantum quantum, double measurmentAngle /*заданный базис измерения - поворот установки*/)
        {
            return Measure(quantum, measurmentAngle, null);
        }

        public static bool MeasureTo(Quantum quantum, double measurmentAngle /*заданный базис измерения - поворот установки*/, bool mResult)
        {
            return Measure(quantum, measurmentAngle, mResult);
        }

        public static bool MeasureTest(double angle, double measurmentAngle /*заданный базис измерения - поворот установки*/)
        {
            //Debug.WriteLine("Measure: --------------------------------------------- ");
            // положение полюса 0 на шкале от 0 до 360
            double actualMeasureAngle0 = AngleHelper.Positive360RangeAngle(measurmentAngle); // чистый угол поворота установки (без лишних оборотов)
            // положение полюса 1 на шкале от 0 до 360
            //double actualMeasureAngle1 = AngleHelper.Positive360RangeAngle(actualMeasureAngle0 + Angles._180degree);

            double measurmentDiff = angle - actualMeasureAngle0;

            double anglesDiff = Math.Abs(measurmentDiff); // разница углов

            double anglesDiffRest = Angles._360degree - anglesDiff; // ответный угол 

            double resultDiff = Math.Min(anglesDiff, anglesDiffRest);

            bool? zeroClockwise = ProbabilityHelper.IsZeroClockwise(angle, measurmentAngle);

            return true;
        }        

        public static string Grad(double rad)
        {
            return ((180.0 / Math.PI) * rad).ToString("0.000000");
        }
    }
}
