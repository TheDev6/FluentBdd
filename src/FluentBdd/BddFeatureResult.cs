namespace FluentBdd
{
    using System.Collections.Generic;

    public class BddFeatureResult
    {
        public BddFeatureResult(string name, string storyText)
        {
            this.Name = name;
            this.StoryText = storyText;
            this.BddScenarioResults = new List<BddScenarioResult>();
        }
        public string Name { get; private set; }
        public string StoryText { get; private set; }
        public List<BddScenarioResult> BddScenarioResults { get; set; }
    }
}
