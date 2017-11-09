namespace Examples.CatSearch
{
    using OpenQA.Selenium;
    using OpenQA.Selenium.Support.PageObjects;

    public class GoogleSearchPage
    {
        private readonly IWebDriver _driver;

        public GoogleSearchPage(IWebDriver driver, string url)
        {
            this._driver = driver;
            this._driver.Navigate().GoToUrl(url);
            PageFactory.InitElements(this._driver, this);
        }

        [FindsBy(How = How.Id, Using = "lst-ib")]
        public IWebElement SearchFieldElement { get; private set; }

        public void EnterSearchTerm(string searchTerm)
        {
            this.SearchFieldElement.SendKeys(searchTerm);
        }

        public SearchResultsPage ClickEnter()
        {
            this.SearchFieldElement.SendKeys(Keys.Return);
            var searchResults = new SearchResultsPage(this._driver);
            return searchResults;
        }

        public void Quit()
        {
            this._driver.Quit();
        }
    }
}
