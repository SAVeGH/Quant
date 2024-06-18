using Qntm;
using Qntm.Helpers;
using QuantTest.Helpers;

namespace QuantTest.StepDefinitions.Core
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
            Console.WriteLine($"deviationPercent: {deviationPercent}, Measurement result: {measurmentResult}");
            Assert.IsTrue(deviationPercent <= p1);
        }

        private double RunMeasurment(Quantum q, double measurmentAngle, int count)
        {
            int falses = 0;
            int trues = 0;
            double angle = q.Angle;

            for (int i = 0; i < count; i++)
            {
                if (MeasurmentHelper.Measure(q, measurmentAngle)) trues++; else falses++;
                q.Reset(angle); // восстановить состояние после измерения
                NutJob();
            }

            return trues / (double)(falses + trues);
        }

        // просто 'тяжелая' операция с произвольным временем выполнения
        private static void NutJob()
        {            
            Random random = RandomHelper.Create();

            double result = 0;
            double a = 123456789.0 * random.NextDouble();
            double b = 123456789.0 * random.NextDouble() + 1.0;

            int count = random.Next(1000, 10000);

            for (int i = 0; i < count; i++)
                result = result + a / b;
        }

    }
}
