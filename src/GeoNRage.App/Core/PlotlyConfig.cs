using Plotly.Blazor;
using Plotly.Blazor.LayoutLib;
using Plotly.Blazor.LayoutLib.LegendLib;

namespace GeoNRage.App.Core
{
    public class PlotlyConfig
    {
        public Config Config { get; set; } = new Config
        {
            DisplayLogo = false,
            ModeBarButtonsToRemove = new[] { "toImage", "zoom", "pan", "select", "zoomIn", "zoomOut", "autoScale", "resetScale", "lasso2d" },
        };

        public Layout Layout { get; set; } = new Layout
        {
            PaperBgColor = "#00000000",
            PlotBgColor = "#00000000",
            Margin = new Margin { B = 40, L = 30, R = 20, T = 20 },
            XAxis = new List<XAxis> { new XAxis { GridColor = "#444444", FixedRange = true } },
            YAxis = new List<YAxis> { new YAxis { GridColor = "#444444", FixedRange = true } },
            Font = new Plotly.Blazor.LayoutLib.Font { Color = "#eeeeee" },
            Legend = new Legend
            {
                YAnchor = YAnchorEnum.Top,
                Y = 0.99m,
                XAnchor = XAnchorEnum.Left,
                X = 0.01m,
                BgColor = "#eeeeee",
                Font = new Plotly.Blazor.LayoutLib.LegendLib.Font { Color = "#444444" }
            },
        };
    }
}
