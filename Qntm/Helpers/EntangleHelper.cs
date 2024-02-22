using Qntm.Constants;
using System;
using System.Collections.Generic;
using System.Linq;

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

        public static void Ringify(Quantum quantum) 
        {
            List<QuantumPointer> waysList = quantum.QuantumPointers.ToList();

            foreach (QuantumPointer pointer in waysList) 
            {
                Ringify(quantum, pointer.Quantum);
            }
        }

        public static void Ringify(Quantum quantumFrom, Quantum quantumTo)
        {
            if (!quantumTo.QuantumPointers.Any(qp => qp.Quantum == quantumFrom))
                return; // нет обратной ссылки

            if (IsReachable(quantumFrom, quantumFrom, quantumTo)) 
            {
                // удалить ссылку на quantumFrom из списка quantumTo
                QuantumPointer deletePointer = quantumTo.QuantumPointers.FirstOrDefault(qp => qp.Quantum == quantumFrom);
                quantumTo.QuantumPointers.Remove(deletePointer);
            }

            Ringify(quantumTo);
        }

        private static bool IsReachable(Quantum quantumSearch, Quantum quantumFrom, Quantum quantumTo) 
        {
            List<QuantumPointer> waysList = quantumTo.QuantumPointers.Where(qp => qp.Quantum != quantumFrom).ToList();

            foreach (QuantumPointer pointer in waysList) 
            {
                if (pointer.Quantum == quantumSearch)
                    return true;

                if (IsReachable(quantumSearch, quantumTo, pointer.Quantum))
                    return true;
            }

            return false;
        }

        public static void Collapse(Quantum quantum)
        {
            // кванты на которые ссылается квант
            List<Quantum> outLinks = quantum.QuantumPointers.Select(qp => qp.Quantum).ToList();
            // кванты которые ссылаются на квант
            List<Quantum> inLinks = GetReferencesList(quantum);

            foreach (Quantum outQuantum in outLinks)
            {

            }

            quantum.QuantumPointers.Clear();
        }

        private static List<Quantum> GetReferencesList(Quantum quantum)
        {
            List<Quantum> referencesList = new List<Quantum>();

            foreach (QuantumPointer quantumPointer in quantum.QuantumPointers)            
                GetReferencesList(quantum, quantumPointer.Quantum, referencesList);

            return referencesList.Distinct().ToList();
        }

        private static List<Quantum> GetReferencesList(Quantum quantumSearch, Quantum quantum, List<Quantum> referencesList)
        {
            if(quantum == quantumSearch)
                return referencesList;

            foreach (QuantumPointer quantumPointer in quantum.QuantumPointers) 
            {
                if (quantumPointer.Quantum == quantumSearch)
                    referencesList.Add(quantum);

                GetReferencesList(quantumSearch, quantumPointer.Quantum, referencesList);
            }

            return referencesList;
        }

        //public static void Collapse(Quantum quantum)  
        //{
        //    foreach (QuantumPointer quantumPointer in quantum.QuantumPointers) 
        //    {
        //        Quantum referencedQuantum = quantumPointer.Quantum;

        //        QuantumPointer refQuantumPointer = referencedQuantum.QuantumPointers.FirstOrDefault(pointer => pointer.Quantum == quantum);

        //        referencedQuantum.QuantumPointers.Remove(refQuantumPointer);
        //    }

        //    quantum.QuantumPointers.Clear();
        //}


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
