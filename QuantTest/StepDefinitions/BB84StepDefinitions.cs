using Qntm;
using Qntm.Constants;
using Qntm.Helpers;
using QuantTest.Helpers;

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

        [Given(@"Alice generates 4n size key where n is (.*)"), Scope(Tag = "BB84")]
        public void GivenAliceGenerates4NSizeKeyWhereNIs(int p0)
        {
            List<bool> AliceKeySequence = RandomHelper.RandomSequence(p0); //  количество будет p0 * 32 (8 бит по 4 байта)

            _scenarioContext["blockSize"] = p0;
            _scenarioContext["AliceKeySequence"] = AliceKeySequence;
        }

        [Scope(Tag = "BB84")]
        [Given(@"'([^']*)' chose basis for each bit in the key")]
        [When(@"'([^']*)' chose basis for each bit in the key")]
        public void GivenChoseBasisForEachBitInTheKey(string name)
        {
            int length = (int)_scenarioContext["blockSize"];

            List<bool> basisSequence = RandomHelper.RandomSequence(length);
            _scenarioContext[$"{name}BasisSequence"] = basisSequence;
        }

        [Given(@"Alice makes quantums stream"), Scope(Tag = "BB84")]
        public void GivenAliceMakesQuantumsStream()
        {
            List<Quantum> AliceQuantums = new List<Quantum>();

            List<bool> AliceKeySequence = (List<bool>)_scenarioContext["AliceKeySequence"];
            List<bool> AliceBasisSequence = (List<bool>)_scenarioContext["AliceBasisSequence"];

            for (int i = 0; i < AliceKeySequence.Count; i++)
            {
                bool keyValue = AliceKeySequence[i];
                bool basisValue = AliceBasisSequence[i]; // true - вертикальный базис V, false - горизонтальный базис H                

                Quantum quantum = MakeQuantum(keyValue, basisValue);

                AliceQuantums.Add(quantum);
            }

            _scenarioContext["AliceQuantums"] = AliceQuantums;
        }

        [When(@"Alice sends quantums stream to Bob"), Scope(Tag = "BB84")]
        public void WhenAliceSendsQuantumsStreamToBob()
        {
            //throw new PendingStepException();
        }

        [When(@"Bob measure quantums stream with chosen basises for each quantum"), Scope(Tag = "BB84")]
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

        [When(@"Alice and Bob compares their basises in unencripted form"), Scope(Tag = "BB84")]
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

        [When(@"'([^']*)' leave key bits that corresponds to coinciding basises"), Scope(Tag = "BB84")]
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

        [When(@"Alice and Bob compares one half of their key bits in unencripted form"), Scope(Tag = "BB84")]
        public void WhenAliceAndBobComparesOneHalfOfTheirKeyBitsInUnencriptedForm()
        {
            List<bool> AliceKeyBits = (List<bool>)_scenarioContext["AliceKeyBits"];
            List<bool> BobKeyBits = (List<bool>)_scenarioContext["BobKeyBits"];

            int blockLength = AliceKeyBits.Count / 2;

            List<bool> AliceKeyComparisionBits = AliceKeyBits.Skip(blockLength).ToList();
            List<bool> BobKeyComparisionBits = BobKeyBits.Skip(blockLength).ToList();

            bool isIdentical = true;

            for (int i = 0; i < AliceKeyComparisionBits.Count; i++)
            {
                if (AliceKeyComparisionBits[i] != BobKeyComparisionBits[i])
                {
                    isIdentical = false;
                    break;
                }
            }

            _scenarioContext["KeyBitsComparisionResult"] = isIdentical;
        }

        [Then(@"Compared key bits are identical"), Scope(Tag = "BB84")]
        public void ThenComparedKeyBitsAreIdentical()
        {
            bool isIdentical = (bool)_scenarioContext["KeyBitsComparisionResult"];

            Assert.IsTrue(isIdentical);
        }

        [Then(@"Alice and Bob keys are identical"), Scope(Tag = "BB84")]
        public void ThenAliceAndBobKeysAreIdentical()
        {            
            List<bool> AliceKeyBits = (List<bool>)_scenarioContext["AliceKeyBits"];
            List<bool> BobKeyBits = (List<bool>)_scenarioContext["BobKeyBits"];

            int blockLength = AliceKeyBits.Count / 2;

            List<bool> AliceKey = AliceKeyBits.Take(blockLength).ToList();
            List<bool> BobKey = BobKeyBits.Take(blockLength).ToList();

            bool isIdentical = true;

            for (int i = 0; i < AliceKey.Count; i++)
            {
                if (AliceKey[i] != BobKey[i])
                {
                    isIdentical = false;
                    break;
                }
            }

            Assert.IsTrue(isIdentical);
        }

        [When(@"Eva intercepts transmittion"), Scope(Tag = "BB84")]
        public void WhenEvaInterceptsTransmittion()
        {
            // Ева может еще не знает о выбранных базисах и результат их сравнения
            // Передается пока только пток квантов
            // Что бы узнать что передала Алиса  - нужно делать измерение

            List<Quantum> AliceQuantums = (List<Quantum>)_scenarioContext["AliceQuantums"];
            List<bool> EvaBasisSequence = (List<bool>)_scenarioContext["EvaBasisSequence"];

            for (int i = 0; i < AliceQuantums.Count; i++)
            {
                Quantum quantum = AliceQuantums[i];
                bool EvaBasis = EvaBasisSequence[i];
                double measurmentAngle = EvaBasis ? Angles._0degree : Angles._90degree;
                bool mResult = MeasurmentHelper.Measure(quantum, measurmentAngle);
            }
        }

        [Then(@"Compared key bits are not identical and differ for 1/4 with deviation of (.*)"), Scope(Tag = "BB84")]
        public void ThenComparedKeyBitsAreNotIdenticalAndDifferForWithDeviationOf(double p0)
        {
            bool isIdentical = (bool)_scenarioContext["KeyBitsComparisionResult"];

            Assert.IsFalse(isIdentical);

            List<bool> AliceKeyBits = (List<bool>)_scenarioContext["AliceKeyBits"];
            List<bool> BobKeyBits = (List<bool>)_scenarioContext["BobKeyBits"];

            int blockLength = AliceKeyBits.Count / 2;

            List<bool> AliceKeyComparisionBits = AliceKeyBits.Skip(blockLength).ToList();
            List<bool> BobKeyComparisionBits = BobKeyBits.Skip(blockLength).ToList();

            int diffCount = 0;

            for (int i = 0; i < AliceKeyComparisionBits.Count; i++)
            {
                if (AliceKeyComparisionBits[i] != BobKeyComparisionBits[i])
                    diffCount++;
            }

            double comparisionResult = (double)diffCount / (double)blockLength;

            double deviationPercent = Math.Abs(comparisionResult - 0.25) * 100.0 / 0.25;

            Assert.IsTrue(deviationPercent <= p0);
        }

        [Then(@"Alice and Bob keys are not identical"), Scope(Tag = "BB84")]
        public void ThenAliceAndBobKeysAreNotIdentical()
        {
            List<bool> AliceKeyBits = (List<bool>)_scenarioContext["AliceKeyBits"];
            List<bool> BobKeyBits = (List<bool>)_scenarioContext["BobKeyBits"];

            int blockLength = AliceKeyBits.Count / 2;

            List<bool> AliceKey = AliceKeyBits.Take(blockLength).ToList();
            List<bool> BobKey = BobKeyBits.Take(blockLength).ToList();

            bool isIdentical = true;

            for (int i = 0; i < AliceKey.Count; i++)
            {
                if (AliceKey[i] != BobKey[i])
                {
                    isIdentical = false;
                    break;
                }
            }

            Assert.IsFalse(isIdentical);
        }

        private Quantum MakeQuantum(bool keyValue, bool basisValue) 
        {
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

            return quantum;
        }
    }
}
