namespace Examples.CatSearch
{
    using OpenQA.Selenium;
    using OpenQA.Selenium.Support.PageObjects;

    public class SearchResultsPage
    {
        private readonly IWebDriver _driver;

        public SearchResultsPage(IWebDriver driver)
        {
            this._driver = driver;
            PageFactory.InitElements(this._driver, this);
        }

        [FindsBy(How = How.CssSelector, Using = "#ires > #rso")]
        public IWebElement SearchResultContainer { get; private set; }
    }
}