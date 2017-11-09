﻿namespace Examples.CatSearch
{
    using System.IO;
    using System.Linq;
    using System.Threading;
    using FluentAssertions;
    using FluentBdd;
    using OpenQA.Selenium;
    using OpenQA.Selenium.Chrome;
    using Xunit;
    using Xunit.Abstractions;

    public class CatSearchTests
    {
        private readonly BddFeatureResult _featureResult = new BddFeatureResult(
            name: "CatSearch",
            storyText: @"As a user of the interwebz
	                    I want to search for cats
	                    So that I can laugh at them");

        private readonly string _keySearchPage = "key_Searchpage";
        private readonly string _keySearchResults = "key_SearchResults";

        private readonly ITestOutputHelper _logger;

        public CatSearchTests(ITestOutputHelper logger)
        {
            _logger = logger;
        }

        public void Log(string logMessage)
        {
            this._logger.WriteLine(logMessage);
        }

        [Theory(DisplayName = "Search google for cats")]
        [Trait(name: "Category", value: "mytag")]
        [Xunit.InlineData("Brown Cats")]
        [Xunit.InlineData("Funny Cats")]
        public void Searchgoogleforcats(string searchTerm)
        {
            var bds = new BddScenario(
                scenarioName: "Search google for cats",
                suppressErrorsUntilEmitFailures: true);

            bds.Given("I am connected to the internet", this.GivenIamconnectedtotheinternet)
                .And("I navigate to google.com", this.AndInavigatetogoogledotcom)
                .When($"I type {searchTerm}", (logger, context) => this.WhenItypeSearchTerm(logger: logger, bddContext: context, searchTerm: searchTerm))
                .And("click enter", this.Andclickenter)
                .Then("cats are displayed", this.Thencatsaredisplayed)
                .And("I laugh at them", this.AndIlaughatthem);

            this.Log(bds.GetTextResult());

            bds.EmitFailures();
        }

        #region StepMethods
        public void GivenIamconnectedtotheinternet(IBddStepLogger logger, IBddContext bddContext)
        {
            //Should we check internet access?
        }
        public void AndInavigatetogoogledotcom(IBddStepLogger logger, IBddContext bddContext)
        {
            var searchPage = new GoogleSearchPage(driver: this.CreateLocalChromeDriver(), url: "https://www.google.com");
            bddContext.Set(key: this._keySearchPage, data: searchPage);
        }
        public void WhenItypeSearchTerm(IBddStepLogger logger, IBddContext bddContext, string searchTerm)
        {
            var searchPage = bddContext.Get<GoogleSearchPage>(this._keySearchPage);
            searchPage.EnterSearchTerm(searchTerm);
        }
        public void Andclickenter(IBddStepLogger logger, IBddContext bddContext)
        {
            var searchPage = bddContext.Get<GoogleSearchPage>(this._keySearchPage);
            var results = searchPage.ClickEnter();
            bddContext.Set(key: this._keySearchResults, data: results);
        }
        public void Thencatsaredisplayed(IBddStepLogger logger, IBddContext bddContext)
        {
            var searchPage = bddContext.Get<GoogleSearchPage>(this._keySearchPage);
            searchPage.SearchFieldElement.Should().NotBeNull(because: "There should be cats displayed");
        }
        public void AndIlaughatthem(IBddStepLogger logger, IBddContext bddContext)
        {
            "Ha ha ha".Should().NotBeNullOrEmpty();
            var searchPage = bddContext.Get<GoogleSearchPage>(this._keySearchPage);
            Thread.Sleep(3000);//so we can actually laugh at them
            searchPage.Quit();
        }

        private IWebDriver CreateLocalChromeDriver()
        {
            //TODO use chrome from path or build dynamic path
            //TODO why can't we relative path to the chrome driver from packages???????
            var chromeDriver = new ChromeDriver(ChromeDriverService.CreateDefaultService(@"J:\Documents\Projects\GitHub\FluentBdd\src\packages\Selenium.WebDriver.ChromeDriver.2.33.0\driver\win32"));
            return chromeDriver;
        }

        //private string BuildPathToDriver(string pathFromPackageToDriver)
        //{
        //    var assem = System.Reflection.Assembly.GetExecutingAssembly();
        //    var fullPath = assem.Location;
        //    fullPath = fullPath.Replace(fullPath.Split('\\').Last(), "");
        //    var relativePathToPackages = "..\\..\\..\\packages\\";
        //    var result = Path.GetFullPath(Path.Combine(path1: fullPath, path2: Path.Combine(path1: relativePathToPackages, path2: pathFromPackageToDriver)));
        //    return result;
        //}
        #endregion

    }
}