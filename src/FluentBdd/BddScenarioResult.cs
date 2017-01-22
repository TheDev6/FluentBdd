namespace FluentBdd
{
    using System.Collections.Generic;

    public class BddScenarioResult
    {
        public BddScenarioResult(string scenarioName, List<StepResult> stepResults)
        {
            this.ScenarioName = scenarioName;
            this.StepResults = stepResults;
        }
        public string ScenarioName { get; private set; }
        public List<StepResult> StepResults { get; private set; }
    }
}
