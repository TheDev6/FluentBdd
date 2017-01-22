﻿namespace FluentBdd
{
    using System;
    using System.Collections.Generic;

    public class StepResult
    {
        public StepResult(string stepText, int stepOrder)
        {
            this.StepText = stepText;
            this.StepOrder = stepOrder;
            this.Logs = new List<string>();
        }
        public string StepText { get; private set; }
        public List<string> Logs { get; private set; }
        public int StepOrder { get; private set; }

        public void Log(string logMessage)
        {
            this.Logs.Add(logMessage);
        }

        public bool IsPass => this.FailException == null;

        public Exception FailException { get; set; }
    }
}