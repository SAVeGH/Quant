using Qntm.Helpers;
using System.Collections.Generic;

namespace Qntm
{

    public class Quantum
    {
		double _angle = double.NaN;

		public HashSet<QuantumPointer> QuantumPointers { get; private set; } = new HashSet<QuantumPointer>();

        public Quantum(double angle)
        {			
            Angle = angle;
        }

        public void Reset(double angle) 
		{
			Angle = angle;
		}

        /// <summary>
        /// Угол в радианах. Можно задать любое число (+ или -). Будет интерпритировано как радианы и 
		/// вычислено положение вектора на окружности 0-360. 
        /// </summary>
        public double Angle 
		{
			get { return _angle; }
			set 
			{
                // остаток от деления на 'целые окружности' - положение вектора за вычетом целых поворотов
                _angle = AngleHelper.Positive360RangeAngle(value);                
            }
		}

		public string Name { get; set; }

	}
}
