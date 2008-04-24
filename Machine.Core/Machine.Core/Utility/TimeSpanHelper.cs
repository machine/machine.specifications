using System;
using System.Collections.Generic;
using System.Text;

namespace Machine.Core.Utility
{
  public static class TimeSpanHelper
  {
    public static string ToPrettyString(TimeSpan span)
    {
      StringBuilder sb = new StringBuilder();
      double delta = span.TotalSeconds;
      double OneMinute = 60;
      double OneHour = OneMinute * 60;
      double OneDay = 24 * OneHour;
      if (delta == 1)
      {
        return "a second";
      }
      else if (delta < OneMinute)
      {
        return span.Seconds + " seconds";
      }
      else if (delta < 2 * OneMinute)
      {
        return "about a minute";
      }
      else if (delta < (45 * OneMinute))
      {
        return span.Minutes + " minutes";
      }
      else if (delta < (90 * OneMinute))
      {
        return "about an hour";
      }
      else if (delta < OneDay)
      {
        return "about " + span.Hours + " hours";
      }
      else if (delta < (2 * OneDay))
      {
        return "1 day";
      }
      else if (delta < (14 * OneDay) )
      {
        return span.Days + " days";
      }
      else
      {
        return (Int32)(span.Days / 7.0) + " weeks";
      }
    }
  }
}
