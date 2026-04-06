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

            // обмен квантов ссылками
            if (!quantum1.QuantumPointers.Any(qp => qp.Quantum == quantum2))
                quantum1.QuantumPointers.Add(quantumPointer2);

            if(!quantum2.QuantumPointers.Any(qp => qp.Quantum == quantum1))
                quantum2.QuantumPointers.Add(quantumPointer1);

            if (!doRingify)
                return;

            Ringify(quantum1);

        }

        /// <summary>
        /// Делает связи однонаправленными убирая обратые ссылки из квантов если они 
        /// достижимы другим путем (без обратной ссылки)
        /// </summary>
        /// <param name="quantum">квант в цепи</param>
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
                // если квант достижим без использования обратной ссылки - удалить ссылку на quantumFrom из списка quantumTo (удалить обратную ссылку)
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
            // кванты на которые ссылается данный квант - тут как минимум 1 ссылка будет т.к. запутанный квант обязательно на кого то ссылается
            List<Quantum> outLinks = quantum.QuantumPointers.Select(qp => qp.Quantum).ToList();
            // кванты которые ссылаются на данный квант
            List<Quantum> inLinks = GetReferencesList(quantum);

            // рекоммуникация квантов. Проход по все квантам на которые ссылался данный квант.
            // квантовая цепь не разрывется. Кванты которые ссылались на удаляемый квант будут ссылаться на все кванты на которые ссылался удаляемый квант (вместо удаляемого кванта).
            // кванты на которые ссылался удаляемый квант получат ссылки на все кванты которые ссылались на удаляемый квант.
            foreach (Quantum outQuantum in outLinks)
            {
                // квант на который сслыется удаляемый квант содержится в списке кванотв которые ссылаются на удаляемый квант - значит у кванта есть и прямая и обратная ссылка
                if (inLinks.Contains(outQuantum))
                {
                    // квант имел и прямую и обратную ссылку
                    // взять все кванты которые ссылаются на удаляемый квант (кроме себя - текущего кванта)
                    // добавить текущему кванту ссылки на все inLinks (т.е. на всех кто ссылался на удаляемый квант)
                    foreach (Quantum inQuantum in inLinks.Where(q => q != outQuantum)) 
                    {
                        if (outQuantum.QuantumPointers.Any(qp => qp.Quantum == inQuantum))
                            continue; // уже есть ссылка на inQuantum - не добавляем

                        QuantumPointer pointer = new QuantumPointer(inQuantum);

                        outQuantum.QuantumPointers.Add(pointer);
                    }
                }

                // раздать ссылку на outQuantum всем квантам которые ссылались на удаляемый квант (кроме себя - текущего кванта)
                // все inLinks должны получить ссылку на outQuantum    
                foreach (Quantum inQuantum in inLinks.Where(q => q != outQuantum))
                {
                    if (inQuantum.QuantumPointers.Any(qp => qp.Quantum == outQuantum))
                        continue;
                
                    QuantumPointer pointer = new QuantumPointer(outQuantum);
                
                    inQuantum.QuantumPointers.Add(pointer);
                }                
            }

            // кванты которые ссылаются на удаляемый квант но не содержатся в списке квантов на которые ссылался удаляемый квант - значит у них была только прямя ссылка на удаляемый квант
            foreach (Quantum inQuantum in inLinks.Except(outLinks)) 
            {
                foreach (Quantum outQuantum in outLinks) 
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

        // Проход по всем квантам цепи и сбор всех квантов которые ссылаются на quantum
        private static List<Quantum> GetReferencesList(Quantum quantum)
        {
            List<Quantum> referencesList = new List<Quantum>();

            foreach (QuantumPointer quantumPointer in quantum.QuantumPointers)            
                GetReferencesList(quantum, quantumPointer.Quantum, referencesList);

            return referencesList.Distinct().ToList();
        }

        /// <summary>
        /// Рекурсивно проходит по всем квантам цепи и собирает все квантоы которые ссылаются на quantumSearch
        /// </summary>
        /// <param name="quantumSearch"> квант ссылки на который ищем</param>
        /// <param name="quantum">квант в ссылка которого ищется quantumSearch</param>
        /// <param name="referencesList">массив с результатом поиска</param>
        /// <returns></returns>
        private static List<Quantum> GetReferencesList(Quantum quantumSearch, Quantum quantum, List<Quantum> referencesList)
        {
            if(quantum == quantumSearch)
                return referencesList; // нашел самого себя  - не добавляем в список ссылок

            if (referencesList.Contains(quantum))
                return referencesList; // уже добавили в список - не обрабатываем дальше

            foreach (QuantumPointer quantumPointer in quantum.QuantumPointers) 
            {
                if (quantumPointer.Quantum == quantumSearch)
                    referencesList.Add(quantum); // квант ссылается на quantumSearch - добавляем в список ссылок

                // рекрсивно обойти дальше по цепи
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

            // сколько пришлось на каждую связь (учет размера кванта Value)           
            Dictionary<QuantumPointer, double> probabilityChangeParts = QuantumsChangeParts(linksList);

            foreach (QuantumPointer quantumPointer in probabilityChangeParts.Keys)            
                RotateAngle(quantumPointer, probabilityChange * probabilityChangeParts[quantumPointer], basisAngle0);            

            passedList.AddRange(linksList.Select(qp => qp.Quantum));

            foreach (QuantumPointer quantumPointer in probabilityChangeParts.Keys)
            {
                Distribute(quantumPointer.Quantum, basisAngle0, probabilityChangeParts[quantumPointer], passedList);
            }
        }

        private static Dictionary<QuantumPointer, double> QuantumsChangeParts(List<QuantumPointer> linksList) 
        {
            Dictionary<QuantumPointer, double> changeParts = new Dictionary<QuantumPointer, double>();

            double qValuesSum = linksList.Sum(qp => qp.Quantum.Value); // полный размер всех квантов по ссылкам

            if(qValuesSum == 0)
                return changeParts;

            foreach (QuantumPointer quantumPointer in linksList) 
            {
                double qChangePart = quantumPointer.Quantum.Value / qValuesSum; // какая доля общего изменения пришлась на каждую связь
                                                                                // в зависимости от размера кванта
                changeParts[quantumPointer] = qChangePart;
            }

            return changeParts;
        }

        private static void RotateAngle(QuantumPointer quantumPointer, double probabilityChangePart, double basisAngle0) 
        {
            Quantum pointerQuantum = quantumPointer.Quantum;
            double connectionChangeSign = quantumPointer.IsInverse ? -1.0 : 1.0;

            probabilityChangePart = probabilityChangePart * connectionChangeSign; // учесть инверсию связи

            double unityProbability = ProbabilityHelper.UnityProbabilityInBasis(pointerQuantum.Angle, basisAngle0);

            bool? isZeroClockwise = ProbabilityHelper.IsZeroClockwise(pointerQuantum.Angle, basisAngle0);

            probabilityChangePart = (isZeroClockwise ?? false) ? probabilityChangePart : -probabilityChangePart;
            // поворот вероятности
            double resultProbability = unityProbability + probabilityChangePart;

            pointerQuantum.Angle = ProbabilityHelper.AngleOfProbabilityInBasis(resultProbability, basisAngle0);
        }

        public static string Grad(double rad)
        {
            return ((180.0 / Math.PI) * rad).ToString("0.000000");
        }

        //public static void RotateQuantum(Quantum quantum, double angle) 
        //{
        //    List<Quantum> passedList = new List<Quantum>();

        //    RotateQuantum(quantum, angle, passedList);
        //}

        //private static void RotateQuantum(Quantum quantum, double angle, List<Quantum> passedList)
        //{
        //    quantum.Angle = angle;

        //    if (quantum.QuantumPointers.Count == 0)
        //        return;

        //    passedList.Add(quantum);

        //    List<QuantumPointer> linksList = quantum.QuantumPointers.Where(qp => !passedList.Contains(qp.Quantum)).ToList();

        //    // сколько пришлось на каждую связь (учет размера кванта Value)           
        //    Dictionary<QuantumPointer, double> angleChangeParts = QuantumsChangeParts(linksList);

        //    foreach (QuantumPointer quantumPointer in probabilityChangeParts.Keys)
        //        RotateAngle(quantumPointer, probabilityChangeParts[quantumPointer], basisAngle0);

        //    passedList.AddRange(linksList.Select(qp => qp.Quantum));

        //    foreach (QuantumPointer quantumPointer in probabilityChangeParts.Keys)
        //    {
        //        Distribute(quantumPointer.Quantum, basisAngle0, probabilityChangeParts[quantumPointer], passedList);
        //    }


        //    //quantum.Angle = AngleHelper.Positive360RangeAngle(quantum.Angle + angle);
        //}
    }
}
