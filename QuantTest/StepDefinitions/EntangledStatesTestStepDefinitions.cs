using Qntm;
using Qntm.Constants;
using Qntm.Helpers;

namespace QuantTest.StepDefinitions
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
        public void WhenMeasureQuantumInBasis(string a, double p1)
        {
            string qName = "Quantum_" + a;
            Quantum q = (Quantum)_scenarioContext[qName];

            bool qResult = MeasurmentHelper.Measure(q, p1);

            string qResultName = "Quantum_" + a + "_Result";
            _scenarioContext[qResultName] = qResult;
        }

        [When(@"Measure to '([^']*)' quantum '([^']*)' in basis (.*)")]
        public void WhenMeasureToQuantumInBasis(string mResult, string a, double p2)
        {
            string qName = "Quantum_" + a;
            Quantum q = (Quantum)_scenarioContext[qName];

            bool mRes = Convert.ToBoolean(mResult);

            bool qResult = MeasurmentHelper.MeasureTo(q, p2, mRes);

            string qResultName = "Quantum_" + a + "_Result";
            _scenarioContext[qResultName] = qResult;
        }

        [Then(@"Measurment result of quantum '([^']*)' is '([^']*)' to measurment result of quantum '([^']*)'")]
        public void ThenMeasurmentResultOfQuantumIsToMeasurmentResultOfQuantum(string a, string match, string b)
        {
            string qAResultName = "Quantum_" + a + "_Result";
            bool qAResult = (bool)_scenarioContext[qAResultName];

            string qBResultName = "Quantum_" + b + "_Result";
            bool qBResult = (bool)_scenarioContext[qBResultName];

            if(match == "match")
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


    }
}
