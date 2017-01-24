namespace Examples
{
    using FluentAssertions;
    using FluentBdd;
    using Xunit;
    using Xunit.Abstractions;

    public class CalculatorTests
    {
        private readonly BddFeatureResult _featureResult = new BddFeatureResult(
            name: "My Calculator Feature",
            storyText: @"As a person that is bad at math
              I can use an Add method to add two numbers
              So that I can get the answer");

        private readonly ITestOutputHelper _logger;

        public CalculatorTests(ITestOutputHelper logger)
        {
            this._logger = logger;
        }

        [Trait("Category", "AddTwoNumbers")]
        [Theory]
        [InlineData(2, 2, 4)]
        [InlineData(2, 3, 5)]
        [InlineData(3, 2, 4)]//wrong on purpose to see fail result
        [InlineData(2, 4, 6)]
        public void AddTwoNumbers(int first, int second, int expected)
        {
            var resultKey = "resultKey";
            var sut = new Calculator();

            var bds = new BddScenario(
                scenarioName: "Add two numbers",
                altLogger: this._logger.WriteLine,//log messages get added to the step result, but you can log to multiple outputs if needed.
                suppressErrorsUntilEmitFailures: true);//If you need to manage your own test output, this can be useful.

            bds.Given("I am a calculator user")
                .And("I am bad at math")
                .When($"I enter the first number {first}", logger => sut.EnterFirstNum(first))
                .And($"I enter the second number {second}", logger => sut.EnterSecondNum(second))
                .And("I call the add method", logger =>
                {
                    logger("clicking the add method");
                    var result = sut.Add();
                    bds.Set(key: resultKey, data: result);
                    logger("clicked the add method");
                })
                .Then($"the result should be {expected}", logger =>
                {
                    var result = bds.Get<int>(resultKey);
                    result.Should().Be(
                        expected: expected,
                        because: $"adding two numbers should emit the correct result of {expected}");
                });

            this._featureResult.BddScenarioResults.Add(bds.GetResult());//test results as objects allows transfer to another destination if 
            var textOutput = bds.GetTextResult();//could use this to log after the fact.
            bds.EmitFailures();//when suppressErrorsUntilEmitFailure is true, this is how you let the assertions bubble up to the test framework
        }
    }
}
