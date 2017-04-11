namespace FluentBdd.UnitTests
{
    using System;
    using FluentAssertions;
    using Xunit;

    public class BddScenario_Tests
    {
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void Constructor(bool supressErrors)
        {
            var act = new Action(() => new BddScenario(scenarioName: "Scenario Name", suppressErrorsUntilEmitFailures: supressErrors));
            act.ShouldNotThrow();
        }

        [Fact]
        public void Get()
        {
            var inputObj = "MyInputVal";
            var key = "_key";

            var sut = new BddScenario("name");
            sut.Set(key: key, data: inputObj);

            var result = sut.Get<string>(key);
            result.Should().Should().NotBeNull();
            result.Should().Be(inputObj);
        }

        [Fact]
        public void Set()
        {
            var key = "_key";
            var inputObj = new BddScenario("fake");

            var sut = new BddScenario("name");

            var act = new Action(() => sut.Set(key: key, data: inputObj));

            act.ShouldNotThrow();
            sut.Get<BddScenario>(key).Should().NotBeNull();
        }

        [Fact]
        public void Given()
        {
            var sut = new BddScenario("name");
            var stepWasCalled = false;
            sut.Given("my step", (logger, context) =>
            {
                stepWasCalled = true;
            });

            stepWasCalled.Should().Be(true);
        }

        [Fact]
        public void Given_ErrorsNotSupressed()
        {
            var sut = new BddScenario("name");
            var stepWasCalled = false;
            var act = new Action(() =>
            {
                sut.Given("my step", (logger, context) =>
                {
                    stepWasCalled = true;
                    throw new Exception("boom");
                });
            });

            act.ShouldThrow<Exception>();
            stepWasCalled.Should().Be(true);
        }

        [Fact]
        public void And()
        {
            var sut = new BddScenario("name");
            var stepWasCalled = false;
            sut.And("my step", (logger, context) =>
            {
                stepWasCalled = true;
            });

            stepWasCalled.Should().Be(true);
        }

        [Fact]
        public void When()
        {
            var sut = new BddScenario("name");
            var stepWasCalled = false;
            sut.When("my step", (logger, context) =>
            {
                stepWasCalled = true;
            });

            stepWasCalled.Should().Be(true);
        }

        [Fact]
        public void Then()
        {
            var sut = new BddScenario("name");
            var stepWasCalled = false;
            sut.Then("my step", (logger, context) =>
            {
                stepWasCalled = true;
            });

            stepWasCalled.Should().Be(true);
        }

        [Fact]
        public void But()
        {
            var sut = new BddScenario("name");
            var stepWasCalled = false;
            sut.But("my step", (logger, context) =>
            {
                stepWasCalled = true;
            });

            stepWasCalled.Should().Be(true);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void EmitFailures(bool hasErrors)
        {
            var sut = new BddScenario(scenarioName: "scenario Name", suppressErrorsUntilEmitFailures: true);

            sut.Given(stepText: "some stuff", stepRunner: (logger, context) =>
            {
                if (hasErrors)
                {
                    throw new Exception("boom");
                }
            });

            var act = new Action(() =>
            {
                sut.EmitFailures();
            });

            if (hasErrors)
            {
                act.ShouldThrow<Exception>();
            }
            else
            {
                act.ShouldNotThrow<Exception>();
            }
        }

        [Fact]
        public void GetTextResults()
        {
            var sut = new BddScenario(scenarioName: "my scenario", suppressErrorsUntilEmitFailures: true);
            sut.Given(stepText: "Something is that way")
                .When("I do something")
                .Then("Something should happen", (logger, context) =>
                {
                    logger.Log("sup a log");
                    throw new Exception("boom", new Exception("inner exception"));
                });

            var result = sut.GetTextResult();

            result.Should().NotBeNullOrWhiteSpace();
        }

        [Fact]
        public void GetResult()
        {
            var sut = new BddScenario("my scneario", true);
            sut.Given(stepText: "Something is that way")
                .When("I do something")
                .Then("Something should happen", (logger, context) =>
                {
                    logger.Log("sup a log");
                    throw new Exception("boom", new Exception("inner exception"));
                })
                .And("Something Else");

            var result = sut.GetResult();
            result.Should().NotBeNull();
            result.StepResults.Count.Should().Be(4);
            sut.ScenarioName.Should().NotBeNullOrWhiteSpace();
        }
    }
}