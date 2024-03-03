using Qntm.Constants;
using System;
using System.Diagnostics;

namespace Qntm.Helpers
{
    public static class MeasurmentHelper
    {
        
        private enum MeasurmentHalfPart { First, Second };

        /// <summary>
        /// 
        /// </summary>
        /// <param name="quantum">Измеряемый квант (угол кванта в радианах)</param>
        /// <param name="measurmentAngle">Угол измерения в радианах (поворот установки)</param>
        /// <returns>Результат измерения кванта true/false в заданном базисе</returns>
        public static bool Measure(Quantum quantum, double measurmentAngle /*заданный базис измерения - поворот установки*/)
        {
            //Debug.WriteLine("Measure: --------------------------------------------- ");
            // положение полюса 0 на шкале от 0 до 360
            double actualMeasureAngle = AngleHelper.Positive360RangeAngle(measurmentAngle); // чистый угол поворота установки (без лишних оборотов)

            //Debug.WriteLine($"Measure: quantum.Angle: {Grad(quantum.Angle)}");

            // Угловое смещение осей координат в 90 градусном секторе. Смещение направления оси измерения нуля
            // Состояния измерения повторяются через 180 градусов поворота установки (т.е. единица будет при вертикальном измерении через каждые 180)
            // Так как 0 это 0 а 90 это 1 - нужно получить смещение угла в секторе от 0 до 90 т.е. смещение оси измерения в секторе 90
            double zeroShiftAngle = MeasurmentAngleToZeroAxisInSector(actualMeasureAngle /* 0 - 360 */);  // один угол поворота установки от 0 до 180 в системе 0 - 90 равен полвине градуса 

            //Debug.WriteLine("Measure: zeroShiftAngle: " + Grad(zeroShiftAngle));


            double quantumMeasurmentAngle = quantum.Angle - zeroShiftAngle;            
            // угол кванта относительно базиса измерения (насколько изменился относительно осей измерения 1 и 0)            
            double actualQuantumMeasurmentAngle = AngleToZeroAxisInSector(quantumMeasurmentAngle);
            //Debug.WriteLine("Measure: actualQuantumMeasurmentAngle: " + Grad(actualQuantumMeasurmentAngle));

            // вероятности при текущем положении вектора
            double unityProbability = Math.Pow(Math.Sin(actualQuantumMeasurmentAngle), 2.0);
            double zeroProbability = 1.0 - unityProbability;

            // насколько сдвинулись к 1. поэтому Sin. Если угол 0  - то вектор кванта на оси 0. Возводим компоненту амплитуды вероятности 1 в степень 2 для получениея вероятности 1.
            uint BasisNumerator = (uint)Math.Round((double)(QuantumThreadWorker.BasisDenominator - 1) * unityProbability);

            // результат измерения относительно заданного базиса
            bool result = QuantumThreadWorker.Measure(BasisNumerator);
            //Debug.WriteLine("Measure: result: " + result.ToString());

            double shiftToZero = AngleShiftToZeroAxis(quantumMeasurmentAngle);
            double shiftToUnity = AngleShiftToUnityAxis(quantumMeasurmentAngle);

            double resultZeroAngle = quantum.Angle + shiftToZero;
            double resultUnityAngle = quantum.Angle + shiftToUnity;
            // изменение угла кванта после измерения в заданном базисе
            // вектор кванта 'ложиться' ('прилипает') на ось 1 или 0 базиса измерения
            quantum.Angle = result ? resultUnityAngle : resultZeroAngle;
            //Debug.WriteLine("Measure: set new quantum.Angle: " + Grad(quantum.Angle));

            double shiftToZeroSign = shiftToZero >= 0 ? 1 : -1;
            double shiftToUnitySign = shiftToUnity >= 0 ? 1 : -1;

            double toZeroProbabilityChange = unityProbability * shiftToZeroSign;
            double toUnityProbabilityChange = zeroProbability * shiftToUnitySign;

            // абсолютное изменение вероятности в терминах поворота угла вероятности к оси 1 или 0
            double probabilityChange = result ? toUnityProbabilityChange : toZeroProbabilityChange;    
            
            // сдвигаем связи на угол смещения вероятности кванта
            EntangleHelper.Distribute(quantum, probabilityChange);

            // отсоединяем квант из цепи
            EntangleHelper.Collapse(quantum);

            return result;
        }

        private static double AngleShiftToZeroAxis(double quantumAngle)
        {
            QuadrantHelper.QuantumQuadrant quadrant = QuadrantHelper.GetQuantumQuadrant(quantumAngle);

            double shiftAngle = AngleToZeroAxisInSector(quantumAngle);

            switch (quadrant)
            {
                case QuadrantHelper.QuantumQuadrant.ZeroPlus:
                case QuadrantHelper.QuantumQuadrant.ZeroMinus:
                    return 0.0;
                case QuadrantHelper.QuantumQuadrant.UnityPlus:
                case QuadrantHelper.QuantumQuadrant.UnityMinus:
                    return Angles._90degree;
                case QuadrantHelper.QuantumQuadrant.First:
                case QuadrantHelper.QuantumQuadrant.Third:
                    return -shiftAngle;
                case QuadrantHelper.QuantumQuadrant.Second:
                case QuadrantHelper.QuantumQuadrant.Fourth:
                    return shiftAngle;
                default:
                    return double.NaN;
            }
        }

        private static double AngleShiftToUnityAxis(double quantumAngle)
        {
            QuadrantHelper.QuantumQuadrant quadrant = QuadrantHelper.GetQuantumQuadrant(quantumAngle);

            double shiftAngle = AngleToUnityAxisInSector(quantumAngle);

            switch (quadrant)
            {
                case QuadrantHelper.QuantumQuadrant.ZeroPlus:
                case QuadrantHelper.QuantumQuadrant.ZeroMinus:
                    return Angles._90degree;
                case QuadrantHelper.QuantumQuadrant.UnityPlus:
                case QuadrantHelper.QuantumQuadrant.UnityMinus:
                    return 0.0;
                case QuadrantHelper.QuantumQuadrant.First:
                case QuadrantHelper.QuantumQuadrant.Third:
                    return shiftAngle;
                case QuadrantHelper.QuantumQuadrant.Second:
                case QuadrantHelper.QuantumQuadrant.Fourth:
                    return -shiftAngle;
                default:
                    return double.NaN;
            }
        }

        /// <summary>
        ///  Проекция угла поворота установки в диапазоне 0 - 360 на сектор от 0 - 90 градусов
        /// </summary>
        /// <param name="measurmentAngle">угол в диапазоне 0 до 360</param>
        /// <returns>всегда положительный угол от 0 до 90 градусов (0-0, 90-1)</returns>
        private static double MeasurmentAngleToZeroAxisInSector(double measurmentAngle /* 0 - 360 */)
        {
            MeasurmentHalfPart halfPart = GetMeasurmentHalfPart(measurmentAngle);
            // один угол поворота установки от 0 до 180 в системе 0 - 90 равен полвине градуса. Поэтому делим на 2. 
            return (halfPart == MeasurmentHalfPart.First ? measurmentAngle : Angles._360degree - measurmentAngle) / 2.0;
        }

        private static MeasurmentHalfPart GetMeasurmentHalfPart(double measurmentAngle)
        {
            return measurmentAngle > Angles._180degree ? MeasurmentHalfPart.Second : MeasurmentHalfPart.First;
        }

       


        /// <summary>
        /// Показывает насколько далеко (в угловых величинах) вектор кванта отстоит от оси 0 в секторе 90 градусов между ближайшей осью 0 и осью 1 
        /// </summary>
        /// <param name="quantumAngle">Направление вектора кванта в радианах +/- </param>
        /// <returns>
        /// Возвращает результат углового удаления вектора от оси 0. Результат всегда от 0 до 90.
        /// Если вектор лежит на оси 0 - то удаление от 0 то же 0 градусов.
        /// Если вектор лежит на оси 1 - то удаление от 0 составляет 90 градусов.
        /// </returns>
        private static double AngleToZeroAxisInSector(double quantumAngle)
        {
            QuadrantHelper.QuantumQuadrant quadrant = QuadrantHelper.GetQuantumQuadrant(quantumAngle);

            double positiveQuantumAngle = AngleHelper.Positive360RangeAngle(quantumAngle);

            switch (quadrant)
            {
                case QuadrantHelper.QuantumQuadrant.ZeroPlus:
                case QuadrantHelper.QuantumQuadrant.ZeroMinus:
                    return 0.0;
                case QuadrantHelper.QuantumQuadrant.UnityPlus:
                case QuadrantHelper.QuantumQuadrant.UnityMinus:
                    return Angles._90degree;
                case QuadrantHelper.QuantumQuadrant.First:
                    return positiveQuantumAngle;
                case QuadrantHelper.QuantumQuadrant.Second:
                    return Angles._180degree - positiveQuantumAngle;
                case QuadrantHelper.QuantumQuadrant.Third:
                    return positiveQuantumAngle - Angles._180degree;
                case QuadrantHelper.QuantumQuadrant.Fourth:
                    return Angles._360degree - positiveQuantumAngle;
                default:
                    return double.NaN;
            }
        }

        private static double AngleToUnityAxisInSector(double quantumAngle)
        {
            return Angles._90degree - AngleToZeroAxisInSector(quantumAngle);
        }

        public static string Grad(double rad)
        {
            return ((180.0 / Math.PI) * rad).ToString("0.000000");
        }
    }
}
