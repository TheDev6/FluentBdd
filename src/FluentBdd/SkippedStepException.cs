namespace FluentBdd
{
    using System;

    public class SkippedStepException : Exception
    {
        public SkippedStepException() : base(message: "Step was skipped because of previous failure.") { }
        public SkippedStepException(Exception innerException) : base(message: "Step was skipped because of previous failure.", innerException: innerException) { }
    }
}
