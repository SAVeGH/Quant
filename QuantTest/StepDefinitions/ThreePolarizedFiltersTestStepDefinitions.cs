using Qntm;
using Qntm.Constants;
using Qntm.Helpers;

namespace QuantTest.StepDefinitions
{
    [Binding]
    public class ThreePolarizedFiltersTestStepDefinitions
    {
        private ScenarioContext _scenarioContext;

        public ThreePolarizedFiltersTestStepDefinitions(ScenarioContext scenarioContext)
        {
            _scenarioContext = scenarioContext;
        }

        [Given(@"System has quantums stream of (.*) quantums with arbitrary state")]
        public void GivenSystemHasQuantumsStreamOfQuantumsWithArbitraryState(int p0)
        {
            List<Quantum> quantumsStream = new List<Quantum>();

            Random randomAngle = new Random();

            for (int i = 0; i < p0; i++)            
                quantumsStream.Add(new Quantum(Angles._360degree * randomAngle.NextDouble()));

            _scenarioContext["quantumsStream"] = quantumsStream;
            _scenarioContext["quantumsStreamCount"] = quantumsStream.Count;
        }

        [Given(@"Quantums stream fall on polarization filter set up with angle (.*)")]
        public void GivenQuantumsStreamFallOnPolarizationFilterSetUpWithAngle(int p0)
        {
            List<Quantum> quantumsStream = (List<Quantum>)_scenarioContext["quantumsStream"];
            List<Quantum> passedStream = new List<Quantum>();

            // фильтры ставятся с углами 0-90 что соответствует измерениям с углами 0-180 - поэтому умножаем угол измерения на 2
            double measurmentAngle = ((double)(p0 * 2)) * Angles._rad;
            double filterAngle = ((double)p0) * Angles._rad;

            foreach (Quantum quantum in quantumsStream) 
            {
                MeasurmentHelper.Measure(quantum, measurmentAngle);

                // если угол кванта после измерения лег на ось пропускания фильтра - квант проходит фильтр
                // угол мог лечь на ось и в обратном направлении ( 0 и -0 или 1 и -1) поэтому учитывем и направление оси фильтра +180 градусов
                if(quantum.Angle == filterAngle || quantum.Angle == (filterAngle + Angles._180degree))
                    passedStream.Add(quantum);
            }

            _scenarioContext["quantumsStream"] = passedStream;
        }

        [When(@"Detection after the filters is performed")]
        public void WhenDetectionAfterTheFiltersIsPerformed()
        {
            //throw new PendingStepException();
        }

        [Then(@"There is (.*) part of stream is passed with deviation of (.*)")]
        public void ThenThereIsPartOfStreamIsPassedWithDeviationOf(double p0, int p1)
        {
            List<Quantum> passedStream = (List<Quantum>)_scenarioContext["quantumsStream"];
            int initialCount = (int)_scenarioContext["quantumsStreamCount"];

            double part = (double)passedStream.Count / (double)initialCount;

            double deviationPercent = Math.Abs(part - p0) * 100.0 / (p0 == 0.0 ? 1.0 : p0);

            Assert.IsTrue(deviationPercent <= p1);
        }
    }
}
