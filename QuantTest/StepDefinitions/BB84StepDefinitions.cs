using QuantTest.Helpers;
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

        [When(@"StubWhen")]
        public void WhenStubWhen()
        {
            //throw new PendingStepException();
        }

        [Then(@"StubThen")]
        public void ThenStubThen()
        {
            //throw new PendingStepException();
        }



        [Given(@"Alice generates 4n size key where n is (.*)")]
        public void GivenAliceGenerates4NSizeKeyWhereNIs(int p0)
        {
            List<bool> key = new List<bool>();

            Random random = RandomHelper.Create();

            for (int i = 0; i < p0; i++)
            {
                int intValue = random.Next();

                byte[] intArray = BitConverter.GetBytes(intValue);

                for (int j = 0; j < 4; j++) 
                {
                    byte b = intArray[j];

                    WriteByteValue(b, key);
                }
            }
        }

        private void WriteByteValue(byte b, List<bool> key) 
        {
            for (int i = 0; i < 8; i++) 
            {
                bool bValue = (b & 1) == 1;
                key.Add(bValue);
                b = (byte)(b >> 1);
            }
        }

    }
}
