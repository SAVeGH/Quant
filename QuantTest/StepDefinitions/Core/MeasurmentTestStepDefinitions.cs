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
        public void WhenQuantumIsMeasuredTimesInBasis(string name, int timesCount, double qBasis)
        {
            string qName = $"Quantum_{name}";
            Quantum q = (Quantum)_scenarioContext[qName];

            double unityPobabilityResult = RunMeasurment(q, AngleHelper.DegreeToRadians(qBasis), timesCount);

            _scenarioContext[$"{name}_MeasurmentResult"] = unityPobabilityResult;
        }

        [Then(@"Quantum '([^']*)' probability corresponds to (.*) with deviation of (.*)")]
        public void ThenQuantumProbabilityCorrespondsToWithDeviationOf(string qName, double qProbability, int qDeviationPercent)
        {
            double measurmentResult = (double)_scenarioContext[$"{qName}_MeasurmentResult"];

            double deviationPercent = Math.Abs(measurmentResult - qProbability) * 100.0;
            Console.WriteLine($"deviationPercent: {deviationPercent}, Measurement result: {measurmentResult}");
            Assert.IsTrue(deviationPercent <= qDeviationPercent);
        }
        

        [When(@"Quantum '([^']*)' is measured to '([^']*)' in (.*) basis")]
        public void WhenQuantumIsMeasuredToInBasis(string name, string mResult, double basis)
        {
            string qName = $"Quantum_{name}";
            Quantum q = (Quantum)_scenarioContext[qName];

            bool measurmentResult = Convert.ToBoolean(mResult);

            MeasurmentHelper.MeasureTo(q, AngleHelper.DegreeToRadians(basis), measurmentResult);
        }

        [Then(@"Quantum '([^']*)' angle corresponds to (.*)")]
        public void ThenQuantumAngleCorrespondsTo(string name, double resultAngle)
        {
            string qName = $"Quantum_{name}";
            Quantum q = (Quantum)_scenarioContext[qName];

            double resAngle = AngleHelper.DegreeToRadians(resultAngle);

            Assert.IsTrue(q.Angle == resAngle);
        }






        //[Then(@"Probability corresponds to (.*) with deviation of (.*)")]
        //public void ThenProbabilityCorrespondsToWithDeviationOf(double p0, int p1)
        //{
        //    double measurmentResult = (double)_scenarioContext["MeasurmentResult"];

        //    double deviationPercent = Math.Abs(measurmentResult - p0) * 100.0;
        //    Console.WriteLine($"deviationPercent: {deviationPercent}, Measurement result: {measurmentResult}");
        //    Assert.IsTrue(deviationPercent <= p1);
        //}

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

            q.Reset(angle);

            return (double)trues / (double)(count);
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
