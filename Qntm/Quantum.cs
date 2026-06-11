using Qntm.Constants;
using Qntm.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Qntm
{

    public class Quantum
    {
		double _angle = double.NaN;
        double _value = double.NaN;

        public HashSet<QuantumPointer> QuantumPointers { get; private set; } = new HashSet<QuantumPointer>();

        public Quantum(double angle)
        {
            _angle = angle;
            _value = 1.0; // размер кванта по умолчанию. Определяет 'вес' кванта при распределении вероятности. 
                          // Т.е. если квант A имеет Value = 0.5 и связан с квантом B который имеет Value = 1.0, то при повороте
                          // кванта A на 90 градусов, угол связанного кванта B изменится на только на 45 градусов. Если же
                          // квант B будет повернут на 90 градусов, то угол связанного кванта A изменится на 180 градусов.
        }

        public void Reset(double angle) // для тестов
        {
            _angle = angle;
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

            // НЕЛЬЗЯ делать открытый сеттер. Теряется +/- в углах. Не будет понятно в какую сторону и на сколько оборотов был повернут квант. Поэтому для изменения угла нужно использовать метод смещения угла ShiftAngle

            private set
            {
                double resultAngle = AngleHelper.Positive360RangeAngle(value);

                _angle = resultAngle;
            }
        }

        public double Value
        {
            get { return _value; }
            set { _value = value; }
        }

        public string Name { get; set; } // для тестов

        public double AngleGrad // для тестов
        {
            get { return AngleHelper.RadiansToDegree(Angle); }
        }

        public static int Precision { get; set; } = 5; // 10e-5 - по умолчанию 0.00001 часть угла.
                                                       // Т.е. если угол кванта после смещения оказался в пределах 0.00001 градуса от базиса измерения,
                                                       // то приравниваем его к базису измерения.       

        public static void ShiftAngle(Quantum quantum, double angleShift)
        {
            double shiftDirection = angleShift < 0 ? -1.0 : 1.0;

            double fullHalfRotations = shiftDirection < 0 ? Math.Ceiling(angleShift / Angles._180degree) : Math.Floor(angleShift / Angles._180degree);
            double restHalfRotations = angleShift % Angles._180degree;           
            
            double restHalfRotationsProbabilityChange = ProbabilityHelper.UnityProbabilityInBasis(restHalfRotations, 0.0) * shiftDirection;

            double probabilityChange = fullHalfRotations + restHalfRotationsProbabilityChange;

            double finalAngle = AngleHelper.Positive360RangeAngle(quantum.Angle + angleShift);

            Distribute(quantum, probabilityChange, finalAngle);
        }

        public static void ShiftAngleByProbabilityShift(Quantum quantum, double propabilityShift, double basisMeasuredAngle)
        {
            // 1. узнать вероятность кванта (вероятность единицы угла кваната считая нулем измеренный угол) в контексте измеренного угла
            double probabilityInBasis = ProbabilityHelper.UnityProbabilityInBasis(quantum.Angle, basisMeasuredAngle);

            // 2. с какой стороны лежит 0 (измеренный угол) от вектор кванта
            bool? isClockwise = ProbabilityHelper.IsZeroClockwise(quantum.Angle, basisMeasuredAngle);

            // 3. если бы угол кванта совпал с уголм измерения - то бралось бы просто смещение
            // Берем смещеине и отнимаем (или прибавляем) "начальное положение" т.е. вероятность
            // кванта в контексте измеренногго угла. Т.е.само смещеине propabilityShift применяем как бы к 0 (измеренному углу)
            // и добавляем начальное положение probabilityInBasis

            // если 0 (измеренный угол) лежит против часовой стрелки от угла кванта - то надо взять вероятность с минусом
            // если по часовой - то надо взять с плюсом
            // Если же isClockwise == null то начальное смещение или 0 или 1 ( 1 и -1 это одно и то же смещение. А веротность 1.3 это то же положение что и -0.7)
            // К 0 и 1 можно применять просто смещеине.
            probabilityInBasis = isClockwise == null ? probabilityInBasis : (isClockwise.Value ? probabilityInBasis : -probabilityInBasis);

            // 4. полный относительный сдвиг с учетом начального положения угла кванта
            // тут может быть несколько полных оборотов. Полный оборот вероятности - это 2.0 (-2.0)
            // Т.е. какое смещение "в вероятностях" нужно применить к измренному углу, что бы получить вектор кванта
            // (на каком расстоянии "в вероятностях" находится вектор кванта от 0).
            double resultProbabilityShift = probabilityInBasis + propabilityShift;

            // 5. теперь нужно убрать лишние обороты и найти финальную вероятность относительно измеренного угла
            // Получится значение в диапазоне -2.0 ... +2.0. Т.е. в пределах одного оборота
            // Это будет смещение вероятности которое нужно применить к измеренному углу, чтобы получить вероятность кванта после смещения. 
            // Смещение находится в пределах одного оборота, но может быть как положительным так и отрицательным.
            // Положительное смещение - это смещение против часовой стрелки от измеренного угла, а отрицательное - по часовой.
            double finalProbabilityShift = resultProbabilityShift % 2.0;

            // 6. перевести значение смещения из диапазона -2.0 ... +2.0 в значение в положительном диапазоне 0.0 .. +2.0
            // Смещение +1.3 это то же самое положение вектора, что и -0.7.
            double positiveFinalProbabilityShift = finalProbabilityShift < 0 ? finalProbabilityShift + 2.0 : finalProbabilityShift;

            // 7. Чистая положительная вероятность единицы относительно измеренного угла. Это значение в диапазоне 0.0 ... 1.0. Т.е. в пределах пол оборота.
            double shiftUnityProbability = positiveFinalProbabilityShift > 1.0 ? 2.0 - positiveFinalProbabilityShift : positiveFinalProbabilityShift;

            // 8. перевести вероятность в угол смещения
            double shiftAngle = ProbabilityHelper.ProbabilityToAngle(shiftUnityProbability);

            // 9. определить направление смещения (по часовой или против часовой) в зависимости от знака смещения вероятности
            // Так как определяется "насколько надо отступить от 0 для получения положения веектора кванта" - то знак смещения противоположен знаку смещения вероятности.
            double shiftDirection = propabilityShift < 0 ? 1.0 : -1.0;

            // 10. найти итоговое положение угла кванта после смещения на угол, который соответствует смещению вероятности
            // Угол мог получиться больше 360 или меньше 0.
            double resultAngle = basisMeasuredAngle + shiftAngle * shiftDirection;

            // 11. отразить итоговое положение угла кванта после смещения на угол, который соответствует смещению вероятности,
            // в положительный диапазон 0 - 360
            double quantumAngle = AngleHelper.Positive360RangeAngle(resultAngle);

            // 12. Определяем допустимую точность (10e-5 часть угла по умолчанию).
            double precision = Angles._360degree / (360.0 * Math.Pow(10.0, Precision));

            // 13. находим разницу между углом кванта после смещения на угол, который соответствует смещению вероятности, и углом измерения
            double measurmentDiff = Math.Abs(quantumAngle - basisMeasuredAngle);

            // 14. если после смещения угол кванта оказался в пределах точности от базиса измерения, то приравниваем его к базису измерения.
            double resultQuantumAngle = measurmentDiff < precision ? basisMeasuredAngle : quantumAngle;

            // 15. устанавливаем новое положение угла кванта после смещения на угол, который соответствует смещению вероятности
            quantum.Angle = resultQuantumAngle;
        }	

        public static void Distribute(Quantum quantum, double probabilityChange, double basisMeasuredAngle)
        {
            if (quantum == null)
                return;

            List<Quantum> passedList = new List<Quantum>();

            Distribute(quantum, probabilityChange, basisMeasuredAngle, passedList);
        }

        private static void Distribute(Quantum quantum, double probabilityChange, double basisMeasuredAngle, List<Quantum> passedList)
        {
            if (quantum == null)
                return;

            ShiftAngleByProbabilityShift(quantum, probabilityChange, basisMeasuredAngle);

            passedList.Add(quantum);

            if (quantum.QuantumPointers.Count == 0)
                return;            

            List<QuantumPointer> linksList = quantum.QuantumPointers.Where(qp => !passedList.Contains(qp.Quantum)).ToList();

            // сколько пришлось на каждую связь (доля изменения). Учет размера кванта (Value) и инверсии связи.           
            Dictionary<QuantumPointer, double> probabilityChangeParts = QuantumsChangeParts(quantum, linksList);

            foreach (QuantumPointer quantumPointer in probabilityChangeParts.Keys)
            {
                double probabilityChangePart = probabilityChangeParts[quantumPointer];

                double resultProbabilityChange = probabilityChange * probabilityChangePart;

                // распределяем долю изменения кванта по его ссылкам
                Distribute(quantumPointer.Quantum, resultProbabilityChange, basisMeasuredAngle, passedList);
            }
        }

        private static Dictionary<QuantumPointer, double> QuantumsChangeParts(Quantum quantum, List<QuantumPointer> linksList)
        {
            Dictionary<QuantumPointer, double> changeParts = new Dictionary<QuantumPointer, double>();

            if (linksList.Count == 0)
                return changeParts;

            double linkPart = (1.0 / linksList.Count); // сколько пришлось на каждую связь

            foreach (QuantumPointer quantumPointer in linksList)
            {
                double connectionChangeSign = quantumPointer.IsInverse ? -1.0 : 1.0;

                double distributionPart = linkPart * (quantum.Value / quantumPointer.Quantum.Value) * connectionChangeSign; // доля приходящаяся на квант в зависимости от его размера и инверсии связи.
                                                                                                                            // Если квант больше, то он получает большую долю изменения.
                                                                                                                            // Если связь инверсная, то изменение идет в противоположную сторону.

                double qChangePart = quantumPointer.Quantum.Value != 0.0 ?
                    distributionPart : // доля приходящаяся на квант в зависимости от его размера
                    0;

                changeParts[quantumPointer] = qChangePart;
            }

            return changeParts;
        }
    }
}
