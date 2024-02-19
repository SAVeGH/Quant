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

    }
}
