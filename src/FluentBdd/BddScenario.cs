﻿namespace FluentBdd
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Text;

    public class BddScenario : IBddContext
    {
        private readonly List<BddStepResult> _stepResults;
        private readonly Dictionary<string, object> _context;
        private readonly bool _supressErrorsUntilEmitFailures;

        public BddScenario(string scenarioName, bool suppressErrorsUntilEmitFailures = false)
        {
            this.ScenarioName = scenarioName;
            this._stepResults = new List<BddStepResult>();
            this._context = new Dictionary<string, object>();
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

        public BddScenario Given(string stepText, Action<IBddStepLogger, IBddContext> stepRunner = null)
        {
            return this.RunStep(stepText: $"Given {stepText}", stepRunner: stepRunner);
        }

        public BddScenario And(string stepText, Action<IBddStepLogger, IBddContext> stepRunner = null)
        {
            return this.RunStep(stepText: $"And {stepText}", stepRunner: stepRunner);
        }

        public BddScenario When(string stepText, Action<IBddStepLogger, IBddContext> stepRunner = null)
        {
            return this.RunStep(stepText: $"When {stepText}", stepRunner: stepRunner);
        }

        public BddScenario Then(string stepText, Action<IBddStepLogger, IBddContext> stepRunner = null)
        {
            return this.RunStep(stepText: $"Then {stepText}", stepRunner: stepRunner);
        }

        public BddScenario But(string stepText, Action<IBddStepLogger, IBddContext> stepRunner = null)
        {
            return this.RunStep(stepText: $"But {stepText}", stepRunner: stepRunner);
        }
        private BddScenario RunStep(string stepText, Action<IBddStepLogger, IBddContext> stepRunner = null)
        {
            var stepResult = new BddStepResult(stepText: stepText, stepOrder: this._stepResults.Count + 1);
            var sw = new Stopwatch();
            sw.Start();
            if (this._stepResults.All(s => s.IsPass))
            {
                try
                {
                    stepRunner?.Invoke(stepResult, this);
                }
                catch (Exception ex)
                {
                    stepResult.FailException = ex;
                    if (!this._supressErrorsUntilEmitFailures)
                    {
                        throw;
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

        private List<Exception> FlattenException(Exception ex)
        {
            var result = new List<Exception>();
            if (ex != null)
            {
                result.Add(ex);
                if (ex.InnerException != null)
                {
                    result.AddRange(FlattenException(ex.InnerException));
                }
            }
            return result;
        }

        public string GetTextResult()
        {
            var sb = new StringBuilder();
            var arrow = "->";

            foreach (var sr in this._stepResults.OrderBy(s => s.StepOrder))
            {
                sb.Append($"{sr.StepText}");
                sb.Append(Environment.NewLine);

                sb.Append(arrow);
                sb.Append(sr.IsPass ? "pass" : "fail");
                if (!sr.IsPass)
                {
                    var dashes = "";
                    foreach (var ex in this.FlattenException(sr.FailException))
                    {
                        sb.Append(Environment.NewLine);
                        sb.Append($"{dashes}{arrow}{ex.Message}");
                        dashes += "-";
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

            TimeSpan ts = default(TimeSpan);
            foreach (var t in this._stepResults)
            {
                ts += t.StepTime;
            }
            sb.Append(Environment.NewLine);
            sb.Append($"Scenario Time:{ts}");
            return sb.ToString();
        }

        public void EmitFailures()
        {
            var ex = this._stepResults.FirstOrDefault(s => !s.IsPass && !(s.FailException is BddSkippedStepException));
            if (ex?.FailException != null)
            {
                throw ex.FailException;
            }
        }
    }
}
