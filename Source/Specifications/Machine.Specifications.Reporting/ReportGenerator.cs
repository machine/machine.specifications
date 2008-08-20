using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Linq;

using Machine.Specifications.Model;

namespace Machine.Specifications.Reporting
{
	public class ReportGenerator
	{
	  private static string _path;
	  private static Dictionary<string, List<Context>> _contextsByAssembly;
    private static Dictionary<Context, List<Specification>> _specificationsByContext;
    private static Dictionary<Specification, SpecificationVerificationResult> _resultsBySpecification;
	  private const string noContextKey = "none";

	  public ReportGenerator()
    {
      _path = string.Empty;
    }

    public ReportGenerator(string path)
    {
      _path = path;
    }

	  public ReportGenerator(string path, Dictionary<string, List<Context>> contextsByAssembly, Dictionary<Context, List<Specification>> specificationsByContext, Dictionary<Specification, SpecificationVerificationResult> resultsBySpecification)
	  {
	    _path = path;
	    _contextsByAssembly = contextsByAssembly;
	    _specificationsByContext = specificationsByContext;
	    _resultsBySpecification = resultsBySpecification;
	  }


    public virtual void WriteReports()
		{
      List<string> htmlPaths = new List<string>();
	    _contextsByAssembly.Keys.ToList().ForEach(assembly =>
	    {
        string generatedReport = Render(assembly, _contextsByAssembly[assembly]);

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
             assembly+
             "_" +
             month+
	           day+
	           DateTime.Now.Year.ToString()+
	           "_"+
	           hour+
	           minute+
             second+
	           ".html";
	  }

	  public static string Render(string assemblyName, List<Context> contextsInAssembly)
		{
			StringBuilder reportBuilder = new StringBuilder();

	    Dictionary<string, List<Context>> contextsOrganizedByConcern = 
        OrganizeContextsByConcern(contextsInAssembly);


      RenderTitle(assemblyName, contextsOrganizedByConcern, reportBuilder);
			RenderHR(reportBuilder);

			RenderConcerns(contextsOrganizedByConcern, reportBuilder);

			string reportBody = reportBuilder.ToString();

			return String.Format(GetTemplate(), assemblyName, reportBody);
		}

	  static Dictionary<string, List<Context>> OrganizeContextsByConcern(List<Context> contextsInAssembly)
	  {
	    Dictionary<string,List<Context>> organized = new Dictionary<string, List<Context>>();
      organized.Add(noContextKey, new List<Context>());

      contextsInAssembly.ForEach(context =>
        AddContextToOrganizedCollectionByConcern(context, organized)
      );

      if (organized[noContextKey].Count == 0)
        organized.Remove(noContextKey);
	    return organized;
	  }

	  public static void AddContextToOrganizedCollectionByConcern(Context context, Dictionary<string, List<Context>> organized)
	  {
	    if (context.Concern == null)
	    {
	      organized[noContextKey].Add(context);
	      return;
	    }
	    
      if(!organized.ContainsKey(context.Concern.FullConcern))
	      organized.Add(context.Concern.FullConcern,new List<Context>());
	    organized[context.Concern.FullConcern].Add(context);
	  }

	  private static void RenderTitle(string assemblyName, Dictionary<string,List<Context>> contextsByConcern, StringBuilder reportBuilder)
		{
	    string concernsCaption = ConcernsCaption(contextsByConcern);
			string contextsCaption = ContextsCaption(contextsByConcern);
      string specificationsCaption = SpecificationsCaption(contextsByConcern);
	    
      string rawFailureCaption = SpecFailuresCaption(contextsByConcern);
	    string specFailuresCaption = string.Empty;
      if (rawFailureCaption != string.Empty)
        specFailuresCaption = ", <span class=\"failure\">"+rawFailureCaption+"</span>";

      string title = String.Format("<h1>{0}&nbsp;&nbsp;&nbsp;&nbsp;</h1><span class=\"count\"><h2>{1}, {2}, {3}{4} </h2></span>", assemblyName, concernsCaption, contextsCaption, specificationsCaption, specFailuresCaption);
			reportBuilder.Append(title);
		}

	  static string SpecFailuresCaption(Dictionary<string, List<Context>> contextsByConcern)
	  {
      List<SpecificationVerificationResult> failedResults = new List<SpecificationVerificationResult>();
	    
      contextsByConcern.Keys.ToList().ForEach(concern =>
      {
        contextsByConcern[concern].ForEach(context =>
        {
          _specificationsByContext[context].ForEach(spec =>
          {
            if(!_resultsBySpecification[spec].Passed)
              failedResults.Add(_resultsBySpecification[spec]);
          });
        });
      });


      if (failedResults.Count == 0)
        return string.Empty;
      return String.Format("{0} {1}", failedResults.Count, Pluralize("failure", failedResults.Count));
	  }

    private static string SpecFailuresCaption(List<Context> contexts)
    {
      var failedResults = new List<SpecificationVerificationResult>();

      contexts.ForEach(context =>
      {
        _specificationsByContext[context].ForEach(spec =>
        {
          if (!_resultsBySpecification[spec].Passed)
            failedResults.Add(_resultsBySpecification[spec]);
        });
      });

      if (failedResults.Count == 0)
        return string.Empty;
      return String.Format("{0} {1}", failedResults.Count, Pluralize("failure", failedResults.Count));
    }

    private static string SpecFailuresCaption(Context context)
    {
      var failedResults = new List<SpecificationVerificationResult>();

      _specificationsByContext[context].ForEach(spec =>
      {
        if (!_resultsBySpecification[spec].Passed)
          failedResults.Add(_resultsBySpecification[spec]);
      });

      if (failedResults.Count == 0)
        return string.Empty;
      return String.Format("{0} {1}", failedResults.Count, Pluralize("failure", failedResults.Count));
    }

	  private static void RenderConcerns(Dictionary<string, List<Context>> contextsByConcern, StringBuilder reportBuilder)
		{
			foreach (string concern in contextsByConcern.Keys)
			{
				RenderConcern(concern, contextsByConcern[concern], reportBuilder);
			}
		}

		private static void RenderConcern(string concernName, List<Context> contextsInConcern, StringBuilder reportBuilder)
		{
      string concernText = RenderConcern(concernName, contextsInConcern);
			reportBuilder.Append(concernText);
		}

    public static string RenderConcern(string concernName, List<Context> contextsInConcern)
		{
			StringBuilder reportBuilder = new StringBuilder();

			string concernHeader = RenderConcernHeader(concernName, contextsInConcern);
			concernHeader = String.Format("{0}\n\n", concernHeader);
			reportBuilder.Append(concernHeader);

			RenderContexts(contextsInConcern, reportBuilder);

			RenderHR(reportBuilder);

			return reportBuilder.ToString();
		}

		public static string RenderConcernHeader(string concernName, List<Context> contexts)
		{
			string contextsCaption = ContextsCaption(contexts);
			string specificationsCaption = SpecificationsCaption(contexts);

      string rawFailureCaption = SpecFailuresCaption(contexts);
      string specFailuresCaption = string.Empty;
      if (rawFailureCaption != string.Empty)
        specFailuresCaption = ", <span class=\"failure\">" + rawFailureCaption + "</span>";

      return String.Format("<h2 class=\"concern\">{0} specifications&nbsp;&nbsp;&nbsp;&nbsp;</h2><h3><span class=\"count\">{1}, {2}{3}</span></h3>", concernName, contextsCaption, specificationsCaption, specFailuresCaption);
		}

		private static void RenderContexts(List<Context> contexts, StringBuilder reportBuilder)
		{
      contexts.ForEach(context =>
        reportBuilder.Append(RenderContext(context)));
		}

		public static string RenderContext(Context context)
		{
			StringBuilder reportBuilder = new StringBuilder();

			reportBuilder.Append("\n");
			string contextHeader = RenderContextHeader(context);
			reportBuilder.Append(contextHeader);

      /*
			string behavesLike = context.BehavesLike;
			if (behavesLike != null)
			{
				reportBuilder.Append(RenderBehavesLike(behavesLike));
				reportBuilder.Append("\n\n");
			}
      */

			string specificationList = RenderSpecificationList(_specificationsByContext[context]);
			reportBuilder.Append(specificationList);
			reportBuilder.Append("\n\n");

			return reportBuilder.ToString();
		}

		public static string RenderContextHeader(Context context)
		{
      string rawFailureCaption = SpecFailuresCaption(context);
      string specFailuresCaption = string.Empty;
      if (rawFailureCaption != string.Empty)
        specFailuresCaption = ", <span class=\"failure\">" + rawFailureCaption + "</span>";

			string specificationsCaption = SpecificationsCaption(context);
			return String.Format("<h3 class=\"context\">{0}&nbsp;&nbsp;&nbsp;&nbsp;</h3><h4><span class=\"count\">{1}{2}</span></h4>", context.Name, specificationsCaption, specFailuresCaption);
		}

		public static string RenderSpecificationList(List<Specification> specifications)
		{
			StringBuilder specificationListBuilder = new StringBuilder();

			foreach (Specification specification in specifications)
			{
			  SpecificationVerificationResult result = _resultsBySpecification[specification];
			  string specificationListItem = "";
        if(result.Passed)
        {
          specificationListItem = String.Format("\t<li>{0}", specification.Name);
        }
        else
        {
          specificationListItem = String.Format("\t<li class=\"failure\">{0}", specification.Name);
          specificationListItem += "<p class=\"exception_type\">" + result.Exception.GetType().ToString();
          specificationListItem += "<pre class=\"exception_message\">" + "Message:\n"+result.Exception.Message+"\nStack Trace:\n" +result.Exception.StackTrace + "</pre></p>";
        }
			  specificationListItem += "</li>\n";
        specificationListBuilder.Append(specificationListItem);
        // TODO: pass/fail goes here?
			}

			return String.Format("<ul>\n{0}</ul>", specificationListBuilder);
		}

		private static void RenderHR(StringBuilder reportBuilder)
		{
			string hr = "<hr>\n\n";
			reportBuilder.Append(hr);
		}

		private static string GetTemplate()
		{
			string template = @"<html>
	<head>
		<title>Specification Report for {0}</title>
		<style type=""text/css"">
			body {{
				font-family: Arial,Helvetica,sans-serif;
				font-size: .9em;
			}}

			.count {{
				color: LightGrey;
			}}

			.behaves_like {{
				color: DarkGrey;
				font-weight: bold;
				margin-left: 20px;
				margin-top: -10px;
			}}
      
      .failure {{
        color: red;
        font-weight: bold;
      }}

      p.exception_type {{
        color: black;
        font-weight: bold;
      }}

      pre.exception_message {{
        border-style: dashed;
        border-color: #FF828D;
        border-width: thin;
        background-color: #FFD2CF;
        white-space: pre-wrap; /* CSS2.1 compliant */
        white-space: -moz-pre-wrap; /* Mozilla-based browsers */
        white-space: o-pre-wrap; /* Opera 7+ */
      }}
      
			hr {{
				color: LightGrey;
				border: 1px solid LightGrey;
				height: 1px;
			}}
      
      
		</style>
	</head>
	<body>
		{1}
	</body>
</html>";
			return template;
		}

		public static string Pluralize(string caption, int count)
		{
			if (count > 1 || count == 0)
			{
				caption += "s";
			}

			return caption; 
		}

    private static string ConcernsCaption(Dictionary<string, List<Context>> contextsByConcern)
		{
      int concernsCount = contextsByConcern.Keys.Count;
			
      string concernsCaption = String.Format("{0} {1}", concernsCount, Pluralize("concern", concernsCount));
			return concernsCaption;
		}

		private static string ContextsCaption(Dictionary<string,List<Context>> contextsByConcern)
		{
		  int contextCount = 0;
      contextsByConcern.Keys.ToList().ForEach(concern =>
      {
        contextCount += contextsByConcern[concern].Count;
      }); 
			return ContextsCaption(contextCount);
		}

    private static string ContextsCaption(List<Context> contexts)
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

    private static string SpecificationsCaption(Dictionary<string,List<Context>> contextsByConcern)
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

		private static string SpecificationsCaption(List<Context> contexts)
		{
		  int specificationCount = 0;
			contexts.ForEach(context =>
		  {
		    specificationCount += _specificationsByContext[context].Count;
		  });
      return SpecificationsCaption(specificationCount);
		}

    private static string SpecificationsCaption(Context context)
    {
      return SpecificationsCaption(_specificationsByContext[context].Count);
    }
	}
  
}