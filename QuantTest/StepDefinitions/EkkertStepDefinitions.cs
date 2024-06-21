using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Qntm.Constants;
using Qntm.Helpers;
using Qntm;
using QuantTest.Helpers;
using System.Xml.Linq;

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

            _scenarioContext["blockSize"] = blockSize;
            _scenarioContext["AliceKeySequence"] = AliceKeySequence;
        }

        [Given(@"'([^']*)' chose basis for each bit in the key"), Scope(Tag = "Ekkert")]
        public void GivenChoseBasisForEachBitInTheKey(string name)
        {
            int length = (int)_scenarioContext["blockSize"];
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
            //int blockSize = (int)_scenarioContext["blockSize"];

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
            _scenarioContext["StreamSize"] = AliceKeySequence.Count;
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
