using System.Threading.Tasks;

namespace Machine.Specifications
{
  public class AwaitResult
  {
    readonly Task _task;

    public AwaitResult(Task task)
    {
      _task = task;
    }

    public Task AsTask
    {
      get { return _task; }
    }
  }

  public class AwaitResult<T>
  {
    readonly Task<T> _task;

    public AwaitResult(Task<T> task)
    {
      _task = task;
    }

    public Task<T> AsTask
    {
      get { return _task; }
    }

    public static implicit operator T(AwaitResult<T> m)
    {
      return m._task.Result;
    }
  }
}