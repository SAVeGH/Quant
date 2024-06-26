﻿using Qntm;
using Qntm.Helpers;

namespace QuantTest.StepDefinitions.Core
{
    [Binding]
    public class RingifyStepDefinitions
    {
        private ScenarioContext _scenarioContext;

        public RingifyStepDefinitions(ScenarioContext scenarioContext)
        {
            _scenarioContext = scenarioContext;
        }

        [Given(@"Ringify '([^']*)'")]
        [When(@"Ringify '([^']*)'")]
        public void WhenRingify(string a)
        {
            string qName = "Quantum_" + a;
            Quantum q = (Quantum)_scenarioContext[qName];

            EntangleHelper.Ringify(q);
        }

        [Then(@"Quantum '([^']*)' has one way reference to quantum '([^']*)'")]
        public void ThenQuantumHasOneWayReferenceToQuantum(string a, string b)
        {
            string qAName = "Quantum_" + a;
            string qBName = "Quantum_" + b;
            Quantum qA = (Quantum)_scenarioContext[qAName];
            Quantum qB = (Quantum)_scenarioContext[qBName];

            Assert.IsTrue(qA.QuantumPointers.Any(qp => qp.Quantum == qB));
            Assert.IsTrue(!qB.QuantumPointers.Any(qp => qp.Quantum == qA));
        }



    }
}
