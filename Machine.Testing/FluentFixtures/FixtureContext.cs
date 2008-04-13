namespace Machine.Testing.FluentFixtures
{
  public abstract class FixtureContext : IFixtureContext
  {
		private readonly NewService _new;
		private readonly IExistingService _existing;
		private readonly CurrentService _current;

		public IExistingService Existing
		{
			get { return _existing; }
		}

		public CurrentService Current
		{
			get { return _current; }
		}

		public NewService New
		{
			get { return _new; }
		}

    public abstract void Save<T>(T entity);
    public abstract void Flush();

    public FixtureContext(IExistingService existingsService)
    {
			_new = new NewService(this);
			_existing = existingsService;
			_current = new CurrentService();
    }
  }
}
