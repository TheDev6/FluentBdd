# FluentBdd


### FluentBdd can assist in expressing, emitting, and logging gherkin sytax from automated tests.

I created BluentBdd as a lightweight less opinionated alternative to Specflow. Specflow is amazing but is built in-between (through code generation) the test and assertion code and the test framework without keeping up with MsTest|NUnit|Xunit features like parallel safe logging (as of early 2017) nor offering enough features to replace those frameworks. So FluentBdd simply works underneath MsTest|NUnit|Xunit to log and emit gherkin sytax. This doesn't pin FluentBdd to any other library feature or quickly changing patterns. The other fundimental difference is that of step code re-use. I chose to be explicit instead of reflecting over the attributes, which in my opinion makes it more readable and navicable to the code editor and related tools, like 'Find all usages'.


#### Quick Start:
This is example is inspired by [spec flow documentaion](http://specflow.org/getting-started/#AddingFeature)

```csharp
var bds = new BddScenario("Add two numbers")
                .Given("I have entered 50 into the calculator")
                .And("I have also entered 70 into the calculator")
                .When("I press add")
                .Then("the result should be 120 on the screen");
```

This of course won't do much because there is no actual testing or assertions happening but it pretty much defines the humble value of FluentBdd. It just servers to match up the business's words to some tests. The next example connects the actual steps.
```csharp
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
```
