namespace FluentBdd
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class BddScenario
    {
        //TODO:
        //Would be really cool to be able to to treat the feature gherkin syntax as data so that we could run a method that helps solve the problem of externally updated specs.
        //If such a feature could be implemented it would answer the following questions. Of 'these' hand coded tests, which are orphans, and which have a matching binding from a data source (Gherkin feature file)?
        //Currently I'm working on a relational model of gherkin, but it would be arguably better to keep the original syntax and just parse it as data in order to compare to this kind of stuff.

        private readonly List<StepResult> _stepResults;
        private readonly Dictionary<string, object> _context;
        private readonly Action<string> _altLogger;

        public BddScenario(string scenarioName, Action<string> altLogger = null)
        {
            this.ScenarioName = scenarioName;
            this._stepResults = new List<StepResult>();
            this._context = new Dictionary<string, object>();
            this._altLogger = altLogger;
        }

        public string ScenarioName { get; private set; }

        public BddScenario Given(string stepText, Action<Action<string>> stepRunner = null)
        {
            return this.RunStep(stepText: stepText, stepRunner: stepRunner);
        }

        public BddScenario And(string stepText, Action<Action<string>> stepRunner = null)
        {
            return this.RunStep(stepText: stepText, stepRunner: stepRunner);
        }

        public BddScenario When(string stepText, Action<Action<string>> stepRunner = null)
        {
            return this.RunStep(stepText: stepText, stepRunner: stepRunner);
        }

        public BddScenario Then(string stepText, Action<Action<string>> stepRunner = null)
        {
            return this.RunStep(stepText: stepText, stepRunner: stepRunner);
        }

        private BddScenario RunStep(string stepText, Action<Action<string>> stepRunner = null)
        {
            var sr = new StepResult(stepText: stepText, stepOrder: this._stepResults.Count + 1);
            if (this._stepResults.All(s => s.IsPass))
            {
                try
                {
                    var logger = new Action<string>(logMessage =>
                    {
                        sr.Log(logMessage);
                        this._altLogger?.Invoke(logMessage);
                    });
                    stepRunner?.Invoke(logger);
                    this._stepResults.Add(sr);
                }
                catch (Exception ex)
                {
                    sr.FailException = ex;
                    this._stepResults.Add(sr);
                    //throw ex;
                    //TODO: Decide if we should throw now, or later :/ or have that be an option in order to accurately capture the results
                    //The test frameworks already have a path for this, BUT there should be a choice to divert and handle results as data and maybe ship to a test results crud system.
                    //I'm not sure yet what would be better???
                    //Maybe the question can be framed as when to use the underlying test framework for assertion reporting, or just handle it as data ??
                    //Can we do both.
                }
            }
            else
            {
                sr.FailException = new SkippedStepException();
                this._stepResults.Add(sr);
            }
            return this;
        }

        public T Get<T>(string key)
        {
            return (T)this._context[key];
        }
        public void Set(string key, object data)
        {
            this._context[key] = data;
        }

        public BddScenarioResult GetResult()
        {
            return new BddScenarioResult(scenarioName: this.ScenarioName, stepResults: this._stepResults);
        }
    }
}
