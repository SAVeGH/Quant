using Qntm;
using Qntm.Constants;
using Qntm.Helpers;
using QuantTest.Tables;
using TechTalk.SpecFlow.Assist;

namespace QuantTest.StepDefinitions
{
    [Binding]
    public class ProbabilityTestStepDefinitions
    {
        private ScenarioContext _scenarioContext;

        public ProbabilityTestStepDefinitions(ScenarioContext scenarioContext)
        {
            _scenarioContext = scenarioContext;
        }

        [Given(@"System has quantums")]
        public void GivenSystemHasQuantums(Table table)
        {
            var qTable = table.CreateSet<QuantumTable>();

            foreach (var row in qTable) 
            {
                Quantum q = new Quantum(AngleHelper.DegreeToRadians(row.Angle));
                q.Name = row.Name;
                //q.Scale = row.Scale;
                string qName = "Quantum_" + row.Name;
                _scenarioContext[qName] = q;
            }            
        }

        [When(@"Define deviation from probability 1/2 after (.*) measurments of quantum '([^']*)'")]
        public void WhenDefineDeviationFromProbabilityAfterMeasurmentsOfQuantum(int p0, string a)
        {
            string qName = "Quantum_" + a;
            Quantum q = (Quantum)_scenarioContext[qName];

            double deviationSqrSum = 0.0;
            // выполняется пачка из 10 замеров сетов заданного размера
            // Для усреднения т.к. вероятностно отклонение от 1/2 пачки из 100 измерений может оказаться меьше отклонения пачки из 10000 зимерений
            for (int i = 0; i < 10; i++) 
            {
                double result = RunMeasurment(q, p0);

                double deviationSqr = Math.Pow((result - 0.5), 2);

                deviationSqrSum = deviationSqrSum + deviationSqr;

            }           

            string resultName = $"Quantum_{a}_DeviationSqr_after_{p0}_measurments";

            _scenarioContext[resultName] = deviationSqrSum;
        }

        [When(@"Make (.*) measurments of quantum '([^']*)'")]
        public void WhenMakeMeasurmentsOfQuant(int p0, string a)
        {
            string qName = "Quantum_" + a;
            Quantum q = (Quantum)_scenarioContext[qName];
            double result = RunMeasurment(q, p0);

            double deviationSqr = Math.Pow((result - 0.5), 2);

            string resultName = $"Quantum_{a}_DeviationSqr_after_{p0}_measurments";

            _scenarioContext[resultName] = deviationSqr;
        }

        [Then(@"Measurment deviation after (.*) measurments of quantum '([^']*)' is less than deviation after (.*) measurments of quantum '([^']*)'")]
        public void ThenMeasurmentDeviationAfterMeasurmentsOfQuantumIsLessThanDeviationAfterMeasurmentsOfQuantum(int p0, string a, int p2, string a3)
        {
            string lowerResultName = $"Quantum_{a}_DeviationSqr_after_{p0}_measurments";
            string higherResultName = $"Quantum_{a3}_DeviationSqr_after_{p2}_measurments";

            double lowerDeviation = (double)_scenarioContext[lowerResultName];
            double higherDeviation = (double)_scenarioContext[higherResultName];

            Assert.IsTrue(higherDeviation > lowerDeviation);
        }

        [When(@"Collect side results of (.*) measurment sets of qunat '([^']*)' size of (.*) measurments")]
        public void WhenCollectSideResultsOfMeasurmentSetsOfQunatSizeOfMeasurments(int p0, string a, int p2)
        {
            int setsAmount = p0;
            string qntName = a;
            int setSize = p2;
            string qName = "Quantum_" + qntName;
            Quantum q = (Quantum)_scenarioContext[qName];

            int trues = 0;
            int falses = 0;

            for (int i = 0; i < setsAmount; i++)
            {
                double unityProbability = RunMeasurment(q, setSize);
                double zeroProbability = 1 - unityProbability;
                
                if (unityProbability > 0.5) trues++;
                if (zeroProbability > 0.5) falses++;
            }

            double deviationPercent = (double)Math.Abs(trues - falses) / (double)setsAmount;

            _scenarioContext["SetsDeviationPercent"] = deviationPercent;
        }

        [Then(@"Balance deviation should not exceed (.*) percents")]
        public void ThenBalanceDeviationShouldNotExceedPercents(int p0)
        {
            double deviationPercent = (double)_scenarioContext["SetsDeviationPercent"];
            
            Console.WriteLine($"deviationPercent: {deviationPercent}");

            Assert.IsTrue(deviationPercent < (double)p0 / 100.0);
        }

        private double RunMeasurment(Quantum q, int count, bool doDelay = true)
        {
            int falses = 0;
            int trues = 0;

            for (int i = 0; i < count; i++)
            {
                if (MeasurmentHelper.Measure(q, 0)) trues++; else falses++;
                q.Reset(Angles._90degree); // восстановить состояние после измерения
                if(doDelay) RandomDelay();
            }
            // Деление trues покажет веротность единицы 
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
