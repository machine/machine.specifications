using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Linq;

using Machine.Specifications.Model;

namespace Machine.Specifications.Reporting
{
	public class ReportGenerator
	{
    
	  private string _writePath;
    
    public ReportGenerator()
    {
      _writePath = string.Empty;
    }

    /*
	  public virtual void WriteReport(Assembly assemblyUnderTest)
		{
			SpecificationDataset specificationDataset = SpecificationDataset.Build(assemblyUnderTest);

			string generatedReport = Render(specificationDataset);

			string reportFilePath = assemblyUnderTest.GetName().Name + ".html";

			if (File.Exists(reportFilePath))
			{
				File.Delete(reportFilePath);
			}

			TextWriter tw = new StreamWriter(reportFilePath);

			tw.Write(generatedReport);
			tw.Close();
		}

		public static string Render(SpecificationDataset specificationDataset)
		{
			StringBuilder reportBuilder = new StringBuilder();

			RenderTitle(specificationDataset, reportBuilder);
			RenderHR(reportBuilder);

			Concern[] concerns =
				(from Concern c in specificationDataset.Concerns
				orderby c.Type.Name
				select c).ToArray();

			RenderConcerns(concerns, reportBuilder);

			string reportBody = reportBuilder.ToString();

			return String.Format(GetTemplate(), specificationDataset.Name, reportBody);
		}

		private static void RenderTitle(SpecificationDataset specificationDataset, StringBuilder reportBuilder)
		{
			string concernsCaption = ConcernsCaption(specificationDataset.Concerns);
			string contextsCaption = ContextsCaption(specificationDataset.Concerns);
			string specificationsCaption = SpecificationsCaption(specificationDataset.Concerns);

			string title = String.Format("<h1>{0}&nbsp;&nbsp;&nbsp;&nbsp;<span class=\"count\">{1}, {2}, {3}</span></h1>", specificationDataset.Name, concernsCaption, contextsCaption, specificationsCaption);
			reportBuilder.Append(title);
		}

		public static string RenderTitle(SpecificationDataset specificationDataset)
		{
			StringBuilder reportBuilder = new StringBuilder();
			RenderTitle(specificationDataset, reportBuilder);
			return reportBuilder.ToString();
		}

		private static void RenderConcerns(Concern[] concerns, StringBuilder reportBuilder)
		{
			foreach (Concern concern in concerns)
			{
				RenderConcern(concern, reportBuilder);
			}
		}

		private static void RenderConcern(Concern concern, StringBuilder reportBuilder)
		{
			string concernText = RenderConcern(concern);
			reportBuilder.Append(concernText);
		}

		public static string RenderConcern(Concern concern)
		{
			StringBuilder reportBuilder = new StringBuilder();

			string concernHeader = RenderConcernHeader(concern);
			concernHeader = String.Format("{0}\n\n", concernHeader);
			reportBuilder.Append(concernHeader);

			RenderContexts(concern.Contexts, reportBuilder);

			RenderHR(reportBuilder);

			return reportBuilder.ToString();
		}

		public static string RenderConcernHeader(Concern concern)
		{
			string contextsCaption = ContextsCaption(concern);
			string specificationsCaption = SpecificationsCaption(concern);

			return String.Format("<h2 class=\"concern\">{0} specifications&nbsp;&nbsp;&nbsp;&nbsp;<span class=\"count\">{1}, {2}</span></h2>", concern.Name, contextsCaption, specificationsCaption);
		}

		private static void RenderContexts(Context[] contexts, StringBuilder reportBuilder)
		{
			foreach (Context context in contexts)
			{
				reportBuilder.Append(RenderContext(context));
			}
		}

		public static string RenderContext(Context context)
		{
			StringBuilder reportBuilder = new StringBuilder();

			reportBuilder.Append("\n");
			string contextHeader = RenderContextHeader(context);
			reportBuilder.Append(contextHeader);

			string behavesLike = context.BehavesLike;
			if (behavesLike != null)
			{
				reportBuilder.Append(RenderBehavesLike(behavesLike));
				reportBuilder.Append("\n\n");
			}

			string specificationList = RenderSpecificationList(context.Specifications);
			reportBuilder.Append(specificationList);
			reportBuilder.Append("\n\n");

			return reportBuilder.ToString();
		}

		private static string RenderBehavesLike(string behavesLike)
		{
			return String.Format("<p class=\"behaves_like\">behaves like: {0}</p>", behavesLike);
		}

		public static string RenderContextHeader(Context context)
		{
			string specificationsCaption = SpecificationsCaption(context);
			return String.Format("<h3 class=\"context\">{0}&nbsp;&nbsp;&nbsp;&nbsp;<span class=\"count\">{1}</span></h3>", context.Name, specificationsCaption);
		}

		public static string RenderSpecificationList(Specification[] specifications)
		{
			StringBuilder specificationListBuilder = new StringBuilder();

			foreach (Specification specification in specifications)
			{
				string specificationListItem = String.Format("\t<li>{0}</li>\n", specification.Name);
				specificationListBuilder.Append(specificationListItem);
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

		private static string ConcernsCaption(Concern[] concerns)
		{
			int concernsCount = concerns.Length;
			string concernsCaption = String.Format("{0} {1}", concernsCount, Pluralize("concern", concernsCount));
			return concernsCaption;
		}

		private static string ContextsCaption(Concern[] concerns)
		{
			int contextCount = concerns.Sum(c => c.Contexts.Length);
			return ContextsCaption(contextCount);
		}

		private static string ContextsCaption(Concern concern)
		{
			int contextCount = concern.Contexts.Length;
			return ContextsCaption(contextCount);
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

		private static string SpecificationsCaption(Concern[] concerns)
		{
			int specificationCount = concerns.Sum(c => c.Contexts.Sum(ctx => ctx.Specifications.Length));
			return SpecificationsCaption(specificationCount);
		}

		private static string SpecificationsCaption(Concern concern)
		{
			int specificationCount = concern.Contexts.Sum(c => c.Specifications.Length);
			return SpecificationsCaption(specificationCount);
		}

		private static string SpecificationsCaption(Context context)
		{
			int specificationCount = context.Specifications.Length;
			return SpecificationsCaption(specificationCount);
		}
    */

	  public string WritePath
	  {
	    get { return _writePath;}
      set { _writePath = value; }
	  }
	}
  
}