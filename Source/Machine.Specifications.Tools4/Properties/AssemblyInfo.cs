using System.Reflection;
using System.Runtime.InteropServices;
using System.Security;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.

[assembly: AssemblyTitle("Machine.Specifications.Tools4")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.

[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM

[assembly: Guid("eb8d3b56-70e4-4d1c-ade2-d1f0fef9c4d9")]
[assembly: AllowPartiallyTrustedCallers]

#if NCRUNCH
// To avoid NCrunch specific profiling specific SecurityExceptions
// http://stackoverflow.com/questions/3917374/why-do-my-tests-fail-with-system-security-verificationexception
[assembly: SecurityRules(SecurityRuleSet.Level1, SkipVerificationInFullTrust = true)]
#endif 