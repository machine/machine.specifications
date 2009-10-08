using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Linq;
using Machine.Specifications.Runner;

namespace Machine.Specifications.Reporting
{
  public class ReportGenerator
  {
    private string _path;
    private Dictionary<string, List<ContextInfo>> _contextsByAssembly;
    private Dictionary<ContextInfo, List<SpecificationInfo>> _specificationsByContext;
    private Dictionary<SpecificationInfo, Result> _resultsBySpecification;
    private bool _showTimeInfo;
    private const string noContextKey = "none";

    public ReportGenerator()
    {
      _path = string.Empty;
    }

    public ReportGenerator(string path)
    {
      _path = path;
    }

    public ReportGenerator(string path, Dictionary<string, List<ContextInfo>> contextsByAssembly, Dictionary<ContextInfo, List<SpecificationInfo>> specificationsByContext, Dictionary<SpecificationInfo, Result> resultsBySpecification, bool showTimeInfo)
    {
      _path = path;
      _contextsByAssembly = contextsByAssembly;
      _specificationsByContext = specificationsByContext;
      _resultsBySpecification = resultsBySpecification;
      _showTimeInfo = showTimeInfo;
    }

    public virtual void WriteReports()
    {
      if (IsProvidedPathAValidDirectoryPath(_path))
      {
        WriteReportsToDirectory();
      }
      else if (IsProvidedPathAValidFilePath(_path))
      {
        WriteReportsToFile();
      }
    }

    void WriteReportsToFile()
    {
      string reportFilePath = _path;
      string generatedReport = GetTemplateHeader();

      generatedReport += Render(_contextsByAssembly);

      generatedReport += GetTemplateFooter();

      if (File.Exists(reportFilePath))
      {
        File.Delete(reportFilePath);
      }

      TextWriter tw = new StreamWriter(reportFilePath);

      tw.Write(generatedReport);
      tw.Close();
    }

    void WriteReportsToDirectory()
    {


      List<string> htmlPaths = new List<string>();
      _contextsByAssembly.Keys.ToList().ForEach(assembly =>
      {
        string generatedReport = GetTemplateHeader();
        var oneAssemblyMap = new Dictionary<string, List<ContextInfo>>();
        oneAssemblyMap[assembly] = _contextsByAssembly[assembly];

        generatedReport += Render(oneAssemblyMap);
        generatedReport += GetTemplateFooter();

        string reportFilePath = GetReportFilePath(assembly);
        htmlPaths.Add(reportFilePath);

        if (File.Exists(reportFilePath))
        {
          File.Delete(reportFilePath);
        }

        TextWriter tw = new StreamWriter(reportFilePath);

        tw.Write(generatedReport);
        tw.Close();
      });
    }

    public string GetReportFilePath(string assembly)
    {

      string month = "";
      string day = "";
      string hour = "";
      string minute = "";
      string second = "";
      month = DateTime.Now.Month < 10 ? "0" + DateTime.Now.Month.ToString() : DateTime.Now.Month.ToString();
      day = DateTime.Now.Day < 10 ? "0" + DateTime.Now.Day.ToString() : DateTime.Now.Day.ToString();
      hour = DateTime.Now.Hour < 10 ? "0" + DateTime.Now.Hour.ToString() : DateTime.Now.Hour.ToString();
      minute = DateTime.Now.Minute < 10 ? "0" + DateTime.Now.Minute.ToString() : DateTime.Now.Minute.ToString();
      second = DateTime.Now.Second < 10 ? "0" + DateTime.Now.Second.ToString() : DateTime.Now.Second.ToString();

      return _path +
             Path.DirectorySeparatorChar +
             assembly +
             "_" +
             month +
             day +
             DateTime.Now.Year.ToString() +
             "_" +
             hour +
             minute +
             second +
             ".html";

    }

    public static bool IsProvidedPathAValidDirectoryPath(string path)
    {
      try
      {
        if (Directory.Exists(path))
        {
          return true;
        }
      }
      catch (NullReferenceException)
      {
        return false;
      }
      return false;
    }

    public static bool IsProvidedPathAValidFilePath(string path)
    {
      if (new FileInfo(path).Directory.Exists)
      {
        return true;
      }
      return false;
    }

    public string Render(Dictionary<string, List<ContextInfo>> contextsByAssembly)
    {
      StringBuilder reportBuilder = new StringBuilder();

      var contextsByConcern = new Dictionary<string, Dictionary<string, List<ContextInfo>>>();

      contextsByAssembly.Keys.ToList().ForEach(assembly =>
      {
        contextsByConcern[assembly] =
          OrganizeContextsByConcern(contextsByAssembly[assembly]);

        RenderTitle(assembly, contextsByConcern[assembly], reportBuilder);
      });
      RenderHR(reportBuilder);
      RenderHR(reportBuilder);

      contextsByAssembly.Keys.ToList().ForEach(assembly =>
      {
        RenderConcerns(contextsByConcern[assembly], reportBuilder);
        reportBuilder.AppendLine("<br><br>");
      });

      RenderHR(reportBuilder);

      string reportBody = reportBuilder.ToString();

      return reportBody;
    }

    static Dictionary<string, List<ContextInfo>> OrganizeContextsByConcern(List<ContextInfo> contextsInAssembly)
    {
      var organized = new Dictionary<string, List<ContextInfo>>();
      organized.Add(noContextKey, new List<ContextInfo>());

      contextsInAssembly.ForEach(context =>
        AddContextToOrganizedCollectionByConcern(context, organized)
      );

      if (organized[noContextKey].Count == 0)
        organized.Remove(noContextKey);
      return organized;
    }

    public static void AddContextToOrganizedCollectionByConcern(ContextInfo context, Dictionary<string, List<ContextInfo>> organized)
    {
      if (context.Concern == null)
      {
        organized[noContextKey].Add(context);
        return;
      }

      if (!organized.ContainsKey(context.Concern))
        organized.Add(context.Concern, new List<ContextInfo>());
      organized[context.Concern].Add(context);
    }

    private void RenderTitle(string assemblyName, Dictionary<string, List<ContextInfo>> contextsByConcern, StringBuilder reportBuilder)
    {
      string concernsCaption = ConcernsCaption(contextsByConcern);
      string contextsCaption = ContextsCaption(contextsByConcern);
      string specificationsCaption = SpecificationsCaption(contextsByConcern);

      string specFailuresCaption = FormatCountString(CountOfSpecsWithDesiredResultIn(contextsByConcern, Status.Failing), "failure", "failure");
      string specNotImplCaption = FormatCountString(CountOfSpecsWithDesiredResultIn(contextsByConcern, Status.NotImplemented), "not implemented spec", "notimplemented");
      string generatedOnCaption = GeneratedOnCaption(_showTimeInfo);

      string title = String.Format("<h1>{0}&nbsp;&nbsp;&nbsp;&nbsp;</h1><span class=\"count\"><h2>{1}, {2}, {3}{4}{5} </h2><br>{6}</span>", assemblyName, concernsCaption, contextsCaption, specificationsCaption, specFailuresCaption, specNotImplCaption, generatedOnCaption);
      reportBuilder.Append(title);
    }

    static string GeneratedOnCaption(bool show)
    {
      string retVal = string.Empty;
      if (!show)
        return retVal;
      DateTime now = DateTime.Now;
      retVal = @"<i>Generated on " + now.ToLongDateString() + " at " + now.ToLongTimeString() + "</i>";
      return retVal;
    }

    int CountOfSpecsWithDesiredResultIn(Dictionary<string, List<ContextInfo>> contextsByConcern, Status status)
    {
      var niResults = 0;
      contextsByConcern.Keys.ToList().ForEach(concern =>
        niResults += CountOfSpecsWithDesiredResultIn(contextsByConcern[concern], status));
      return niResults;
    }

    private int CountOfSpecsWithDesiredResultIn(List<ContextInfo> contexts, Status status)
    {
      var niResults = 0;
      contexts.ForEach(context =>
        niResults += CountOfSpecsWithDesiredResultIn(context, status));
      return niResults;
    }

    private int CountOfSpecsWithDesiredResultIn(ContextInfo context, Status status)
    {
      var niResults = 0;
      niResults += iterateOverSpecsAndReturnThoseThatMatchResults(context, status).Count;
      return niResults;
    }

    private static string FormatCountString(int count, string itemToPluralize, string formatClass)
    {
      return count > 0 ? String.Format(", <span class=\"" + formatClass + "\">{0} {1}</span>", count, Pluralize(itemToPluralize, count)) : string.Empty;
    }

    private List<Result> iterateOverSpecsAndReturnThoseThatMatchResults(ContextInfo context, Status status)
    {
      var niResults = new List<Result>();
      _specificationsByContext[context].ForEach(spec =>
      {
        if (_resultsBySpecification[spec].Status == status)
          niResults.Add(_resultsBySpecification[spec]);
      });
      return niResults;
    }

    private void RenderConcerns(Dictionary<string, List<ContextInfo>> contextsByConcern, StringBuilder reportBuilder)
    {
      foreach (string concern in contextsByConcern.Keys)
      {
        RenderConcern(concern, contextsByConcern[concern], reportBuilder);
      }
    }

    private void RenderConcern(string concernName, List<ContextInfo> contextsInConcern, StringBuilder reportBuilder)
    {
      string concernText = RenderConcern(concernName, contextsInConcern);
      reportBuilder.Append(concernText);
    }

    public string RenderConcern(string concernName, List<ContextInfo> contextsInConcern)
    {
      StringBuilder reportBuilder = new StringBuilder();

      string concernHeader = RenderConcernHeader(concernName, contextsInConcern);
      concernHeader = String.Format("{0}", concernHeader);
      reportBuilder.AppendLine(concernHeader);

      RenderContexts(contextsInConcern, reportBuilder);

      RenderHR(reportBuilder);

      return reportBuilder.ToString();
    }

    public string RenderConcernHeader(string concernName, List<ContextInfo> contexts)
    {
      string contextsCaption = ContextsCaption(contexts);
      string specificationsCaption = SpecificationsCaption(contexts);

      string specFailuresCaption = FormatCountString(CountOfSpecsWithDesiredResultIn(contexts, Status.Failing), "failure", "failure");
      string specNotImplCaption = FormatCountString(CountOfSpecsWithDesiredResultIn(contexts, Status.NotImplemented), "not implemented spec", "notimplemented");

      return String.Format("<h2 class=\"concern\">{0} specifications&nbsp;&nbsp;&nbsp;&nbsp;</h2><h3><span class=\"count\">{1}, {2}{3}{4}</span></h3>", concernName, contextsCaption, specificationsCaption, specFailuresCaption, specNotImplCaption);
    }

    private void RenderContexts(List<ContextInfo> contexts, StringBuilder reportBuilder)
    {
      contexts.ForEach(context =>
        reportBuilder.Append(RenderContext(context)));
    }

    public string RenderContext(ContextInfo context)
    {
      StringBuilder reportBuilder = new StringBuilder();

      reportBuilder.AppendLine();
      string contextHeader = RenderContextHeader(context);
      reportBuilder.Append(contextHeader);

      /*
      string behavesLike = context.BehavesLike;
      if (behavesLike != null)
      {
        reportBuilder.Append(RenderBehavesLike(behavesLike));
        reportBuilder.AppendLine();
      }
      */

      string specificationList = RenderSpecificationList(_specificationsByContext[context]);
      reportBuilder.AppendLine(specificationList);
      reportBuilder.AppendLine();

      return reportBuilder.ToString();
    }

    public string RenderContextHeader(ContextInfo context)
    {
      string specFailuresCaption = FormatCountString(CountOfSpecsWithDesiredResultIn(context, Status.Failing), "failure", "failure");
      string specNotImplCaption = FormatCountString(CountOfSpecsWithDesiredResultIn(context, Status.NotImplemented), "not implemented spec", "notimplemented");

      string specificationsCaption = SpecificationsCaption(context);
      return String.Format("<h3 class=\"context\">{0}&nbsp;&nbsp;&nbsp;&nbsp;</h3><h4><span class=\"count\">{1}{2}{3}</span></h4>", context.Name, specificationsCaption, specFailuresCaption, specNotImplCaption);
    }

    public string RenderSpecificationList(List<SpecificationInfo> specifications)
    {
      StringBuilder specificationListBuilder = new StringBuilder();

      foreach (SpecificationInfo specification in specifications)
      {
        Result result = _resultsBySpecification[specification];
        string specificationListItem = "";
        switch (result.Status)
        {
          case Status.Passing:
            specificationListItem = String.Format("\t<li>{0}", specification.Name);
            break;
          case Status.Failing:
            specificationListItem = String.Format("\t<li>{0}", specification.Name + " <div class=\"failure\">&lt;--FAILED</div><br/>");
            specificationListItem += "<p class=\"exception_type\">" + result.Exception.GetType();
            specificationListItem += "<pre class=\"exception_message\">" + result.Exception + "</pre></p>";
            break;
          case Status.NotImplemented:
            specificationListItem = String.Format("\t<li>{0}", specification.Name + " <div class=\"notimplemented\">&lt;--NOT IMPLEMENTED</div><br/>");
            break;
        }
        specificationListItem += "</li>";
        specificationListBuilder.AppendLine(specificationListItem);
        // TODO: pass/fail goes here?
      }

      return String.Format("<ul>{0}</ul>", specificationListBuilder);
    }

    private static void RenderHR(StringBuilder reportBuilder)
    {
      string hr = "<hr>";
      reportBuilder.AppendLine(hr);
    }

    private static string GetTemplateHeader()
    {
      return @"<html>
  <head>
    <title>Machine.Specifications Report</title>
    <style type=""text/css"">
      body {
        font-family: Arial,Helvetica,sans-serif;
        font-size: .9em;
      }

      .count {
        color: LightGrey;
      }

      .behaves_like {
        color: DarkGrey;
        font-weight: bold;
        margin-left: 20px;
        margin-top: -10px;
      }
      
      .failure {
        color: red;
        font-weight: bold;
        display: inline;
      }

      .notimplemented {
        color: orange;
        font-weight: bold;
        display: inline;
      }

      p.exception_type {
        color: black;
        font-weight: bold;
        display: inline;
      }

      pre.exception_message {
        border-style: dashed;
        border-color: #FF828D;
        border-width: thin;
        background-color: #FFD2CF;
        white-space: pre-wrap; /* CSS2.1 compliant */
        white-space: -moz-pre-wrap; /* Mozilla-based browsers */
        white-space: o-pre-wrap; /* Opera 7+ */
        padding: 1em;
      }
      
      hr {
        color: LightGrey;
        border: 1px solid LightGrey;
        height: 1px;
      }
      
      
    </style>
  </head>
  <body>
    ";
    }

    public static string GetTemplateFooter()
    {
      return @"
</body>
</html>";
    }

    public static string Pluralize(string caption, int count)
    {
      if (count > 1 || count == 0)
      {
        caption += "s";
      }

      return caption;
    }

    private static string ConcernsCaption(Dictionary<string, List<ContextInfo>> contextsByConcern)
    {
      int concernsCount = contextsByConcern.Keys.Count;

      string concernsCaption = String.Format("{0} {1}", concernsCount, Pluralize("concern", concernsCount));
      return concernsCaption;
    }

    private static string ContextsCaption(Dictionary<string, List<ContextInfo>> contextsByConcern)
    {
      int contextCount = 0;
      contextsByConcern.Keys.ToList().ForEach(concern =>
      {
        contextCount += contextsByConcern[concern].Count;
      });
      return ContextsCaption(contextCount);
    }

    private static string ContextsCaption(List<ContextInfo> contexts)
    {
      return ContextsCaption(contexts.Count);
    }

    private static string ContextsCaption(int contextCount)
    {
      string contextCaption = String.Format("{0} {1}", contextCount, Pluralize("context", contextCount));
      return contextCaption;
    }

    private static string SpecificationsCaption(int specificationCount)
    {
      string specificationCaption = String.Format("{0} {1}", specificationCount, Pluralize("specification", specificationCount));
      return specificationCaption;
    }

    private string SpecificationsCaption(Dictionary<string, List<ContextInfo>> contextsByConcern)
    {
      int specificationCount = 0;

      contextsByConcern.Keys.ToList().ForEach(concern =>
      {
        contextsByConcern[concern].ForEach(context =>
        {
          specificationCount += _specificationsByContext[context].Count;
        });
      });

      return SpecificationsCaption(specificationCount);
    }

    private string SpecificationsCaption(List<ContextInfo> contexts)
    {
      int specificationCount = 0;
      contexts.ForEach(context =>
      {
        specificationCount += _specificationsByContext[context].Count;
      });
      return SpecificationsCaption(specificationCount);
    }

    private string SpecificationsCaption(ContextInfo context)
    {
      return SpecificationsCaption(_specificationsByContext[context].Count);
    }
  }

}