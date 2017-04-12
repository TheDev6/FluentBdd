# FluentBdd
[Nuget](https://www.nuget.org/packages/FluentBdd/)  


### FluentBdd can assist in expressing, emitting, and logging gherkin sytax from automated tests.

I created FluentBdd as a lightweight less opinionated alternative to Specflow. Specflow is amazing but is built in-between (through code generation) the test and assertion code and the test framework without keeping up with MsTest|NUnit|Xunit features like parallel safe logging (as of early 2017) nor offering enough features to replace those frameworks. So FluentBdd simply works underneath MsTest|NUnit|Xunit to log and emit gherkin sytax. This doesn't pin FluentBdd to any other library feature or quickly changing patterns. The other fundimental difference is that of step code re-use. I chose to be explicit instead of reflecting over the attributes, which in my opinion makes it more readable and navicable to the code editor and related tools, like 'Find all usages'.


#### Quick Start:
This is example is inspired by [spec flow documentaion](http://specflow.org/getting-started/#AddingFeature)

```csharp
var bds = new BddScenario("Add two numbers")
              .Given("I have entered 50 into the calculator")
              .And("I have also entered 70 into the calculator")
              .When("I press add")
              .Then("the result should be 120 on the screen");
```

This of course won't do much because there is no actual testing or assertions happening but it pretty much defines the humble value of FluentBdd. It just matches up the gherkin syntax to some tests. The next example connects the actual steps and is included in the examples project.
```csharp
namespace Examples
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

```

##### Here is the output from a passing test:

Given I am a calculator user  
->pass  
And I am bad at math  
->pass  
When I enter the first number 2  
->pass  
And I enter the second number 4  
->pass  
And I call the add method  
->pass  
-->Logs:  
-->Pressing Add now..  
-->Add called.  
Then the result should be 6  
->pass  

Scenario Time:00:00:00.0000020

##### Here is the output from a failing test

Xunit.Sdk.XunitException
Expected value to be 4, but found 5.
   at FluentBdd.BddScenario.EmitFailures() in \GitHub\FluentBdd\src\FluentBdd\BddScenario.cs:line 163
   at Examples.CalculatorTests.AddTwoNumbers_ExampleVariation(Int32 first, Int32 second, Int32 expected) in \GitHub\FluentBdd\src\Examples\CalculatorTests.cs:line 86

Given I am a calculator user  
->pass  
And I am bad at math  
->pass  
When I enter the first number 3  
->pass  
And I enter the second number 2  
->pass  
And I call the add method  
->pass  
-->Logs:  
-->Pressing Add now..  
-->Add called.  
Then the result should be 4  
->fail  
->Expected value to be 4, but found 5.  

Scenario Time:00:00:00.2575123

###### The function driven style is inspired by jasmine and Karma javascript testing frameworks. It just seemed so obvious that Bdd perfectly fits with text + assertion function.

###### Please let me know if you find any mistakes or have questions or sugguestions. Thanks!
