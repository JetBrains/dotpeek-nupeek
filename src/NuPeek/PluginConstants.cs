using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NuGet;

namespace JetBrains.DotPeek.Plugins.NuPeek
{
    public static class PluginConstants
    {
        public static readonly PackageSource NuGetPackageSource = new PackageSource("https://www.nuget.org/api/v2", "NuGet.org", true);
    }
}
