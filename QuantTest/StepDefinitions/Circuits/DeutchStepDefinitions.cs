using Qntm;
using Qntm.Constants;
using Qntm.Helpers;
using QuantTest.Helpers;
using Qntm.Gates;
using Qntm.Functions;

namespace QuantTest.StepDefinitions.Circuits
{
    [Binding]
    public class DeutchStepDefinitions
    {
        private ScenarioContext _scenarioContext;

        public DeutchStepDefinitions(ScenarioContext scenarioContext)
        {
            _scenarioContext = scenarioContext;
        }

        [Given("Gate H is applied to quantum {string}")]
        [When("Gate H is applied to quantum {string}")]
        public void GivenGateHIsAppliedToQuantum(string qName)
        {
            throw new PendingStepException();
        }

    }
}
