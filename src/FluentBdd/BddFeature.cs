namespace FluentBdd
{
    using System.Collections.Generic;

    public class BddFeature
    {
        public BddFeature(string name)
        {
            this.Name = name;
            this.BddScenarioResults = new List<BddScenarioResult>();
        }
        public string Name { get; private set; }
        public List<BddScenarioResult> BddScenarioResults { get; set; }
    }
}
