using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text;

using Machine.Specifications.Runner.Impl;

namespace Machine.Specifications.ReSharperRunner.Runners
{
  internal static class VersionCompatibilityChecker
  {
    static readonly Action<Assembly, Assembly, Compatibility>[] Rules = new Action<Assembly, Assembly, Compatibility>[]
                                                                        {
                                                                          RunnerPath,
                                                                          MSpecPaths,
                                                                          VersionMajorMinorEquality,
                                                                          BothUnsignedOrSignedWithSameKey
                                                                        };

    /// <summary>
    /// Checks for compatibility issues between the Machine.Specifications.dll referenced by the ReSharper runner and
    /// the context assembly (i.e. test assembly).
    /// </summary>
    /// <remarks>
    /// Scenarios that do not work, that is, will display gray test icons in the ReSharper Unit Test Session window:
    /// - Runner reference is strong-named, context reference is not strong-named
    /// - Runner reference is not strong-named, context reference is strong-named
    /// - Both references are strong-named, but version numbers do not match
    /// 
    /// When both references are not strong-named and provided API compatibility is met, the runner will work even 
    /// if version numbers do not match.
    /// </remarks>
    public static Compatibility Check(Assembly contextAssembly)
    {
      var result = new Compatibility();
      result.ContextAssembly.Path = contextAssembly.Location;

      var referenced = FindMSpecReference(contextAssembly);
      if (referenced == null)
      {
        return result;
      }

      var runner = typeof(DefaultRunner).Assembly;

      foreach (var rule in Rules)
      {
        rule(runner, referenced, result);
      }

      return result;
    }

    static void RunnerPath(Assembly runner, Assembly context, Compatibility result)
    {
      result.Runner.Path = typeof(VersionCompatibilityChecker).Assembly.Location;
    }

    static void MSpecPaths(Assembly runner, Assembly context, Compatibility result)
    {
      result.Runner.MSpec.Path = runner.Location;
      result.ContextAssembly.MSpec.Path = context.Location;
    }

    static void VersionMajorMinorEquality(Assembly runner, Assembly context, Compatibility result)
    {
      var runnerVersion = runner.GetName().Version;
      var contextVersion = context.GetName().Version;

      var compatible = runnerVersion == contextVersion;

      result.Mark(compatible);
      result.Runner.MSpec.Version = runnerVersion;
      result.ContextAssembly.MSpec.Version = contextVersion;
    }

    static void BothUnsignedOrSignedWithSameKey(Assembly runner, Assembly context, Compatibility result)
    {
      var runnerToken = ToReadableString(runner.GetName().GetPublicKeyToken());
      var contextToken = ToReadableString(context.GetName().GetPublicKeyToken());

      var compatible = runnerToken.Equals(contextToken, StringComparison.OrdinalIgnoreCase);

      result.Mark(compatible);
      result.Runner.MSpec.PublicKeyToken = String.IsNullOrEmpty(runnerToken) ? "null" : runnerToken;
      result.ContextAssembly.MSpec.PublicKeyToken = String.IsNullOrEmpty(contextToken) ? "null" : contextToken;
    }

    static string ToReadableString(IEnumerable<byte> token)
    {
      const string Hex = "0123456789abcdef";
      const byte Mask = 15;

      var result = new StringBuilder();
      foreach (var @byte in token)
      {
        result.Append(Hex[@byte / 16 & Mask]);
        result.Append(Hex[@byte & Mask]);
      }

      return result.ToString();
    }

    static Assembly FindMSpecReference(Assembly contextAssembly)
    {
      foreach (var referenced in contextAssembly.GetReferencedAssemblies())
      {
        if (!referenced.Name.Equals("Machine.Specifications", StringComparison.OrdinalIgnoreCase))
        {
          continue;
        }

        return Assembly.Load(referenced);
      }

      return null;
    }

    delegate void Action<T1, T2, T3>(T1 arg1, T2 arg2, T3 arg3);

    internal class Compatibility
    {
      public readonly AssemblyInfo ContextAssembly;
      public readonly AssemblyInfo Runner;
      public bool Success = true;

      public Compatibility()
      {
        ContextAssembly = new AssemblyInfo();
        Runner = new AssemblyInfo();
      }

      public string ErrorMessage
      {
        get
        {
          return
            "The versions of Machine.Specifications.dll referenced by the context assembly and the ReSharper runner are not compatible.";
        }
      }

      public string Explanation
      {
        get
        {
          return String.Format(
            ErrorMessage + "{0}{0}" +
            "Context assembly:{0}  Path: {1}{0}  Machine.Specifications.dll reference:{0}    Path: {2}{0}    Version: {3}{0}    Public key token: {4}{0}{0}" +
            "Runner assembly:{0}  Path: {5}{0}  Machine.Specifications.dll reference:{0}    Path: {6}{0}    Version: {7}{0}    Public key token: {8}{0}{0}" +
            "Compatibility rules:{0}" +
            "  Version numbers must match when using a strong-named Machine.Specifications.dll{0}" +
            "  Public key tokens must match",
            Environment.NewLine,
            ContextAssembly.Path,
            ContextAssembly.MSpec.Path,
            ContextAssembly.MSpec.Version,
            ContextAssembly.MSpec.PublicKeyToken,
            Runner.Path,
            Runner.MSpec.Path,
            Runner.MSpec.Version,
            Runner.MSpec.PublicKeyToken);
        }
      }

      public void Mark(bool compatible)
      {
        Success &= compatible;
      }

      internal class AssemblyInfo
      {
        public readonly MSpecReference MSpec;
        public string Path;

        public AssemblyInfo()
        {
          MSpec = new MSpecReference();
        }
      }

      internal class MSpecReference
      {
        public string Path;
        public string PublicKeyToken;
        public Version Version;
      }
    }
  }
}