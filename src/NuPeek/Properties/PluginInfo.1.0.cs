using System.Reflection;
using JetBrains.ActionManagement;
using JetBrains.Application.PluginSupport;

[assembly: ActionsXml("JetBrains.DotPeek.Plugins.NuPeek.Actions.1.0.xml")]

// The following information is displayed in the Plugins dialog
[assembly: PluginTitle("NuPeek")]
[assembly: PluginDescription("NuPeek allows loading and decompilation of NuGet packages from any NuGet repository.")]
[assembly: PluginVendor("Maarten Balliauw")]

[assembly: AssemblyVersion("1.0.*")]
[assembly: AssemblyFileVersion("1.0.0.0")]