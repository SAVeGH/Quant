using Qntm;
using Qntm.Constants;
using Qntm.Helpers;
using System.Xml.Linq;

namespace QuantTest.StepDefinitions.Core
{
    [Binding]
    public class EntangledStatesTestStepDefinitions
    {
        private ScenarioContext _scenarioContext;

        public EntangledStatesTestStepDefinitions(ScenarioContext scenarioContext)
        {
            _scenarioContext = scenarioContext;
        }

        [Given(@"Quantums '([^']*)' and '([^']*)' are entangled")]
        public void GivenQuantumsAndAreEntangled(string a, string b)
        {
            string qAName = "Quantum_" + a;
            Quantum qA = (Quantum)_scenarioContext[qAName];

            string qBName = "Quantum_" + b;
            Quantum qB = (Quantum)_scenarioContext[qBName];

            EntangleHelper.Entangle(qA, qB);
        }

        [Given(@"Quantums '([^']*)' and '([^']*)' are entangled inverse")]
        public void GivenQuantumsAndAreEntangledInverse(string a, string b)
        {
            string qAName = "Quantum_" + a;
            Quantum qA = (Quantum)_scenarioContext[qAName];

            string qBName = "Quantum_" + b;
            Quantum qB = (Quantum)_scenarioContext[qBName];

            EntangleHelper.Entangle(qA, qB, isInverse: true);
        }


        [When(@"Measure quantum '([^']*)'")]
        public void WhenMeasureQuantum(string a)
        {
            string qName = "Quantum_" + a;
            Quantum q = (Quantum)_scenarioContext[qName];
        }

        [When(@"Measure quantum '([^']*)' in basis (.*)")]
        public void WhenMeasureQuantumInBasis(string name, double qBasis)
        {
            string qName = $"Quantum_{name}";
            Quantum q = (Quantum)_scenarioContext[qName];

            bool qResult = MeasurmentHelper.Measure(q, qBasis);

            string qResultName = $"Quantum_{name}_Result";
            _scenarioContext[qResultName] = qResult;
        }

        //[When(@"Measure (.*) times quantum '([^']*)' in basis (.*)")]
        //public void WhenMeasureTimesQuantumInBasis(int mTimes, string name, double basis)
        //{
        //    string qName = "Quantum_" + name;
        //    Quantum q = (Quantum)_scenarioContext[qName];

        //    double qAngle = q.Angle;
        //    int trueCounts = 0;

        //    bool qResult = MeasurmentHelper.Measure(q, basis);

        //    string qResultName = "Quantum_" + name + "_Result";
        //    _scenarioContext[qResultName] = qResult;

        //    q.Reset(qAngle);

        //    for (int i = 0; i < mTimes; i++) 
        //    {
        //        qResult = MeasurmentHelper.Measure(q, basis);

        //        if (qResult) trueCounts++;

        //        q.Reset(qAngle);
        //    }

        //    double resProbability = (double)trueCounts / (double)mTimes;

        //    string qResultProbabilityName = "Quantum_" + name + "_ResultProbability";
        //    _scenarioContext[qResultProbabilityName] = resProbability;
        //}


        [When(@"Measure to '([^']*)' quantum '([^']*)' in basis (.*)")]
        public void WhenMeasureToQuantumInBasis(string mResult, string name, double p2)
        {
            string qName = $"Quantum_{name}";
            Quantum q = (Quantum)_scenarioContext[qName];

            bool mRes = Convert.ToBoolean(mResult);

            bool qResult = MeasurmentHelper.MeasureTo(q, p2, mRes);

            string qResultName = $"Quantum_{name}_Result";
            _scenarioContext[qResultName] = qResult;
        }

        [Then(@"Measurment result of quantum '([^']*)' is '([^']*)' to measurment result of quantum '([^']*)'")]
        public void ThenMeasurmentResultOfQuantumIsToMeasurmentResultOfQuantum(string a, string match, string b)
        {
            string qAResultName = "Quantum_" + a + "_Result";
            bool qAResult = (bool)_scenarioContext[qAResultName];

            string qBResultName = "Quantum_" + b + "_Result";
            bool qBResult = (bool)_scenarioContext[qBResultName];

            if (match == "match")
                Assert.IsTrue(qAResult == qBResult);
            else
                Assert.IsFalse(qAResult == qBResult);
        }


        [Then(@"Measurment result corresponds to W state")]
        public void ThenMeasurmentResultCorrespondsToWState()
        {
            string qAResultName = "Quantum_A_Result";
            bool qAResult = (bool)_scenarioContext[qAResultName];

            string qBResultName = "Quantum_B_Result";
            bool qBResult = (bool)_scenarioContext[qBResultName];

            string qCResultName = "Quantum_C_Result";
            bool qCResult = (bool)_scenarioContext[qCResultName];

            int a = qAResult ? 1 : 0;
            int b = qBResult ? 1 : 0;
            int c = qCResult ? 1 : 0;

            int result = a + b + c;

            Assert.IsTrue(result == 1);
        }

        //[Then(@"Quantum '([^']*)' measurment result corresponds to (.*) state with (.*) percent deviation")]
        //public void ThenQuantumMeasurmentResultCorrespondsToStateWithPercentDeviation(string qName, double qUnityProbability, double deviationPercent)
        //{
        //    string qResultProbabilityName = "Quantum_" + qName + "_ResultProbability";
        //    double resProbability = (double)_scenarioContext[qResultProbabilityName];

        //    double measuredDeviationPercent = Math.Abs(resProbability - qUnityProbability) / 100.0;

        //    double allowedDeviationPercent = deviationPercent / 100.0;

        //    Assert.IsTrue(measuredDeviationPercent <= allowedDeviationPercent);
        //}

        [Then(@"State '([^']*)' is (.*) and '([^']*)' is (.*) does not exists")]
        public void ThenStateIsAndIsDoesNotExists(string aName, int aState, string bName, int bState)
        {
            string qAResultName = $"Quantum_{aName}_Result";
            string qBResultName = $"Quantum_{bName}_Result";

            bool qAResult = (bool)_scenarioContext[qAResultName];
            bool qBResult = (bool)_scenarioContext[qBResultName];

            bool aResultState = Convert.ToBoolean(aState);
            bool bResultState = Convert.ToBoolean(bState);

            bool disallowedSateExists = qAResult == aResultState && qBResult == bResultState;

            Assert.IsTrue(!disallowedSateExists);
        }
    }
}
