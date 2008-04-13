namespace Machine.Testing.FluentFixtures
{
  public interface IFixtureContext
  {
    IExistingService Existing { get; }
    CurrentService Current { get; }
    NewService New { get; }
    void Save<T>(T entity);
    void Flush();
  }
}