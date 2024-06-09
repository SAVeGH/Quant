using Qntm.Constants;
using System;

namespace Qntm.Helpers
{
    public static class AngleHelper
    {
        /// <summary>
        /// Положительный угол на шкале 0-360 в радианах
        /// </summary>
        /// <param name="angle">Значение угла (может быть + и -). Значение будет считаться радианами.</param>
        /// <returns>Положительный угол (0-360) в радианах</returns>
        public static double Positive360RangeAngle(double angle)
        {
            // убираем лишние обороты
            double actualAngle = angle % Angles._360degree;

            double resultAngle = actualAngle < 0 ? Angles._360degree + actualAngle : actualAngle;
            
            return resultAngle;
        }

        public static bool? IsZeroClockwise(double qAngle, double mAngle)
        {
            double restAngle = qAngle - mAngle;

            if (Math.Abs(restAngle) % Angles._180degree == 0)
                return null; // нет поворота - или оба на 0 или на 180 повернуты

            double actualMeasureAngle0 = AngleHelper.Positive360RangeAngle(restAngle);

            return actualMeasureAngle0 < Angles._180degree;
        }

        public static double DegreeToRadians(double degreeAngle)
        {
            return Angles._rad * degreeAngle;
        }

        public static double RadiansToDegree(double radAngle)
        {
            return Angles._grad * radAngle;
        }
    }
}
