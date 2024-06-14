using Qntm.Constants;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Qntm.Helpers
{
    public static class EntangleHelper
    {
        public static void Entangle(Quantum quantum1, Quantum quantum2, bool isInverse = false, bool doRingify = false)
        {
            QuantumPointer quantumPointer1 = new QuantumPointer(quantum1) { IsInverse = isInverse };
            QuantumPointer quantumPointer2 = new QuantumPointer(quantum2) { IsInverse = isInverse };

            if (!quantum1.QuantumPointers.Any(qp => qp.Quantum == quantum2))
                quantum1.QuantumPointers.Add(quantumPointer2);

            if(!quantum2.QuantumPointers.Any(qp => qp.Quantum == quantum1))
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
            // кванты на которые ссылается квант - тут как минимум 1 ссылка будет т.к. запутанный квант обязательно на кого то ссылается
            List<Quantum> outLinks = quantum.QuantumPointers.Select(qp => qp.Quantum).ToList();
            // кванты которые ссылаются на квант
            List<Quantum> inLinks = GetReferencesList(quantum);
            
            foreach (Quantum outQuantum in outLinks)
            {
                if (inLinks.Contains(outQuantum))
                {
                    // квант имел и прямую и обратную ссылку
                    // взять все которые ссылаются на квант кроме себя
                    // добавить ссылки на все inLinks
                    foreach (Quantum inQuantum in inLinks.Where(q => q != outQuantum)) 
                    {
                        if (outQuantum.QuantumPointers.Any(qp => qp.Quantum == inQuantum))
                            continue;

                        QuantumPointer pointer = new QuantumPointer(inQuantum);

                        outQuantum.QuantumPointers.Add(pointer);
                    }
                }
               
                // все inLinks должны получить ссылку на outQuantum    
                foreach (Quantum inQuantum in inLinks.Where(q => q != outQuantum))
                {
                    if (inQuantum.QuantumPointers.Any(qp => qp.Quantum == outQuantum))
                        continue;
                
                    QuantumPointer pointer = new QuantumPointer(outQuantum);
                
                    inQuantum.QuantumPointers.Add(pointer);
                }                
            }

            // удалить все ссылки на квант в других квантах
            foreach (Quantum inQuantum in inLinks) 
            {
                QuantumPointer pointer = inQuantum.QuantumPointers.First(qp => qp.Quantum == quantum);

                inQuantum.QuantumPointers.Remove(pointer);
            }
            // удалить все ссылки кванта на другие кванты
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

            if(referencesList.Contains(quantum))
                return referencesList;

            foreach (QuantumPointer quantumPointer in quantum.QuantumPointers) 
            {
                if (quantumPointer.Quantum == quantumSearch)
                    referencesList.Add(quantum);

                GetReferencesList(quantumSearch, quantumPointer.Quantum, referencesList);
            }

            return referencesList;
        }

        public static void Distribute(Quantum quantum, double basisAngle0, double probabilityChange)
        {
            if (quantum == null)
                return;

            if (quantum.QuantumPointers.Count == 0)
                return;

            List<Quantum> passedList = new List<Quantum>();

            Distribute(quantum, basisAngle0, probabilityChange, passedList);
        }

        private static void Distribute(Quantum quantum, double basisAngle0, double probabilityChange, List<Quantum> passedList)
        {
            if (quantum == null)
                return;

            if (quantum.QuantumPointers.Count == 0)
                return;

            passedList.Add(quantum);

            List<QuantumPointer> linksList = quantum.QuantumPointers.Where(qp => !passedList.Contains(qp.Quantum)).ToList();

            // сколько пришлось на каждую связь
            double probabilityChangePart = probabilityChange / linksList.Count;            

            foreach (QuantumPointer quantumPointer in linksList)            
                RotateAngle(quantumPointer, probabilityChangePart, basisAngle0);            

            passedList.AddRange(linksList.Select(qp => qp.Quantum));

            foreach (QuantumPointer quantumPointer in linksList)
            {
                Distribute(quantumPointer.Quantum, basisAngle0, probabilityChangePart, passedList);
            }
        }

        private static void RotateAngle(QuantumPointer quantumPointer, double probabilityChangePart, double basisAngle0) 
        {
            Quantum pointerQuantum = quantumPointer.Quantum;
            double connectionChangeSign = quantumPointer.IsInverse ? -1.0 : 1.0;

            probabilityChangePart = probabilityChangePart * connectionChangeSign;

            double unityProbability = ProbabilityHelper.UnityProbabilityInBasis(pointerQuantum.Angle, basisAngle0);

            bool? isZeroClockwise = ProbabilityHelper.IsZeroClockwise(pointerQuantum.Angle, basisAngle0);

            probabilityChangePart = (isZeroClockwise ?? false) ? probabilityChangePart : -probabilityChangePart;

            double resultProbability = unityProbability + probabilityChangePart;

            pointerQuantum.Angle = ProbabilityHelper.AngleOfProbabilityInBasis(resultProbability, basisAngle0);
        }

        public static string Grad(double rad)
        {
            return ((180.0 / Math.PI) * rad).ToString("0.000000");
        }
    }
}
