using Qntm.Helpers;
using System;
using System.Collections.Generic;

namespace Qntm
{

    public class Quantum
    {
		// угол в радианах:
		// PI    угол     0     1 
		// 0       0      1     0
		// PI/4    45    1/2   1/2
		// PI/2    90     0     1
		// 3PI/4   135  -1/2   1/2
		// PI      180   -1     0
		// 5PI/4   225   -1/2  -1/2  
		// 3PI/2   270    0    -1
		// 7PI/4   315   1/2   -1/2

		double _angle = double.NaN;

		//double _scale = 1.0;
		public HashSet<QuantumPointer> QuantumPointers { get; private set; } = new HashSet<QuantumPointer>();

        public Quantum(double angle)
        {
			// Изначально квант запутан сам с собой
			//Next = this;           

            // амплитуда верояности получить 1 - это проекция вектора на ось Y. Т.е. это sin угла. 
            // вероятность получить 1  - это квадрат синуса угла.
            Angle = angle;
        }

        public void Reset(double angle) 
		{
			Angle = angle;
		}

        /// <summary>
        /// Угол в радианах. Можно задать любое число (+ или -). Будет интерпритировано как радианы и 
		/// вычислено положение вектора на окружности 0-360. Каждый градус на окружности соответсвует
		/// двум градусам спина т.к. спин 0-720.
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

		public double QuantumPointersCount
        {
			get { return QuantumPointers.Count; }
		}

		public string Name { get; set; }

	}
}
