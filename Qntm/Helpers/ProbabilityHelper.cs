using Qntm.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            // вероятности при текущем положении вектора
            double unityProbability = Math.Pow(Math.Sin(resultDiff / 2.0), 2.0);

            return unityProbability;
        }

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
