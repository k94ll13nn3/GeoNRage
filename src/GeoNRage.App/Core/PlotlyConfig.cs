using Plotly.Blazor;
using Plotly.Blazor.LayoutLib;
using Plotly.Blazor.LayoutLib.LegendLib;

namespace GeoNRage.App.Core;

public class PlotlyConfig
{
    public Config Config { get; set; } = new()
    {
        DisplayLogo = false,
        ModeBarButtonsToRemove = new[] { "toImage", "zoom", "pan", "select", "zoomIn", "zoomOut", "autoScale", "resetScale", "lasso2d" },
    };

    public Layout Layout { get; set; } = new()
    {
        PaperBgColor = "#00000000",
        PlotBgColor = "#00000000",
        Margin = new Margin { B = 40, L = 30, R = 20, T = 20 },
        XAxis = new List<XAxis> { new() { GridColor = "#808080", FixedRange = true } },
        YAxis = new List<YAxis> { new() { GridColor = "#808080", FixedRange = true } },
        Font = new Plotly.Blazor.LayoutLib.Font { Color = "#808080" },
        Legend = new List<Legend>(){
            new() {
                YAnchor = YAnchorEnum.Top,
                Y = 0.99m,
                XAnchor = XAnchorEnum.Left,
                X = 0.01m,
                BgColor = "#eeeeee",
                Font = new Plotly.Blazor.LayoutLib.LegendLib.Font { Color = "#444444" }
            },
        }
    };
}
