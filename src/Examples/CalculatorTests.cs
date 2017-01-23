namespace Examples
{
    using FluentAssertions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using FluentBdd;

    [TestClass]
    public class CalculatorTests
    {
        private readonly BddFeatureResult _featureResult = new BddFeatureResult(
            name: "My Calculator Feature",
            storyText:
            @"As a person that is bad at math
              I can use an Add method to add two numbers
              So that I can get the answer");

        [TestMethod]
        public void AddTwoNumbers()
        {
            var calcKey = "calc";
            var resultKey = "result";
            var expect = 4;
            var bs = new BddScenario(scenarioName: "Add two numbers");
            bs.Set(calcKey, new Calculator());

            //able to for each here based on data, or theories/test case scenarios as needed

            bs.Given("I am a calculator user.");
            bs.When("I enter 2", logger => bs.Get<Calculator>(calcKey).EnterFirstNum(2))
                .And("I enter 2", logger => bs.Get<Calculator>(calcKey).EnterSecondNum(2))
                .When("I press Add", logger => bs.Set(resultKey, bs.Get<Calculator>(calcKey).Add()));
            bs.Then(stepText: $"the result is {expect}", stepRunner: logger =>
            {
                logger("Hey this a useful message about this step.");
                bs.Get<int>(resultKey).Should().Be(expect);
            });

            this._featureResult.BddScenarioResults.Add(bs.GetResult());
            var textOutput = bs.GetTextResult();
        }

        [TestMethod]
        public void AddTwoNumbers_Alternative()
        {
            //We can ignore the private dictionary and just 'close over' whatever variables we need to use across steps/methods.

            var firstNum = 1;
            var secNum = 1;
            var expect = 2;
            var result = default(int);
            var sut = new Calculator();
            var bs = new BddScenario(scenarioName: "Add two numbers");

            bs.Given("I am a calculator user.");
            bs.When("I enter 2", logger => sut.EnterFirstNum(firstNum))
                .And("I enter 2", logger => sut.EnterSecondNum(secNum))
                .And("I call Add", logger => { result = sut.Add(); });
            bs.Then(stepText: $"the result is {expect}", stepRunner: logger =>
            {
                logger("Hey this a useful message about this step.");
                result.Should().Be(expect);
            });

            this._featureResult.BddScenarioResults.Add(bs.GetResult());
            var textOutput = bs.GetTextResult();
        }

        [TestMethod]
        public void AddTwoNumbersFail()
        {
            var calcKey = "calc";
            var resultKey = "result";
            var firstNum = 2;
            var secondNum = 2;
            var expected = 5;//incorrect on purpose for error example

            var bdd = new BddScenario(scenarioName: "Add two numbers");
            bdd.Set(calcKey, new Calculator());

            bdd.Given("I am a calculator user.")
                .When($"I enter {firstNum}", logger => bdd.Get<Calculator>(calcKey).EnterFirstNum(firstNum))
                .And($"I Enter {secondNum}", logger => bdd.Get<Calculator>(calcKey).EnterSecondNum(secondNum))
                .And("I press Add", logger => bdd.Set(resultKey, bdd.Get<Calculator>(calcKey).Add()))
                .Then($"The result is {expected}", logger =>
                {
                    logger("Hey this a useful message about this step.");
                    bdd.Get<int>(resultKey).Should().Be(expected);
                });

            this._featureResult.BddScenarioResults.Add(bdd.GetResult());
            var textOutput = bdd.GetTextResult();
        }

        [TestMethod]
        public void AddTwoNumbers_Variation()
        {
            var firstNum = 1;
            var secNum = 1;
            var expect = 2;
            var result = default(int);
            var sut = new Calculator();
            var bs = new BddScenario(scenarioName: "Add two numbers");

            bs.Given("I am a calculator user.");
            bs.When($"I enter {firstNum}", logger => sut.EnterFirstNum(firstNum))
                .And($"I enter {secNum}", logger =>
                {
                    logger("");
                    sut.EnterSecondNum(secNum);
                    logger("post action log log.");
                    false.Should().Be(true);
                })
                .And("I call Add", logger => { result = sut.Add(); });
            bs.Then(stepText: $"the result is {expect}", stepRunner: logger =>
            {
                logger("Hey this a useful message about this step.");
                result.Should().Be(expect);
            });

            this._featureResult.BddScenarioResults.Add(bs.GetResult());
            var textOutput = bs.GetTextResult();
        }
    }
}
