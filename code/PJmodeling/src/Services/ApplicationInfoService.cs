using System.Diagnostics;
using System.IO;
using System.Reflection;

using NMSE.Contracts.Services;

namespace NMSE.Services;

public class ApplicationInfoService : IApplicationInfoService
{
    public ApplicationInfoService()
    {
    }

    public Version GetVersion()
    {
        // Set the app version in NMSE > Properties > Package > PackageVersion
        // undone Assembly.GetExecutingAssembly().Location is only available in DEBUG builds, empty in RELEASE, so no file can be found
        //string assemblyLocation = Assembly.GetExecutingAssembly().Location;
        var assemblyLocation = Path.Combine(AppContext.BaseDirectory, "nmse.exe");
        var version = FileVersionInfo.GetVersionInfo(assemblyLocation).FileVersion;
        return new(version ?? "0.0");
    }
}
