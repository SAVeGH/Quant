using Qntm;
using Qntm.Constants;
using Qntm.Helpers;
using QuantTest.Helpers;

namespace QuantTest.StepDefinitions
{
    [Binding]
    public class EkkertStepDefinitions
    {
        private ScenarioContext _scenarioContext;

        public EkkertStepDefinitions(ScenarioContext scenarioContext)
        {
            _scenarioContext = scenarioContext;
        }

        [Given(@"Alice generates 3n size key where n is (.*)"), Scope(Tag = "Ekkert")]
        public void GivenAliceGenerates3NSizeKeyWhereNIs(int p0)
        {
            int blockSize = p0 * 3;

            List<bool> AliceKeySequence = RandomHelper.RandomSequence(blockSize); //  количество будет p0 * 96 (8 бит по 4 байта 3 раза)

            _scenarioContext["KeySequenceSize"] = AliceKeySequence.Count;
            _scenarioContext["AliceKeySequence"] = AliceKeySequence;
        }

        [Given(@"'([^']*)' chose basis for each bit in the key"), Scope(Tag = "Ekkert")]
        public void GivenChoseBasisForEachBitInTheKey(string name)
        {
            int length = (int)_scenarioContext["KeySequenceSize"];
            List<double> basisSequence = new List<double>();
            Random random = RandomHelper.Create();

            for (int i = 0; i < length; i++) 
            {
                double basisAngle = GetRandomBasisAngle(random);
                basisSequence.Add(basisAngle);
            }
            
            _scenarioContext[$"{name}BasisSequence"] = basisSequence;
        }

        [Given(@"Alice makes quantums streams"), Scope(Tag = "Ekkert")]
        public void GivenAliceMakesQuantumsStream()
        {
            Random randomAngle = RandomHelper.Create();

            List<bool> AliceKeySequence = (List<bool>)_scenarioContext["AliceKeySequence"];

            List<Quantum> AliceStream = new List<Quantum>();
            List<Quantum> BobStream = new List<Quantum>();

            for (int i = 0; i < AliceKeySequence.Count; i++)
            {
                double qAngle = AliceKeySequence[i] ? Angles._180degree : Angles._0degree; // 1 or 0

                // Замечание: тест работает даже если задавать каждому кванту произвольный угол и затем запутывать.
                // Так же тест проходит если при запутывании ставить инверсию связи.

                // состояние Бэлла:
                // 1/sqrt(2)00> + 1/sqrt(2)11>
                Quantum quantumA = new Quantum(qAngle);
                Quantum quantumB = new Quantum(qAngle);

                EntangleHelper.Entangle(quantumA, quantumB);

                AliceStream.Add(quantumA);
                BobStream.Add(quantumB);
            }

            _scenarioContext["AliceStream"] = AliceStream;
            _scenarioContext["BobStream"] = BobStream;
        }

        [When(@"Alice sends quantums stream to Bob"), Scope(Tag = "Ekkert")]
        public void WhenAliceSendsQuantumsStreamToBob()
        {
            //throw new PendingStepException();
        }

        [When(@"'([^']*)' measure own quantums stream with chosen basises for each quantum"), Scope(Tag = "Ekkert")]
        public void WhenMeasureOwnQuantumsStreamWithChosenBasisesForEachQuantum(string name)
        {
            List<double> basisSequence = (List<double>)_scenarioContext[$"{name}BasisSequence"];
            List<Quantum> quantumStream = (List<Quantum>)_scenarioContext[$"{name}Stream"];
            List<bool> measurmentResults = new List<bool>();

            for (int i = 0; i < quantumStream.Count; i++) 
            {
                Quantum quantum = quantumStream[i];
                double basisAngle = basisSequence[i];
                bool mResult = MeasurmentHelper.Measure(quantum, basisAngle);
                measurmentResults.Add(mResult);
            }

            _scenarioContext[$"{name}MeasurmentResults"] = measurmentResults;
        }


        [When(@"Alice and Bob compares their basises in unencripted form"), Scope(Tag = "Ekkert")]
        public void WhenAliceAndBobComparesTheirBasisesInUnencriptedForm()
        {
            List<double> AliceBasisSequence = (List<double>)_scenarioContext["AliceBasisSequence"];
            List<double> BobBasisSequence = (List<double>)_scenarioContext["BobBasisSequence"];

            List<bool> CommonBasisSequence = new List<bool>();

            for (int i = 0; i < AliceBasisSequence.Count; i++)
            {
                double AliceBasis = AliceBasisSequence[i];
                double BobBasis = BobBasisSequence[i];

                bool isCoinside = AliceBasis == BobBasis;

                CommonBasisSequence.Add(isCoinside);
            }

            _scenarioContext["CommonBasisSequence"] = CommonBasisSequence;
        }

        [When(@"'([^']*)' leave key bits that corresponds to coinciding basises"), Scope(Tag = "Ekkert")]
        public void WhenLeaveKeyBitsThatCorrespondsToCoincidingBasises(string name)
        {
            List<bool> CommonBasisSequence = (List<bool>)_scenarioContext["CommonBasisSequence"];

            List<bool> measurmentResults = (List<bool>)_scenarioContext[$"{name}MeasurmentResults"];

            List<bool> keyBits = new List<bool>();

            for (int i = 0; i < CommonBasisSequence.Count; i++)
            {
                if (CommonBasisSequence[i] == false)
                    continue;

                keyBits.Add(measurmentResults[i]);
            }

            _scenarioContext[$"{name}KeyBits"] = keyBits;
        }

        [When(@"Alice and Bob compares key bits where basises differ in unencripted form"), Scope(Tag = "Ekkert")]
        public void WhenAliceAndBobComparesKeyBitsWhereBasisesDifferInUnencriptedForm()
        {
            List<bool> CommonBasisSequence = (List<bool>)_scenarioContext["CommonBasisSequence"];

            List<bool> AliceMeasurmentResults = (List<bool>)_scenarioContext["AliceMeasurmentResults"];
            List<bool> BobMeasurmentResults = (List<bool>)_scenarioContext["BobMeasurmentResults"];

            List<bool> AliceDifferentBasisBits = new List<bool>();
            List<bool> BobDifferentBasisBits = new List<bool>(); ;

            for (int i = 0; i < CommonBasisSequence.Count; i++)
            {
                if (CommonBasisSequence[i] == true)
                    continue;

                bool AliceBit = AliceMeasurmentResults[i];
                bool BobBit = BobMeasurmentResults[i];

                AliceDifferentBasisBits.Add(AliceBit);
                BobDifferentBasisBits.Add(BobBit);
            }

            _scenarioContext["AliceDifferentBasisBits"] = AliceDifferentBasisBits;
            _scenarioContext["BobDifferentBasisBits"] = BobDifferentBasisBits;
        }

        [Then(@"Compared key bits with different basises matches in (.*) cases with deviation (.*)"), Scope(Tag = "Ekkert")]
        public void ThenComparedKeyBitsWithDifferentBasisesMatchesInCasesWithDeviation(double p0, double p1)
        {
            int matchedCount = 0;

            List<bool> AliceDifferentBasisBits = (List<bool>)_scenarioContext["AliceDifferentBasisBits"];
            List<bool> BobDifferentBasisBits = (List<bool>)_scenarioContext["BobDifferentBasisBits"];

            for (int i = 0; i < AliceDifferentBasisBits.Count; i++)
            {
                if (AliceDifferentBasisBits[i] == BobDifferentBasisBits[i])
                    matchedCount++;
            }

            double comparisionResult = (double)matchedCount / (double)AliceDifferentBasisBits.Count;

            double deviationPercent = Math.Abs(comparisionResult - p0) * 100.0 / p0;

            Assert.IsTrue(deviationPercent <= p1);

        }

        [Then(@"Alice and Bob keys are identical"), Scope(Tag = "Ekkert")]
        public void ThenAliceAndBobKeysAreIdentical()
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

            Assert.IsTrue(isIdentical);
        }

        private double GetRandomBasisAngle(Random rnd)
        {
            int intValue = rnd.Next(0, 3);

            switch (intValue)
            {
                case 0:
                    return Angles._0degree;
                case 1:
                    return Angles._120degree;
                case 2:
                    return Angles._240degree;
            }

            return 0;
        }

    }
}
