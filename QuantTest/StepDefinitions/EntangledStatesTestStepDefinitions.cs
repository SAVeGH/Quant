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

        [When(@"State is measured (.*) times in basis (.*) in order: '([^']*)','([^']*)','([^']*)'")]
        public void WhenStateIsMeasuredTimesInBasisInOrder(int p0, double p1, string a, string b, string c)
        {
            string qAName = "Quantum_" + a;
            Quantum qA = (Quantum)_scenarioContext[qAName];

            string qBName = "Quantum_" + b;
            Quantum qB = (Quantum)_scenarioContext[qBName];

            string qCName = "Quantum_" + c;
            Quantum qC = (Quantum)_scenarioContext[qCName];

            double AAngle = qA.Angle;
            double BAngle = qB.Angle;
            double CAngle = qC.Angle;

            int uzzCount = 0;
            int zuzCount = 0;
            int zzuCount = 0;

            for (int i = 0; i < p0; i++) 
            {
                bool aResult = MeasurmentHelper.Measure(qA, p1);
                bool bResult = MeasurmentHelper.Measure(qB, p1);
                bool cResult = MeasurmentHelper.Measure(qC, p1);

                if (aResult)
                {
                    if (bResult) 
                    {
                        
                    } 
                    else 
                    { 
                    }
                }
                else 
                {

                }


            }
        }

    }
}
