using QuantTest.Helpers;
using Qntm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Qntm.Constants;
using System.Xml.Linq;
using Qntm.Helpers;

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

        [Then(@"StubThen")]
        public void ThenStubThen()
        {
            List<bool> AliceKeyBits = (List<bool>)_scenarioContext["AliceKeyBits"];
            List<bool> BobKeyBits = (List<bool>)_scenarioContext["BobKeyBits"];

            bool isIdentical = true;

            for (int i = 0; i < AliceKeyBits.Count; i++) 
            {
                if (AliceKeyBits[i] != BobKeyBits[i]) 
                {
                    isIdentical = false;
                    break;
                }
            }
        }



        [Given(@"Alice generates 4n size key where n is (.*)")]
        public void GivenAliceGenerates4NSizeKeyWhereNIs(int p0)
        {
            List<bool> AliceKeySequence = RandomSequence(p0);

            _scenarioContext["blockSize"] = p0;
            _scenarioContext["AliceKeySequence"] = AliceKeySequence;
        }

        [Given(@"'([^']*)' chose basis for each bit in the key")]
        public void GivenChoseBasisForEachBitInTheKey(string name)
        {
            int length = (int)_scenarioContext["blockSize"];

            List<bool> basisSequence = RandomSequence(length);
            _scenarioContext[$"{name}BasisSequence"] = basisSequence;
        }

        [When(@"Alice sends quantums stream to Bob")]
        public void WhenAliceSendsQuantumsStreamToBob()
        {
            //throw new PendingStepException();
        }

        [When(@"Bob measure quantums stream with chosen basises for each quantum")]
        public void WhenBobMeasureQuantumsStreamWithChosenBasisesForEachQuantum()
        {
            List<Quantum> AliceQuantums = (List<Quantum>)_scenarioContext["AliceQuantums"];
            List<bool> BobBasisSequence = (List<bool>)_scenarioContext["BobBasisSequence"];
            List<bool> BobKeySequence = new List<bool>();

            for (int i = 0; i < AliceQuantums.Count; i++) 
            {
                Quantum quantum = AliceQuantums[i];
                bool BobBasis = BobBasisSequence[i];
                double measurmentAngle = BobBasis ? Angles._0degree : Angles._90degree;
                bool mResult = MeasurmentHelper.Measure(quantum, measurmentAngle);
                BobKeySequence.Add(mResult);                
            }

            _scenarioContext["BobKeySequence"] = BobKeySequence;
        }

        [When(@"Alice and Bob compares their basises in unencripted form")]
        public void WhenAliceAndBobComparesTheirBasisesInUnencriptedForm()
        {
            List<bool> AliceBasisSequence = (List<bool>)_scenarioContext["AliceBasisSequence"];
            List<bool> BobBasisSequence = (List<bool>)_scenarioContext["BobBasisSequence"];

            List<bool> CommonBasisSequence = new List<bool>();

            for (int i = 0; i < AliceBasisSequence.Count; i++) 
            {
                bool AliceBasis = AliceBasisSequence[i];
                bool BobBasis = BobBasisSequence[i];

                bool isCoinside = AliceBasis == BobBasis;

                CommonBasisSequence.Add(isCoinside);
            }

            _scenarioContext["CommonBasisSequence"] = CommonBasisSequence;
        }

        [When(@"'([^']*)' leave key bits that corresponds to coinciding basises")]
        public void WhenLeaveKeyBitsThatCorrespondsToCoincidingBasises(string name)
        {
            List<bool> CommonBasisSequence = (List<bool>)_scenarioContext["CommonBasisSequence"];

            List<bool> keySequence = (List<bool>)_scenarioContext[$"{name}KeySequence"];

            List<bool> keyBits = new List<bool>();

            for (int i = 0; i < CommonBasisSequence.Count; i++) 
            {
                if (CommonBasisSequence[i] == false)
                    continue;

                keyBits.Add(keySequence[i]);
            }

            _scenarioContext[$"{name}KeyBits"] = keyBits;
        }


        [Given(@"Alice makes quantums stream")]
        public void GivenAliceMakesQuantumsStream()
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
