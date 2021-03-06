﻿namespace Examples
{
    using FluentAssertions;
    using FluentBdd;
    using Xunit;
    using Xunit.Abstractions;

    public class CalculatorTests
    {
        //The structure exists to buble up to a feature result.
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

        [Theory(DisplayName = "AddTwoNumbersVariation")]
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
                suppressErrorsUntilEmitFailures: true);//If you need to manage your own test output, this can be useful.

            bds.Given("I am a calculator user")
                .And("I am bad at math")
                .When($"I enter the first number {first}", (logger, context) => sut.EnterFirstNum(first))
                .And($"I enter the second number {second}", (logger, context) => sut.EnterSecondNum(second))
                .And("I call the add method", (logger, context) =>
                {
                    logger.Log("clicking the add method");
                    var result = sut.Add();
                    bds.Set(key: resultKey, data: result);
                    logger.Log("clicked the add method");
                })
                .Then($"the result should be {expected}", (logger, context) =>
                {
                    var result = bds.Get<int>(resultKey);
                    result.Should().Be(
                        expected: expected,
                        because: $"adding two numbers should emit the correct result of {expected}");
                });

            this._logger.WriteLine(bds.GetTextResult());

            bds.EmitFailures();//when suppressErrorsUntilEmitFailure is true, this is how you let the assertions bubble up to the test framework
        }

        [Theory(DisplayName = "AddTwoNumbers_ExampleVariation")]
        [InlineData(2, 2, 4)]
        [InlineData(2, 3, 5)]
        [InlineData(2, 4, 6)]
        [InlineData(3, 2, 4)] //wrong on purpose to see fail result
        public void AddTwoNumbers_ExampleVariation(int first, int second, int expected)
        {
            var bds = new BddScenario(
                scenarioName: "Add two numbers",
                suppressErrorsUntilEmitFailures: true);//was trying to show without this, but I like it!

            bds.Set("calc", new Calculator());//Subject Under Test
            bds.Set("first", first);
            bds.Set("second", second);
            bds.Set("expect", expected);

            bds.Given("I am a calculator user")
                .And("I am bad at math")
                .When($"I enter the first number {first}", this.EnterFirstNumber)
                .And($"I enter the second number {second}", this.EnterSecondNumber)
                .And("I call the add method", this.CallAdd)
                .Then($"the result should be {expected}", this.TheResultShouldBeExpected);

            this._logger.WriteLine(bds.GetTextResult());
            bds.EmitFailures();
        }

        private void EnterFirstNumber(IBddStepLogger logger, IBddContext bddContext)
        {
            bddContext.Get<Calculator>("calc").EnterFirstNum(bddContext.Get<int>("first"));
        }

        private void EnterSecondNumber(IBddStepLogger logger, IBddContext bddContext)
        {
            bddContext.Get<Calculator>("calc").EnterSecondNum(bddContext.Get<int>("second"));
        }

        private void CallAdd(IBddStepLogger logger, IBddContext bddContext)
        {
            logger.Log("Pressing Add now..");
            bddContext.Set("result", bddContext.Get<Calculator>("calc").Add());
            logger.Log("Add called.");
        }

        private void TheResultShouldBeExpected(IBddStepLogger logger, IBddContext bddContext)
        {
            bddContext.Get<int>("result").Should().Be(bddContext.Get<int>("expect"));
        }
    }
}
