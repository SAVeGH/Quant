using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Qntm;
using Qntm.Constants;
using Qntm.Helpers;
using Qntm.Functions;
using Qntm.Gates;

namespace Quant
{
	public class Launcher
	{
        public void Run()
        {
            try
            {
                QuantumThreadWorker.Run(129);

                //Quantum q = new Quantum(Angles._0degree/*Math.PI / 4.0)*/);
                //Quantum q = new Quantum(48.1897 * Angles._rad);
                //q.Name = "A";

                DeutchTest();

                //Quantum.ShiftProbability1(q, 1.0 / 6.0, Angles._180degree);


            }
            finally
            {
                QuantumThreadWorker.Stop();
            }
        }

        public void DeutchTest() 
        {
            QuantumThreadWorker.Run(129);

            //Quantum q = new Quantum(Angles._0degree/*Math.PI / 4.0)*/);
            Quantum q = new Quantum(Angles._0degree);
            q.Name = "A";
            Quantum q1 = new Quantum(Angles._0degree);
            q1.Name = "B";

            Gates.H(q);
            Gates.X(q1);
            Gates.H(q1);

            EntangleHelper.Entangle(q, q1);

            DeuthQuantumFunction qf = new DeuthQuantumFunction(DeutchFunctions.ConstFalse);
            DeuthQuantumFunction qt = new DeuthQuantumFunction(DeutchFunctions.ConstTrue);
            DeuthQuantumFunction qid = new DeuthQuantumFunction(DeutchFunctions.BalancedId);
            DeuthQuantumFunction qnot = new DeuthQuantumFunction(DeutchFunctions.BalancedNot);

            double qff = qf.CallFunction(q);
            double qft = qt.CallFunction(q);
            double qfid = qid.CallFunction(q);
            double qfnot = qnot.CallFunction(q);

            //Console.WriteLine($"false: {qft}");

            Console.WriteLine($"false: {qff}, true: {qft}, id: {qfid}, not: {qfnot}");

        }


        public void Run111()
		{
            try
            {
                QuantumThreadWorker.Run(129);

                //Quantum q = new Quantum(Angles._0degree/*Math.PI / 4.0)*/);
                Quantum q = new Quantum(0); 
                 RunMeasurment2();
               
            }
            finally 
            {
                QuantumThreadWorker.Stop();
            }
        }

        private void RunMeasurment2()
        {

            Quantum q = new Quantum(0);

            Console.WriteLine(MeasurmentHelper.Measure(q, Angles._0degree));

            //q.Angle = Angles._270degree;

            Console.WriteLine(MeasurmentHelper.Measure(q, Angles._0degree));



        }

        private void RunMeasurment1(Quantum q)
        {
            


            while (Console.ReadKey(true).Key != ConsoleKey.Q)
            {


                double qAngle = q.Angle;

                //Console.WriteLine(MeasurmentHelper.Measure(q, - Angles._90degree));
                //Console.WriteLine(MeasurmentHelper.Measure(q, Angles._rad * 240));
                Console.WriteLine(MeasurmentHelper.Measure(q, Angles._0degree));

                q.Reset(qAngle);

            }

        }

        private void RunMeasurment(Quantum q)
        {
            Random r = new Random();


            while (Console.ReadKey(true).Key != ConsoleKey.Q)
            {
                int falses = 0;
                int trues = 0;

                for (int i = 0; i < 1000; i++)
                {
                    double qAngle = q.Angle;
                    if (MeasurmentHelper.Measure(q, 90.0 * Angles._rad)) trues++; else falses++;
                    q.Reset(qAngle);
                    //Thread.Sleep(r.Next(0, 30));
                }
                double fr = (double)falses / (double)(falses + trues);
                double tr = (double)trues / (double)(falses + trues);
                Console.WriteLine("False: " + fr.ToString() + " True: " + tr.ToString());

            }

        }
    }
}
