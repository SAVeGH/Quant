using Qntm.Constants;
using System;

namespace Qntm.Helpers
{
    public static class ProbabilityHelper
    {
        public static bool? IsZeroClockwise(double quantumAngle, double measurmentAngle)
        {
            double baseAngle = quantumAngle - measurmentAngle;

            if (Math.Abs(baseAngle) % Angles._180degree == 0)
                return null; // нет поворота - вектор кванта на оси измерения

            double actualAngle = AngleHelper.Positive360RangeAngle(baseAngle);

            return actualAngle < Angles._180degree;
        }

        public static double UnityProbabilityInBasis(double quantumAngle, double measurmentAngle)
        {            
            double resultDiff = AngleChangeInBasis(quantumAngle, measurmentAngle); // выбираем наименьший. Он и будет давать проекцию на линию 0 - 180 (0 - 1)

            // для нахождения синуса используем половинный угол т.к. 0 - 1 это разворот на 180 градусов, а sin 0..1 это углы от 0 до 90.
            // Вероятность 1 при текущем положении вектора
            double unityProbability = Math.Pow(Math.Sin(resultDiff / 2.0), 2.0);

            return unityProbability;
        }

        public static double AngleChangeInBasis(double quantumAngle, double measurmentAngle)
        {
            double actualMeasureAngle0 = AngleHelper.Positive360RangeAngle(measurmentAngle);

            double measurmentDiff = quantumAngle - actualMeasureAngle0;

            double anglesDiff = Math.Abs(measurmentDiff); // разница углов

            double anglesDiffRest = Angles._360degree - anglesDiff; // ответный угол            

            double resultDiff = Math.Min(anglesDiff, anglesDiffRest); // выбираем наименьший. Он и будет давать проекцию на линию 0 - 180 (0 - 1)

            return resultDiff;
        }

        public static double ProbabilityToAngle(double probability)
        {
            return Math.Asin(Math.Sqrt(probability)) * 2.0;
        }
    }
}
