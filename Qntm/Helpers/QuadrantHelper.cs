using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qntm.Helpers
{
    public static class QuadrantHelper
    {
        public enum QuantumQuadrant { Undefined, ZeroPlus, ZeroMinus, UnityPlus, UnityMinus, First, Second, Third, Fourth };

        /// <summary>
        /// Определяет в каком квадранте находится вектор кванта
        /// </summary>
        /// <param name="quantumAngle">Направление вектора кванта в радианах +/- </param>
        /// <returns>Квадрант или положение на одной из осей измерения</returns>
        public static QuantumQuadrant GetQuantumQuadrant(double quantumAngle)
        {
            // функции при определенных значениях PI/n реально соответсвующих значению 0 могут давать очень маленькое не нулевое значение
            // при этом положения соответсвующие 1 выдают без ошибок
            double ZeroAmplitude = Math.Cos(quantumAngle);
            double UnityAmplitude = Math.Sin(quantumAngle);

            // коррекция неправильных нулей
            if (ZeroAmplitude == 1.0 || ZeroAmplitude == -1.0)
                UnityAmplitude = 0.0;

            if (UnityAmplitude == 1.0 || UnityAmplitude == -1.0)
                ZeroAmplitude = 0.0;

            if (ZeroAmplitude > 0 && UnityAmplitude > 0)
                return QuantumQuadrant.First;
            else if (ZeroAmplitude < 0 && UnityAmplitude > 0)
                return QuantumQuadrant.Second;
            else if (ZeroAmplitude < 0 && UnityAmplitude < 0)
                return QuantumQuadrant.Third;
            else if (ZeroAmplitude > 0 && UnityAmplitude < 0)
                return QuantumQuadrant.Fourth;
            else if (ZeroAmplitude == 0.0 && UnityAmplitude > 0)
                return QuantumQuadrant.UnityPlus;
            else if (ZeroAmplitude == 0.0 && UnityAmplitude < 0)
                return QuantumQuadrant.UnityMinus;
            else if (ZeroAmplitude > 0 && UnityAmplitude == 0.0)
                return QuantumQuadrant.ZeroPlus;
            else if (ZeroAmplitude < 0 && UnityAmplitude == 0.0)
                return QuantumQuadrant.ZeroMinus;

            return QuantumQuadrant.Undefined;

        }
    }
}
