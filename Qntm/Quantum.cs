using Qntm.Helpers;
using System.Collections.Generic;

namespace Qntm
{

    public class Quantum
    {
		double _angle = double.NaN;
        double _value = double.NaN;

        public HashSet<QuantumPointer> QuantumPointers { get; private set; } = new HashSet<QuantumPointer>();

        public Quantum(double angle)
        {			
            Angle = angle;
            Value = 1.0; // размер кванта по умолчанию. Определяет 'вес' кванта при распределении вероятности. 
                         // Т.е. если квант A имеет Value = 0.5 и связан с квантом B который имеет Value = 1.0, то при повороте
                         // кванта A на 90 градусов, угол связанного кванта B изменится на только на 45 градусов. Если же
                         // квант B будет повернут на 90 градусов, то угол связанного кванта A изменится на 180 градусов.
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
            // Положение 180 (градусов) означает 'спин вниз' и при измерении дает 1 (True)
            // Разница между 0 и 1 составляет 180 градусов
            // Положения 90 градусов и 270 градусов означают вероятность 1/2
            // Отсчет положительных углов - против часовой стрелки.
            get { return _angle; }
			set 
			{
                // остаток от деления на 'целые окружности' - положение вектора за вычетом целых поворотов
                _angle = AngleHelper.Positive360RangeAngle(value);                
            }
		}


        public double Value 
        {
            get { return _value; }
            set { _value = value; }
        }

		public string Name { get; set; } // для тестов

	}
}
