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
            // Если базис измерения совпадает с направлением 0 кванта:
            // Положение 0 (градусов) означает 'спин вверх' и при измерении дает 0 (False)
            // Положение 270 (градусов) означает 'спин вниз' и при измерении дает 1 (True)
            // Разница между 0 и 1 составляет 180 градусов
            // Положения 90 градусов и 180 градусов означают вероятность 1/2
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
