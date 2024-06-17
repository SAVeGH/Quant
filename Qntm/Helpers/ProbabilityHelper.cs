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
                return null; // нет поворота - или оба на 0 или на 180 повернуты

            double actualAngle = AngleHelper.Positive360RangeAngle(baseAngle);

            return actualAngle < Angles._180degree;
        }

        public static double UnityProbabilityInBasis(double quantumAngle, double measurmentAngle)
        {
            double actualMeasureAngle0 = AngleHelper.Positive360RangeAngle(measurmentAngle);

            double measurmentDiff = quantumAngle - actualMeasureAngle0;

            double anglesDiff = Math.Abs(measurmentDiff); // разница углов

            double anglesDiffRest = Angles._360degree - anglesDiff; // ответный угол            

            double resultDiff = Math.Min(anglesDiff, anglesDiffRest); // выбираем наименьший. Он и будет давать проекцию на линию 0 - 180 (0 - 1)

            // для нахождения синуса используем половинный угол т.к. 0 - 1 это разворот на 180 градусов, а sin 0..1 это углы от 0 до 90.
            // Вероятность 1 при текущем положении вектора
            double unityProbability = Math.Pow(Math.Sin(resultDiff / 2.0), 2.0);

            return unityProbability;
        }

        /// <summary>
        /// Находит угол кванта соответсвующий переданной вероятности в заданном базисе
        /// </summary>
        /// <param name="probability">Вероятность полученная в результате поворота вероятности. Значение от -1 до +2. Значения за диапазоном 0-1 - означает проворот
        /// с пересечением линии измерения</param>
        /// <param name="basisAngle0">Угол кванта соответсвующий вероятности</param>
        /// <returns></returns>
        public static double AngleOfProbabilityInBasis(double probability, double basisAngle0)
        {
            double unityProbability = probability;
            double resultAngle = ProbabilityToAngle(unityProbability); // > 1 && < 0 -> NaN           

            if (probability > 1.0)
            {
                unityProbability = 2.0 - probability;
                resultAngle = Angles._360degree - ProbabilityToAngle(unityProbability);
            }
            else if (probability < 0.0)
            {
                unityProbability = Math.Abs(probability);
                resultAngle = Angles._360degree - ProbabilityToAngle(unityProbability);
            }

            resultAngle = AngleHelper.Positive360RangeAngle(resultAngle + basisAngle0);

            return resultAngle;
        }

        private static double ProbabilityToAngle(double probability)
        {
            return Math.Asin(Math.Sqrt(probability)) * 2.0;
        }
    }
}
