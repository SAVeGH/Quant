using Qntm.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qntm.Helpers
{
    public static class EntangleHelper
    {
        public static void Entangle(Quantum quantum, Quantum nextQuantum)
        {
            Quantum chainTail = GetTail(quantum);

            chainTail = chainTail == null ? quantum : chainTail;

            nextQuantum.Next = quantum;
            chainTail.Next = nextQuantum;
        }

        public static Quantum Collapse(Quantum quantum)  
        {
            if (quantum.Next == quantum)
                return quantum;

            Quantum chainHead = quantum.Next;
            Quantum chainTail = GetTail(quantum);

            chainTail.Next = chainHead;

            quantum.Next = quantum;

            return chainHead; // оставшаяся цепь
        }

        public static void Roll(Quantum quantum, double probabilityChainShift)
        {
            if (quantum == null)
                return;

            int chainLength = EntangledChainLength(quantum);

            // поворачиваем все кванты на полученный угол
            RollChain(quantum, quantum.Next, probabilityChainShift, chainLength);
        }

        
        private static void RollChain(Quantum quantum, Quantum nextQuantum, double probabilityChainShift /*изменение вероятности*/, int chainLength) 
        {
            // уменьшаем передаточное число на длину оставшейся цепи. Новый Scale сработает при следующем измерении в цепи
            nextQuantum.Scale = nextQuantum.Scale / (double)(chainLength + 1);

            double currentProbAngle = ProbabilityAngle(nextQuantum); // текущий угол кванта в углах вероятности
            double shiftProbAngle = probabilityChainShift * Angles._90degree; // угол вероятности изменения
            double resultProbAngle = currentProbAngle + shiftProbAngle; // получился угол вероятности в радианах

            nextQuantum.Angle = QuantumAngle(resultProbAngle);

            //var t = AngleHelper.RadiansToDegree(nextQuantum.Angle);

            if (quantum == nextQuantum)
                return;

            RollChain(quantum, nextQuantum.Next, probabilityChainShift, chainLength);
        }

        private static double QuantumAngle(double quntumProbabilityAngle)
        {

            QuadrantHelper.QuantumQuadrant quadrant = QuadrantHelper.GetQuantumQuadrant(quntumProbabilityAngle);

            double probabilityAngle = Math.Asin(Math.Abs(Math.Sin(quntumProbabilityAngle)));

            switch (quadrant)
            {
                case QuadrantHelper.QuantumQuadrant.ZeroPlus: // вектор лежит на оси +0
                    probabilityAngle = Angles._0degree;
                    break;
                case QuadrantHelper.QuantumQuadrant.ZeroMinus:
                    probabilityAngle = Angles._180degree;
                    break;
                case QuadrantHelper.QuantumQuadrant.UnityPlus:
                    probabilityAngle = Angles._90degree;
                    break;
                case QuadrantHelper.QuantumQuadrant.UnityMinus:
                    probabilityAngle = Angles._270degree;
                    break;

                case QuadrantHelper.QuantumQuadrant.First:
                    break;

                case QuadrantHelper.QuantumQuadrant.Second:
                    probabilityAngle = Angles._180degree - probabilityAngle;
                    break;

                case QuadrantHelper.QuantumQuadrant.Third:
                    probabilityAngle = probabilityAngle + Angles._180degree;
                    break;

                case QuadrantHelper.QuantumQuadrant.Fourth:
                    probabilityAngle = Angles._360degree - probabilityAngle;
                    break;

                default:
                    return double.NaN;
            }

            return probabilityAngle; // угол вероятности в радианах
        }

        private static double ProbabilityAngle(Quantum quantum)
        {

            QuadrantHelper.QuantumQuadrant quadrant = QuadrantHelper.GetQuantumQuadrant(quantum.Angle);

            double probabilityAngle = Math.Pow(Math.Sin(quantum.Angle), 2.0) * Angles._90degree; // доля от 90 градусов в радианах

            switch (quadrant)
            {
                case QuadrantHelper.QuantumQuadrant.ZeroPlus: // вектор лежит на оси +0
                    probabilityAngle = Angles._0degree;                    
                    break;
                case QuadrantHelper.QuantumQuadrant.ZeroMinus:
                    probabilityAngle = Angles._180degree;
                    break;
                case QuadrantHelper.QuantumQuadrant.UnityPlus:
                    probabilityAngle = Angles._90degree;
                    break;
                case QuadrantHelper.QuantumQuadrant.UnityMinus:
                    probabilityAngle = Angles._270degree;
                    break;

                case QuadrantHelper.QuantumQuadrant.First:                   
                    break;
                    
                case QuadrantHelper.QuantumQuadrant.Second:
                    probabilityAngle = Angles._180degree - probabilityAngle; 
                    break;
                    
                case QuadrantHelper.QuantumQuadrant.Third:
                    probabilityAngle = probabilityAngle + Angles._180degree; 
                    break;

                case QuadrantHelper.QuantumQuadrant.Fourth:
                    probabilityAngle = Angles._360degree - probabilityAngle;
                    break;

                default:
                    return double.NaN;
            }

            return probabilityAngle; // угол вероятности в радианах
        }

        public static string Grad(double rad)
        {
            return ((180.0 / Math.PI) * rad).ToString("0.000000");
        }

        private static int EntangledChainLength(Quantum quantum)
        {
            if (quantum.Next == null)
                return 1;

            int length = EntangledChainLength(quantum, quantum.Next, 1);

            return length;
        }

        private static int EntangledChainLength(Quantum sourceQuantum, Quantum nextQuantum, int count)
        {
            if (sourceQuantum == nextQuantum.Next)
                return count;

            return EntangledChainLength(sourceQuantum, nextQuantum.Next, count + 1);
        }

        private static Quantum GetTail(Quantum quantum)
        {
            if (quantum.Next == quantum)
                return quantum;

            Quantum chainTail = GetTail(quantum, quantum.Next);

            return chainTail;
        }

        private static Quantum GetTail(Quantum quantum, Quantum nextQuantum)
        {
            if (quantum == nextQuantum.Next)
                return nextQuantum;

            return GetTail(quantum, nextQuantum.Next);
        }
    }
}
