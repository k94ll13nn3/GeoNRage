using System.Reflection;
using Microsoft.AspNetCore.Components;

namespace GeoNRage.App.Layouts.Main;

public partial class MainLayout
{
    [Inject]
    public NavigationManager NavigationManager { get; set; } = null!;

    public string VersionString { get; set; } = string.Empty;

    protected override void OnInitialized()
    {
        string version = Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion ?? "0.0.0";
        VersionString = $"v{version}";
    }
}
