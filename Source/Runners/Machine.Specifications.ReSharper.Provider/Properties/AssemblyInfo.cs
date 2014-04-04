using System.Reflection;
using System.Runtime.InteropServices;

using JetBrains.Application.PluginSupport;

// General Information about an assembly is controlled through the following
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.

[assembly: AssemblyTitle("Machine.Specifications." + ProductInfo.Product + "Runner." + ProductInfo.Version)]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible
// to COM components.  If you need to access a type in this assembly from
// COM, set the ComVisible attribute to true on that type.

[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM

[assembly: Guid(ProductInfo.Uuid)]
[assembly: PluginTitle("Machine.Specifications Runner for " + ProductInfo.Product + " " + ProductInfo.Version)]
[assembly: PluginDescription("Allows " + ProductInfo.Product + " " + ProductInfo.Version + " to run Machine.Specifications as unit tests")]
[assembly: PluginVendor("Machine")]