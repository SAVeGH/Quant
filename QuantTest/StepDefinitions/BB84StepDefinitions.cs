using QuantTest.Helpers;
using Qntm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Qntm.Constants;

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
            List<bool> AliceKeySequence = RandomSequence(p0);

            _scenarioContext["blockSize"] = p0;
            _scenarioContext["AliceKeySequence"] = AliceKeySequence;
        }

        [Given(@"Alice chose basis for each bit in the key")]
        public void GivenAliceChoseBasisForEachBitInTheKey()
        {
            int length = (int)_scenarioContext["blockSize"];

            List<bool> AliceBasisSequence = RandomSequence(length);
            _scenarioContext["AliceBasisSequence"] = AliceBasisSequence;
        }

        [Given(@"'([^']*)' chose basis for each bit in the key")]
        public void GivenChoseBasisForEachBitInTheKey(string name)
        {
            int length = (int)_scenarioContext["blockSize"];

            List<bool> basisSequence = RandomSequence(length);
            _scenarioContext[$"{name}BasisSequence"] = basisSequence;
        }


        [Given(@"Alice makes transmittion quantums sequence")]
        public void GivenAliceMakesTransmittionQuantumsSequence()
        {
            int length = (int)_scenarioContext["blockSize"];

            List<Quantum> AliceQuantums = new List<Quantum>();

            List<bool> AliceKeySequence = (List<bool>)_scenarioContext["AliceKeySequence"];
            List<bool> AliceBasisSequence = (List<bool>)_scenarioContext["AliceBasisSequence"];

            for (int i = 0; i < AliceKeySequence.Count; i++) 
            {
                bool keyValue = AliceKeySequence[i];
                bool basisValue = AliceBasisSequence[i]; // true - вертикальный базис V, false - горизонтальный базис H
                double quantumAngle = double.NaN;

                if (keyValue)
                {
                    // отправляем 1
                    if (basisValue)
                    {
                        // V
                        quantumAngle = Angles._180degree;
                    }
                    else 
                    {
                        // H
                        quantumAngle = Angles._270degree;
                    }
                }
                else 
                {
                    // отправляем 0
                    if (basisValue)
                    {
                        // V
                        quantumAngle = Angles._0degree;
                    }
                    else 
                    {
                        // H
                        quantumAngle = Angles._90degree;
                    }
                }

                Quantum quantum = new Quantum(quantumAngle);

                AliceQuantums.Add(quantum);
            }

            _scenarioContext["AliceQuantums"] = AliceQuantums;
        }




        private List<bool> RandomSequence(int length) 
        {
            List<bool> sequence = new List<bool>();

            Random random = RandomHelper.Create();

            for (int i = 0; i < length; i++)
            {
                int intValue = random.Next();

                byte[] intArray = BitConverter.GetBytes(intValue);

                for (int j = 0; j < 4; j++)
                {
                    byte b = intArray[j];

                    WriteByteValue(b, sequence);
                }
            }

            return sequence;
        }

        private void WriteByteValue(byte b, List<bool> sequence) 
        {
            for (int i = 0; i < 8; i++) 
            {
                bool bValue = (b & 1) == 1;
                sequence.Add(bValue);
                b = (byte)(b >> 1);
            }
        }
    }
}
