namespace Examples
{
    using FluentAssertions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using FluentBdd;

    [TestClass]
    public class CalculatorTests
    {
        private readonly BddFeature _feature = new BddFeature(
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
                .And("I Enter 2", logger => bs.Get<Calculator>(calcKey).EnterSecondNum(2))
                .And("I press Add", logger => bs.Set(resultKey, bs.Get<Calculator>(calcKey).Add()));
            bs.Then(stepText: $"the result is {expect}", stepRunner: logger =>
            {
                logger("Hey this a useful message about this step.");
                bs.Get<int>(resultKey).Should().Be(expect);
            });

            this._feature.BddScenarioResults.Add(bs.GetResult());
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

            this._feature.BddScenarioResults.Add(bdd.GetResult());
            var textOutput = bdd.GetTextResult();
        }

        [TestCleanup]
        public void TestCleanup()
        {
            //Emit the test results to a test result location. Or just let the test results bubble up.
        }
    }
}
