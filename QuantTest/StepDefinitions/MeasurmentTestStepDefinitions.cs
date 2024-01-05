using Qntm;
using Qntm.Constants;
using Qntm.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace QuantTest.StepDefinitions
{
    [Binding]
    public class MeasurmentTestStepDefinitions
    {
        private ScenarioContext _scenarioContext;

        public MeasurmentTestStepDefinitions(ScenarioContext scenarioContext)
        {
            _scenarioContext = scenarioContext;
        }

        [When(@"Quantum '([^']*)' is measured (.*) times in (.*) basis")]
        public void WhenQuantumIsMeasuredTimesInBasis(string a, int p1, double p2)
        {
            string qName = "Quantum_" + a;
            Quantum q = (Quantum)_scenarioContext[qName];

            double result = RunMeasurment(q, AngleHelper.DegreeToRadians(p2), p1);

            _scenarioContext["MeasurmentResult"] = result;
        }


        [Then(@"Probability corresponds to (.*) with deviation of (.*)")]
        public void ThenProbabilityCorrespondsToWithDeviationOf(double p0, int p1)
        {
            double measurmentResult = (double)_scenarioContext["MeasurmentResult"];

            double deviationPercent = Math.Abs(measurmentResult - p0) * 100.0;
            Console.WriteLine($"deviationPercent: {deviationPercent}");
            Assert.IsTrue(deviationPercent <= p1);
        }

        private double RunMeasurment(Quantum q, double measurmentAngle,int count)
        {
            int falses = 0;
            int trues = 0;
            double angle = q.Angle;

            for (int i = 0; i < count; i++)
            {
                if (MeasurmentHelper.Measure(q, measurmentAngle)) trues++; else falses++;
                q.Reset(angle); // восстановить состояние после измерения
                RandomDelay();
            }

            return (double)trues / (double)(falses + trues);
        }

        // просто 'тяжелая' операция с произвольным временем выполнения
        private void RandomDelay()
        {
            Random random = new Random();

            double result = 0;
            double a = 12346789.0 * random.NextDouble();
            double b = 12346789.0 * random.NextDouble() + 1.0;

            int count = random.Next(100, 1000);

            for (int i = 0; i < count; i++)
                result = a / b;
        }

    }
}
