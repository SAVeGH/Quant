using Qntm;
using Qntm.Constants;
using Qntm.Helpers;
using TechTalk.SpecFlow;

namespace QuantTest.StepDefinitions
{
    [Binding]
    public class BellsInequalityStepsDefinitions
    {
        private ScenarioContext _scenarioContext;

        public BellsInequalityStepsDefinitions(ScenarioContext scenarioContext)
        {
            _scenarioContext = scenarioContext;
        }

        [Given(@"System has stream of entangled quantums size of (.*)")]
        public void GivenSystemHasStreamOfEntangledQuantumsSizeOf(int p0)
        {
            Guid guid = Guid.NewGuid();
            int seed = BitConverter.ToInt32(guid.ToByteArray());
            Random randomAngle = new Random(seed);

            List<Quantum> AliceStream = new List<Quantum>();
            List<Quantum> BobStream = new List<Quantum>();

            for (int i = 0; i < p0; i++)
            {
                //double qAngle = Angles._360degree * randomAngle.NextDouble();

                //Quantum quantumA = new Quantum(qAngle);
                //Quantum quantumB = new Quantum(qAngle);

                // странно, но это работает и без инверсии запутывания
                Quantum quantumA = new Quantum(Angles._360degree * randomAngle.NextDouble());
                Quantum quantumB = new Quantum(Angles._360degree * randomAngle.NextDouble());

                // если А измеринтся в 1 то В точно в 0 в том же базисе. Поэтому inverse
                EntangleHelper.Entangle(quantumA, quantumB, isInverse: true);

                AliceStream.Add(quantumA);
                BobStream.Add(quantumB);
            }

            _scenarioContext["AliceStream"] = AliceStream;
            _scenarioContext["BobStream"] = BobStream;
            _scenarioContext["StreamSize"] = p0;
        }

        [When(@"'([^']*)' has measured strem using arbitrary basises from three options basises")]
        public void WhenHasMeasuredStremUsingArbitraryBasisesFromThreeOptionsBasises(string name)
        {
            string AliceStreamName = $"AliceStream";
            string BobStreamName = $"BobStream";
            List<Quantum> AliceQuantumStream = (List<Quantum>)_scenarioContext[AliceStreamName];
            List<Quantum> BobQuantumStream = (List<Quantum>)_scenarioContext[BobStreamName];

            List<bool> AliceMeasurmentResults = new List<bool>();
            List<bool> BobMeasurmentResults = new List<bool>();

            Guid guid = Guid.NewGuid();
            byte[] bytes = guid.ToByteArray();
            int seed = BitConverter.ToInt32(bytes, 0);
            Random rnd = new Random(seed);

            foreach (Quantum item in AliceQuantumStream) 
            {
                double basisAngle = GetRandomBasisAngle(rnd);
                bool mResult = MeasurmentHelper.Measure(item, basisAngle);
                AliceMeasurmentResults.Add(mResult);
            }

            foreach (Quantum item in BobQuantumStream)
            {
                double basisAngle = GetRandomBasisAngle(rnd);
                bool mResult = MeasurmentHelper.Measure(item, basisAngle);
                BobMeasurmentResults.Add(mResult);
            }


            string AliceMeasurmentResultsName = $"AliceMeasurmentResults";
            _scenarioContext[AliceMeasurmentResultsName] = AliceMeasurmentResults;

            string BobMeasurmentResultsName = $"AliceMeasurmentResults";
            _scenarioContext[BobMeasurmentResultsName] = BobMeasurmentResults;
        }

        [When(@"Alice and Bob has measured strem using arbitrary basises from three options basises")]
        public void WhenAliceAndBobHasMeasuredStremUsingArbitraryBasisesFromThreeOptionsBasises()
        {
            string AliceStreamName = $"AliceStream";
            string BobStreamName = $"BobStream";
            List<Quantum> AliceQuantumStream = (List<Quantum>)_scenarioContext[AliceStreamName];
            List<Quantum> BobQuantumStream = (List<Quantum>)_scenarioContext[BobStreamName];

            List<bool> AliceMeasurmentResults = new List<bool>();
            List<bool> BobMeasurmentResults = new List<bool>();

            Guid guid = Guid.NewGuid();
            byte[] bytes = guid.ToByteArray();
            int seed = BitConverter.ToInt32(bytes, 0);
            Random rnd = new Random(seed);

            foreach (Quantum item in AliceQuantumStream)
            {
                double basisAngle = GetRandomBasisAngle(rnd);
                bool mResult = MeasurmentHelper.Measure(item, basisAngle);
                AliceMeasurmentResults.Add(mResult);
            }

            foreach (Quantum item in BobQuantumStream)
            {
                double basisAngle = GetRandomBasisAngle(rnd);
                bool mResult = MeasurmentHelper.Measure(item, basisAngle);
                BobMeasurmentResults.Add(mResult);
            }


            string AliceMeasurmentResultsName = $"AliceMeasurmentResults";
            _scenarioContext[AliceMeasurmentResultsName] = AliceMeasurmentResults;

            string BobMeasurmentResultsName = $"BobMeasurmentResults";
            _scenarioContext[BobMeasurmentResultsName] = BobMeasurmentResults;
        }


        private double GetRandomBasisAngle(Random rnd) 
        {
            int intValue = rnd.Next(0, 3);

            switch (intValue) 
            {
                case 0:
                    return Angles._0degree;
                case 1:
                    return Angles._120degree;
                case 2:
                    return Angles._240degree;
            }

            return 0;
        }

        //[Then(@"Comparision gives 1/2 of matched results")]
        //public void ThenComparisionGivesOfMatchedResults()
        //{
        //    List<bool> AliceMeasurmentResults = (List<bool>)_scenarioContext["AliceMeasurmentResults"];
        //    List<bool> BobMeasurmentResults = (List<bool>)_scenarioContext["BobMeasurmentResults"];

        //    List<bool> comparisionResults = new List<bool>();

        //    for (int i = 0; i < AliceMeasurmentResults.Count; i++) 
        //    {
        //        bool AliceResult = AliceMeasurmentResults[i];
        //        bool BobResult = BobMeasurmentResults[i];
        //        bool cmpResult = AliceResult == BobResult;

        //        comparisionResults.Add(cmpResult);
        //    }

        //    double complains = (double)comparisionResults.Count(item => item == true);

        //    double result = complains / (double)comparisionResults.Count;

        //    double deviationPercent = Math.Abs(result - 0.5) * 100.0;
        //    //Console.WriteLine($"deviationPercent: {deviationPercent}, Measurement result: {measurmentResult}");
        //    Assert.IsTrue(deviationPercent <= 10);


        //}

        [Then(@"Comparision gives 1/2 of matched results for quantum case rather than 5/9 for the classical case with devation of (.*) percents")]
        public void ThenComparisionGivesOfMatchedResultsForQuantumCaseRatherThanForTheClassicalCaseWithDevationOfPercents(double p0)
        {
            List<bool> AliceMeasurmentResults = (List<bool>)_scenarioContext["AliceMeasurmentResults"];
            List<bool> BobMeasurmentResults = (List<bool>)_scenarioContext["BobMeasurmentResults"];

            List<bool> comparisionResults = new List<bool>();

            for (int i = 0; i < AliceMeasurmentResults.Count; i++)
            {
                bool AliceResult = AliceMeasurmentResults[i];
                bool BobResult = BobMeasurmentResults[i];
                bool cmpResult = AliceResult == BobResult;

                comparisionResults.Add(cmpResult);
            }

            double complains = (double)comparisionResults.Count(item => item == true);

            double classicCaseExpectation = 5.0 / 9.0;
            double quantumCaseExpectation = 0.5;

            double result = complains / (double)comparisionResults.Count;

            // Отклонение от 1/2 меньше чем от 5/9
            Assert.IsTrue(Math.Abs(quantumCaseExpectation - result) < Math.Abs(classicCaseExpectation - result));

            //double deviationPercent = Math.Abs(result - 0.5) * 100.0;
            //Console.WriteLine($"deviationPercent: {deviationPercent}, Measurement result: {measurmentResult}");

            double deviationPercent = Math.Abs(quantumCaseExpectation - result) * 100.0 / quantumCaseExpectation;

            Assert.IsTrue(deviationPercent <= p0);
        }





    }
}
