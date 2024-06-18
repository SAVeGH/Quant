using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuantTest.StepDefinitions
{
    [Binding]
    public class BB84StepDefinitions
    {
        private ScenarioContext _scenarioContext;

        public BB84StepDefinitions(ScenarioContext scenarioContext)
        {
            _scenarioContext = scenarioContext;
        }

        [Given(@"Alice generates 4n size key where n is (.*)")]
        public void GivenAliceGenerates4NSizeKeyWhereNIs(int p0)
        {
            throw new PendingStepException();
        }

    }
}
