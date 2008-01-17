using System;
using System.Collections.Generic;

namespace Machine.BackgroundJobs.Sample
{
  [BackgroundJob(typeof(LongRunningJobHandler), typeof(LongRunningJobRepository))]
  public class LongRunningJob : IBackgroundJob
  {
    private Int32 _jobNumber;
    private bool _isComplete;
    private bool _isStarted;
    private double _percentageComplete;
    private string _statusMessage;
    private TimeSpan _runFor;

    public int JobNumber
    {
      get { return _jobNumber; }
      set { _jobNumber = value; }
    }

    public bool IsComplete
    {
      get { return _isComplete; }
      set { _isComplete = value; }
    }

    public bool IsStarted
    {
      get { return _isStarted; }
      set { _isStarted = value; }
    }

    public double PercentageComplete
    {
      get { return _percentageComplete; }
      set { _percentageComplete = value; }
    }

    public string StatusMessage
    {
      get { return _statusMessage; }
      set { _statusMessage = value; }
    }

    public TimeSpan RunFor
    {
      get { return _runFor; }
      set { _runFor = value; }
    }
  }
}
