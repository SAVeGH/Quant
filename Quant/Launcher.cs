using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Qntm;
using Qntm.Constants;
using Qntm.Helpers;

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
                Quantum q = new Quantum(0); 
                 RunMeasurment(q);
               
            }
            finally 
            {
                QuantumThreadWorker.Stop();
            }
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
                    if (MeasurmentHelper.Measure(q, 240.0 * Angles._rad)) trues++; else falses++;
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
