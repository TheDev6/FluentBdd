namespace FluentBdd
{
    public interface IBddContext
    {
        string ScenarioName { get; }
        T Get<T>(string key);
        void Set(string key, object data);
    }
}
