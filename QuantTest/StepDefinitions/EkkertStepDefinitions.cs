using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Qntm.Constants;
using Qntm.Helpers;
using Qntm;
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

        [Given(@"Alice generates 3n size key where n is (.*)")]
        public void GivenAliceGenerates3NSizeKeyWhereNIs(int p0)
        {
            List<bool> AliceKeySequence = RandomHelper.RandomSequence(p0 * 3); //  количество будет p0 * 96 (8 бит по 4 байта 3 раза)

            _scenarioContext["blockSize"] = p0 * 3;
            _scenarioContext["AliceKeySequence"] = AliceKeySequence;

            Random randomAngle = RandomHelper.Create();

            List<Quantum> AliceStream = new List<Quantum>();
            List<Quantum> BobStream = new List<Quantum>();

            for (int i = 0; i < p0; i++)
            {
                double qAngle = Angles._360degree * randomAngle.NextDouble();

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
            _scenarioContext["StreamSize"] = p0;


        }
    }
}
