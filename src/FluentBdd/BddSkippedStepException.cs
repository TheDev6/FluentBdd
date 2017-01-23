namespace FluentBdd
{
    using System;

    public class BddSkippedStepException : Exception
    {
        public BddSkippedStepException() : base(message: "Step was skipped because of previous failure.") { }
        public BddSkippedStepException(Exception innerException) : base(message: "Step was skipped because of previous failure.", innerException: innerException) { }
    }
}
