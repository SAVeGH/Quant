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
        public static void Entangle(Quantum quantum1, Quantum quantum2, bool doRingify = false)
        {
            QuantumPointer quantumPointer1 = new QuantumPointer(quantum1);
            QuantumPointer quantumPointer2 = new QuantumPointer(quantum2);

            quantum1.QuantumPointers.Add(quantumPointer2);
            quantum2.QuantumPointers.Add(quantumPointer1);

            if (!doRingify)
                return;

            Ringify(quantum1);

        }

        private static void Detach(Quantum quantum, Quantum reachableFrom)
        {
            foreach (QuantumPointer quantumPointer in quantum.QuantumPointers.Where(qp => qp.Quantum != reachableFrom))
            {
                QuantumPointer deletePointer = quantumPointer.Quantum.QuantumPointers.FirstOrDefault(qp => qp.Quantum == quantum);
                quantumPointer.Quantum.QuantumPointers.Remove(deletePointer);
            }
        }

        public static void Ringify(Quantum quantum)
        {
            Quantum reachableFrom = GetReachablePoint(quantum);

            if (reachableFrom != null)
            {
                Detach(quantum, reachableFrom);

                foreach (QuantumPointer quantumPointer in quantum.QuantumPointers)
                    Ringify(quantumPointer.Quantum);
            }
        }

        private static Quantum GetReachablePoint(Quantum quantum)
        {
            foreach (QuantumPointer quantumPointer in quantum.QuantumPointers)
            {
                Quantum reachableFrom = GetReachablePoint(quantum, quantum, quantumPointer.Quantum);

                if (reachableFrom != null)
                    return reachableFrom;
            }

            return null;
        }

        private static Quantum GetReachablePoint(Quantum srcQuantum, Quantum parentQuantum, Quantum childQuantum)
        {
            List<QuantumPointer> quantumPointersList = childQuantum.QuantumPointers.Where(qp => qp.Quantum != parentQuantum).ToList();

            foreach (QuantumPointer quantumPointer in quantumPointersList)
            {
                if (quantumPointer.Quantum == srcQuantum)
                    return parentQuantum;

                foreach (QuantumPointer innerQuantumPointer in quantumPointer.Quantum.QuantumPointers)
                {
                    Quantum reachableFrom = GetReachablePoint(srcQuantum, quantumPointer.Quantum, innerQuantumPointer.Quantum);

                    if (reachableFrom != null)
                        return reachableFrom;
                }
            }

            return null;
        }

        //public static void Ringify(Quantum quantum)
        //{
        //    if (IsReachable(quantum))
        //    {
        //        Detach(quantum);

        //        foreach (QuantumPointer quantumPointer in quantum.QuantumPointers)
        //            Ringify(quantumPointer.Quantum);
        //    }
        //}

        //private static bool IsReachable(Quantum quantum)
        //{
        //    foreach (QuantumPointer quantumPointer in quantum.QuantumPointers)            
        //        if (IsReachable(quantum, quantum, quantumPointer.Quantum))
        //            return true;            

        //    return false;
        //}

        //private static bool IsReachable(Quantum srcQuantum, Quantum parentQuantum, Quantum childQuantum)
        //{
        //    List<QuantumPointer> quantumPointersList = childQuantum.QuantumPointers.Where(qp => qp.Quantum != parentQuantum).ToList();

        //    foreach (QuantumPointer quantumPointer in quantumPointersList)
        //    {
        //        if (quantumPointer.Quantum == srcQuantum)
        //            return true;

        //        foreach (QuantumPointer innerQuantumPointer in quantumPointer.Quantum.QuantumPointers)                
        //            if (IsReachable(srcQuantum, quantumPointer.Quantum, innerQuantumPointer.Quantum))
        //                return true;
        //    }

        //    return false;
        //}

        public static void Collapse(Quantum quantum)  
        {
            foreach (QuantumPointer quantumPointer in quantum.QuantumPointers) 
            {
                Quantum referencedQuantum = quantumPointer.Quantum;

                QuantumPointer refQuantumPointer = referencedQuantum.QuantumPointers.FirstOrDefault(pointer => pointer.Quantum == quantum);

                referencedQuantum.QuantumPointers.Remove(refQuantumPointer);
            }

            quantum.QuantumPointers.Clear();
        }

        
        public static void Roll(Quantum quantum, double probabilityChange)
        {
            if (quantum == null)
                return;

            if (quantum.QuantumPointers.Count == 0)
                return;

            // сколько пришлось на каждую связь
            double probabilityChangePart = probabilityChange / quantum.QuantumPointers.Count;
            double shiftProbabilityAngle = probabilityChangePart * Angles._90degree; // угол вероятности изменения

            foreach (QuantumPointer quantumPointer in quantum.QuantumPointers) 
            {
                Quantum nextQuantum = quantumPointer.Quantum;

                double currentProbAngle = ProbabilityAngle(nextQuantum); // текущий угол кванта в углах вероятности
                
                double resultProbAngle = currentProbAngle + shiftProbabilityAngle; // получился угол вероятности в радианах

                nextQuantum.Angle = QuantumAngle(resultProbAngle);

            }
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
    }
}
