namespace FluentBdd
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Text;

    public class BddScenario
    {
        private readonly List<BddStepResult> _stepResults;
        private readonly Dictionary<string, object> _context;
        private readonly Action<string> _altLogger;
        private readonly bool _supressErrorsUntilEmitFailures;

        public BddScenario(string scenarioName, Action<string> altLogger = null, bool suppressErrorsUntilEmitFailures = false)
        {
            this.ScenarioName = scenarioName;
            this._stepResults = new List<BddStepResult>();
            this._context = new Dictionary<string, object>();
            this._altLogger = altLogger;
            this._supressErrorsUntilEmitFailures = suppressErrorsUntilEmitFailures;
        }

        public string ScenarioName { get; private set; }
        public T Get<T>(string key)
        {
            return (T)this._context[key];
        }
        public void Set(string key, object data)
        {
            this._context[key] = data;
        }

        public BddScenario Given(string stepText, Action<Action<string>> stepRunner = null)
        {
            return this.RunStep(stepText: $"Given {stepText}", stepRunner: stepRunner);
        }

        public BddScenario And(string stepText, Action<Action<string>> stepRunner = null)
        {
            return this.RunStep(stepText: $"And {stepText}", stepRunner: stepRunner);
        }

        public BddScenario When(string stepText, Action<Action<string>> stepRunner = null)
        {
            return this.RunStep(stepText: $"When {stepText}", stepRunner: stepRunner);
        }

        public BddScenario Then(string stepText, Action<Action<string>> stepRunner = null)
        {
            return this.RunStep(stepText: $"Then {stepText}", stepRunner: stepRunner);
        }

        private BddScenario RunStep(string stepText, Action<Action<string>> stepRunner = null)
        {
            var stepResult = new BddStepResult(stepText: stepText, stepOrder: this._stepResults.Count + 1);
            var sw = new Stopwatch();
            sw.Start();
            if (this._stepResults.All(s => s.IsPass))
            {
                try
                {
                    stepRunner?.Invoke(logMessage =>
                    {
                        stepResult.Log(logMessage);
                        this._altLogger?.Invoke(logMessage);
                    });
                }
                catch (Exception ex)
                {
                    var message = $"{stepResult.StepText} {ex.Message}";
                    stepResult.FailException = new Exception(message, ex);
                    if (!this._supressErrorsUntilEmitFailures)
                    {
                        throw ex;
                    }
                }
            }
            else
            {
                stepResult.FailException = new BddSkippedStepException();
            }
            sw.Stop();
            stepResult.StepTime = sw.Elapsed;
            this._stepResults.Add(stepResult);

            return this;
        }

        public BddScenarioResult GetResult()
        {
            return new BddScenarioResult(scenarioName: this.ScenarioName, stepResults: this._stepResults);
        }

        public string GetTextResult()
        {
            var sb = new StringBuilder();
            var arrow = "->";

            foreach (var sr in this._stepResults.OrderBy(s => s.StepOrder))
            {
                sb.Append($"{sr.StepText} - {sr.StepTime}");
                sb.Append(Environment.NewLine);

                sb.Append(arrow);
                sb.Append(sr.IsPass ? "pass" : "fail");
                if (!sr.IsPass)
                {
                    sb.Append(Environment.NewLine);
                    sb.Append($"{arrow}{sr.FailException?.Message ?? "failure exception was null. Please alert code author."}");
                    if (sr.FailException?.InnerException != null)
                    {
                        sb.Append(Environment.NewLine);
                        sb.Append(arrow);
                        sb.Append(sr.FailException.InnerException?.Message);
                    }
                }
                sb.Append(Environment.NewLine);

                if (sr.Logs.Any())
                {
                    sb.Append($"-{arrow}Logs:");
                    sb.Append(Environment.NewLine);
                    foreach (var l in sr.Logs)
                    {
                        sb.Append($"-{arrow}");
                        sb.Append(l);
                        sb.Append(Environment.NewLine);
                    }
                }
            }

            return sb.ToString();
        }

        public void EmitFailures()
        {
            var ex = this._stepResults.FirstOrDefault(s => !s.IsPass && !(s.FailException is BddSkippedStepException));
            if (ex != null)
            {
                throw ex.FailException;
            }
        }
    }
}
