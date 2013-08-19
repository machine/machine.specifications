using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;

using Machine.Specifications.Runner;

namespace Machine.Specifications.Reporting.Generation.Xml
{
  public class XmlReportGenerator
  {
    private readonly string _path;
    private readonly Dictionary<AssemblyInfo, List<ContextInfo>> _contextsByAssembly;
    private readonly Dictionary<ContextInfo, List<SpecificationInfo>> _specificationsByContext;
    private readonly Dictionary<SpecificationInfo, Result> _resultsBySpecification;
    readonly TimingRunListener _timer;
    private readonly bool _showTimeInfo;

    public XmlReportGenerator()
    {
      _path = string.Empty;
    }

    public XmlReportGenerator(string path)
    {
      _path = path;
    }

    public XmlReportGenerator(string path,
                              Dictionary<AssemblyInfo, List<ContextInfo>> contextsByAssembly,
                              Dictionary<ContextInfo, List<SpecificationInfo>> specificationsByContext,
                              Dictionary<SpecificationInfo, Result> resultsBySpecification,
                              TimingRunListener timer,
                              bool showTimeInfo)
    {
      _path = path;
      _contextsByAssembly = contextsByAssembly;
      _specificationsByContext = specificationsByContext;
      _resultsBySpecification = resultsBySpecification;
      _timer = timer;
      _showTimeInfo = showTimeInfo;
    }

    public virtual void WriteReports()
    {
      if (IsProvidedPathAValidFilePath(_path))
      {
        WriteReportsToFile();
      }
    }

    private void WriteReportsToFile()
    {
      string reportFilePath = _path;

      if (File.Exists(reportFilePath))
      {
        File.Delete(reportFilePath);
      }

      var settings = new XmlWriterSettings();
      settings.Indent = true;
      var reportBuilder = XmlWriter.Create(_path, settings);
      Render(reportBuilder, _contextsByAssembly);

      reportBuilder.Close();
    }

    public static bool IsProvidedPathAValidFilePath(string path)
    {
      if (new FileInfo(path).Directory.Exists)
      {
        return true;
      }
      return false;
    }

    public void Render(XmlWriter reportBuilder, Dictionary<AssemblyInfo, List<ContextInfo>> contextsByAssembly)
    {
      reportBuilder.WriteStartDocument();
      reportBuilder.WriteStartElement("MSpec");

      if (_showTimeInfo)
      {
        RenderTimeStamp(reportBuilder);
      }

      RenderRun(reportBuilder);
      RenderAssemblies(reportBuilder, contextsByAssembly);

      reportBuilder.WriteEndElement();
      reportBuilder.WriteEndDocument();
    }

    private static void RenderTimeStamp(XmlWriter reportBuilder)
    {
      var now = DateTime.Now;
      reportBuilder.WriteStartElement("generated");
      reportBuilder.WriteElementString("date", now.ToLongDateString());
      reportBuilder.WriteElementString("time", now.ToLongTimeString());
      reportBuilder.WriteEndElement();
    }

    void RenderRun(XmlWriter reportBuilder)
    {
      reportBuilder.WriteStartElement("run");
      reportBuilder.WriteAttributeString("time", _timer.GetRunTime().ToString(CultureInfo.InvariantCulture));
      reportBuilder.WriteEndElement();
    }

    private void RenderAssemblies(XmlWriter reportBuilder, Dictionary<AssemblyInfo, List<ContextInfo>> contextsByAssembly)
    {
      contextsByAssembly.Keys.ToList().ForEach(assembly =>
        {
          reportBuilder.WriteStartElement("assembly");
          reportBuilder.WriteAttributeString("name", assembly.Name);
          reportBuilder.WriteAttributeString("location", assembly.Location);
          reportBuilder.WriteAttributeString("time", _timer.GetAssemblyTime(assembly).ToString(CultureInfo.InvariantCulture));
          RenderCapturedOutput(reportBuilder, assembly);
          RenderConcerns(reportBuilder, contextsByAssembly[assembly]);
          reportBuilder.WriteEndElement();
        });
    }

    static void RenderCapturedOutput(XmlWriter reportBuilder, object info)
    {
      var capture = info.GetType().GetProperty("CapturedOutput");
      if (capture == null)
      {
        return;
      }
      
      var output = capture.GetValue(info, new object[0]);

      reportBuilder.WriteStartElement("output");
      if (output != null)
      {
        reportBuilder.WriteValue(output);
      }
      reportBuilder.WriteEndElement();
    }

    private void RenderConcerns(XmlWriter reportBuilder, IEnumerable<ContextInfo> contexts)
    {
      Dictionary<string, List<ContextInfo>> contextsByConcern = OrganiseContextsByConcern(contexts);
      contextsByConcern.Keys.ToList().ForEach(concern =>
        {
          reportBuilder.WriteStartElement("concern");
          reportBuilder.WriteAttributeString("name", concern);

          RenderContexts(reportBuilder, contextsByConcern[concern]);

          reportBuilder.WriteEndElement();
        });
    }

    private static Dictionary<string, List<ContextInfo>> OrganiseContextsByConcern(IEnumerable<ContextInfo> contexts)
    {
      var contextsByConcern = new Dictionary<string, List<ContextInfo>>();

      foreach (var context in contexts)
      {
        if (!contextsByConcern.ContainsKey(context.Concern))
        {
          contextsByConcern.Add(context.Concern, new List<ContextInfo>());
        }
        contextsByConcern[context.Concern].Add(context);
      }
      return contextsByConcern;
    }

    private void RenderContexts(XmlWriter reportBuilder, IEnumerable<ContextInfo> contexts)
    {
      foreach (var context in contexts)
      {
        reportBuilder.WriteStartElement("context");
        reportBuilder.WriteAttributeString("name", context.Name);
        reportBuilder.WriteAttributeString("type-name", context.TypeName);
        reportBuilder.WriteAttributeString("time", _timer.GetContextTime(context).ToString(CultureInfo.InvariantCulture));
        RenderCapturedOutput(reportBuilder, context);
        RenderSpecifications(reportBuilder, context);
        reportBuilder.WriteEndElement();
      }
    }

    private void RenderSpecifications(XmlWriter reportBuilder, ContextInfo context)
    {
      foreach (var specification in _specificationsByContext[context])
      {
        var result = _resultsBySpecification[specification];
        string status;
        switch (result.Status)
        {
          case Status.Failing:
            status = "failed";
            break;
          case Status.Passing:
            status = "passed";
            break;
          case Status.NotImplemented:
            status = "not-implemented";
            break;
          case Status.Ignored:
            status = "ignored";
            break;
          default:
            if (result.Exception != null)
            {
              status = "exception";
            }
            else
            {
              status = "failed";
            }
            break;
        }
        reportBuilder.WriteStartElement("specification");
        reportBuilder.WriteAttributeString("leader", specification.Leader);
        reportBuilder.WriteAttributeString("name", specification.Name);
        reportBuilder.WriteAttributeString("field-name", specification.FieldName);
        reportBuilder.WriteAttributeString("status", status);
        reportBuilder.WriteAttributeString("time", _timer.GetSpecificationTime(specification).ToString(CultureInfo.InvariantCulture));
        RenderError(reportBuilder, result.Exception);
        RenderCapturedOutput(reportBuilder, specification);
        reportBuilder.WriteEndElement();
      }
    }

    static void RenderError(XmlWriter reportBuilder, ExceptionResult exception)
    {
      if (exception == null)
      {
        return;
      }

      reportBuilder.WriteStartElement("error");

      reportBuilder.WriteStartElement("type");
      reportBuilder.WriteCData(exception.FullTypeName);
      reportBuilder.WriteEndElement();

      reportBuilder.WriteStartElement("message");
      reportBuilder.WriteCData(exception.Message);
      reportBuilder.WriteEndElement();

      reportBuilder.WriteStartElement("stack-trace");
      reportBuilder.WriteCData(exception.StackTrace);
      reportBuilder.WriteEndElement();

      if (exception.InnerExceptionResult != null)
      {
        RenderError(reportBuilder, exception.InnerExceptionResult);
      }

      reportBuilder.WriteEndElement();
    }
  }
}