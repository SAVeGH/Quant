using Qntm.Constants;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

            List<QuantumPointer> waysList = quantumTo.QuantumPointers.ToList();

            foreach (QuantumPointer pointer in waysList)
            {
                Ringify(quantumTo, pointer.Quantum);
            }

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

        //private static void Detach(Quantum quantum, Quantum reachableFrom, int level)
        //{
        //    Debug.WriteLine($"Detach {level}: Process quantum {quantum.Name} reacheble from {reachableFrom.Name}");

        //    List<QuantumPointer> quantumPointersList = quantum.QuantumPointers.Where(qp => qp.Quantum != reachableFrom).ToList();

        //    Debug.WriteLine($"Detach {level}: quantumPointersList {quantumPointersList.Count}");

        //    for (int i = quantumPointersList.Count - 1; i > -1; i--) 
        //    {
        //        QuantumPointer quantumPointer = quantumPointersList[i];
        //        QuantumPointer deletePointer = quantumPointer.Quantum.QuantumPointers.FirstOrDefault(qp => qp.Quantum == quantum);
        //        quantumPointer.Quantum.QuantumPointers.Remove(deletePointer);

        //        Debug.WriteLine($"Detach {level}: quantum {quantum.Name} removed from {quantumPointer.Quantum.Name} links");
        //        Debug.WriteLine($"Detach {level}: quantum {quantumPointer.Quantum.Name} links count {quantumPointer.Quantum.QuantumPointers.Count}");
        //    }
        //}

        //public static void Ringify(Quantum quantum, int level = 0)
        //{
        //    level++;

        //    Debug.WriteLine($"Ringify {level}: Process quantum {quantum.Name}");

        //    Quantum reachableFrom = GetReachablePoint(quantum);

        //    if (reachableFrom != null)
        //    {
        //        Detach(quantum, reachableFrom, level);

        //        for (int i = quantum.QuantumPointers.Count - 1; i > -1; i--)
        //        {
        //            QuantumPointer quantumPointer = quantum.QuantumPointers.ElementAt(i);

        //            Ringify(quantumPointer.Quantum, level);
        //        }
        //    }
        //    else { Debug.WriteLine($"Ringify {level}: quantum: {quantum.Name} is not reacheble"); }
        //}

        //private static Quantum GetReachablePoint(Quantum quantum)
        //{
        //    for (int i = quantum.QuantumPointers.Count - 1; i > -1; i--) 
        //    {
        //        QuantumPointer quantumPointer = quantum.QuantumPointers.ElementAt(i);

        //        Quantum reachableFrom = GetReachablePoint(quantum, quantum, quantumPointer.Quantum);

        //        if (reachableFrom != null)
        //            return reachableFrom;
        //    }

        //    return null;
        //}

        //private static Quantum GetReachablePoint(Quantum srcQuantum, Quantum parentQuantum, Quantum childQuantum)
        //{
        //    List<QuantumPointer> quantumPointersList = childQuantum.QuantumPointers.Where(qp => qp.Quantum != parentQuantum).ToList();

        //    for (int i = quantumPointersList.Count - 1; i > -1; i--) 
        //    {
        //        QuantumPointer quantumPointer = quantumPointersList[i];

        //        if (quantumPointer.Quantum == srcQuantum)
        //            return parentQuantum;

        //        for (int j = quantumPointer.Quantum.QuantumPointers.Count - 1; j > -1; j--) 
        //        {
        //            QuantumPointer innerQuantumPointer = quantumPointer.Quantum.QuantumPointers.ElementAt(j);

        //            Quantum reachableFrom = GetReachablePoint(srcQuantum, quantumPointer.Quantum, innerQuantumPointer.Quantum);

        //            if (reachableFrom != null)
        //                return reachableFrom;
        //        }
        //    }

        //    return null;
        //}

        //public static void Ringify(Quantum quantum)
        //{
        //    Quantum reachableFrom = GetReachablePoint(quantum);

        //    if (reachableFrom != null)
        //    {
        //        Detach(quantum, reachableFrom);

        //        foreach (QuantumPointer quantumPointer in quantum.QuantumPointers)
        //            Ringify(quantumPointer.Quantum);
        //    }
        //}

        //private static Quantum GetReachablePoint(Quantum quantum)
        //{
        //    foreach (QuantumPointer quantumPointer in quantum.QuantumPointers)
        //    {
        //        Quantum reachableFrom = GetReachablePoint(quantum, quantum, quantumPointer.Quantum);

        //        if (reachableFrom != null)
        //            return reachableFrom;
        //    }

        //    return null;
        //}

        //private static Quantum GetReachablePoint(Quantum srcQuantum, Quantum parentQuantum, Quantum childQuantum)
        //{
        //    List<QuantumPointer> quantumPointersList = childQuantum.QuantumPointers.Where(qp => qp.Quantum != parentQuantum).ToList();

        //    foreach (QuantumPointer quantumPointer in quantumPointersList)
        //    {
        //        if (quantumPointer.Quantum == srcQuantum)
        //            return parentQuantum;

        //        foreach (QuantumPointer innerQuantumPointer in quantumPointer.Quantum.QuantumPointers)
        //        {
        //            Quantum reachableFrom = GetReachablePoint(srcQuantum, quantumPointer.Quantum, innerQuantumPointer.Quantum);

        //            if (reachableFrom != null)
        //                return reachableFrom;
        //        }
        //    }

        //    return null;
        //}

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
