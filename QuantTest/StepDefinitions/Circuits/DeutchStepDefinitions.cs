using Qntm;
using Qntm.Constants;
using Qntm.Functions;
using Qntm.Gates;

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
        public void GivenGateHIsAppliedToQuantum(string name)
        {
            string qName = $"Quantum_{name}";
            Quantum q = (Quantum)_scenarioContext[qName];
            Gates.H(q);
        }

        [Given("Gate X is applied to quantum {string}")]
        public void GivenGateXIsAppliedToQuantum(string name)
        {
            string qName = $"Quantum_{name}";
            Quantum q = (Quantum)_scenarioContext[qName];
            Gates.X(q);
        }

        [When("Run Deutch circuit with quantum X as parameter and (.*) with (.*)")]
        public void WhenRunDeutchCircuitWithQuantumXAsParmeterAndFunctionNameWithOutput(string fnType, string fnOutput)
        {
            string qName = "Quantum_X";
            Quantum q = (Quantum)_scenarioContext[qName];

            Func<bool, bool> fnToCall = null;

            if (fnType == "constant")
            {
                if (fnOutput == "false")
                    fnToCall = DeutchFunctions.ConstFalse;
                else if (fnOutput == "true")
                    fnToCall = DeutchFunctions.ConstTrue;
            }
            else if (fnType == "balanced") 
            {
                if (fnOutput == "id")
                    fnToCall = DeutchFunctions.BalancedId;
                else if (fnOutput == "not")
                    fnToCall = DeutchFunctions.BalancedNot;
            }

            DeuthQuantumFunction deutchFunction = new DeuthQuantumFunction(fnToCall);

            double functionOutput = deutchFunction.CallFunction(q);

            if (functionOutput == Angles._0degree)
                _scenarioContext["DeutchFunctionResult"] = false;
            else if (functionOutput == Angles._180degree)
                _scenarioContext["DeutchFunctionResult"] = true;


        }

        [Then("Circuit output corresponds to (.*)")]
        public void ThenCircuitOutputCorrespondsToConstant(string fnType)
        {
            bool? expectedResult = fnType switch { "constant" => false, "balanced" => true, _ => null };
            bool actualResult = (bool)_scenarioContext["DeutchFunctionResult"];

            Assert.IsTrue(expectedResult == actualResult);
        }
    }
}
