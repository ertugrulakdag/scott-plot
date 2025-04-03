using ScottPlot;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Tdv.Dimet.Commons
{
    public class GenerateChart
    {
        private static readonly Color[] colors =
        {
            Colors.Red, Colors.LightBlue, Colors.LightGreen, Colors.Yellow, Colors.Orange, Colors.Purple, Colors.Black, Colors.White, Colors.Gray, Colors.Cyan
        };

        private static Color GetColor(int index)
        {
            if (index > 9)
            {
                return Colors.Brown;
            }
            return colors[index];
        }
        public static byte[] PieChart(Dictionary<string, int> data, int width = 500, int height = 500, int? legendAlignment = 5)
        {
            Plot plot = new();
            List<PieSlice> slices = new();
            int index = 0;
            double total = data.Select(x => x.Value).Sum();
            double[] percentages = data.Select(x => x.Value / total * 100).ToArray();

            foreach (var item in data)
            {
                if (legendAlignment != null)
                {
                    slices.Add(new PieSlice()
                    {
                        Value = item.Value,
                        FillColor = GetColor(index),
                        Label = $"{percentages[index]:0.0}%",
                        LabelFontSize = 14,
                        LabelBold = true,
                        LegendText = $"{item.Key}, {item.Value} Adet"
                    });
                }
                else
                {
                    slices.Add(new PieSlice()
                    {
                        Value = item.Value,
                        FillColor = GetColor(index),
                        Label = $"{percentages[index]:0.0}%",
                        LabelFontSize = 14,
                        LabelBold = true
                    });
                }
                index++;
            }

            var pie = plot.Add.Pie(slices);
            pie.ExplodeFraction = 0.05;
            pie.SliceLabelDistance = 0.8;


            plot.Axes.Frameless();
#if RELEASE
            plot.HideGrid();
#endif
            if (legendAlignment != null)
            {
                plot.ShowLegend();
                plot.Legend.Alignment = (Alignment)legendAlignment!;
                plot.Legend.FontSize = 14;
                plot.Legend.BackgroundColor = Colors.White;
            }


            using (var stream = new MemoryStream())
            {
                plot.SavePng("chart.png", width, height);
                byte[] imageBytes = File.ReadAllBytes("chart.png");
                return imageBytes;
            }
        }
        public static byte[] BarChart(Dictionary<string, int> data, int width = 500, int height = 500, int? legendAlignment = 5)
        {
            Plot myPlot = new();

            double[] xValues = Enumerable.Range(0, data.Count).Select(i => (double)i).ToArray();
            double[] yValues = data.Values.Select(v => (double)v).ToArray();
            var barPlot = myPlot.Add.Bars(yValues);


            foreach (var bar in barPlot.Bars)
            {
                bar.Label = bar.Value.ToString();
            }

            barPlot.ValueLabelStyle.Bold = true;
            barPlot.ValueLabelStyle.FontSize = 18;

            myPlot.Axes.Margins(bottom: 0, top: .2);
#if RELEASE
            myPlot.HideGrid();
#endif
            myPlot.Axes.Frameless();
            using (var stream = new MemoryStream())
            {
                myPlot.SavePng("chart.png", width, height);
                byte[] imageBytes = File.ReadAllBytes("chart.png");
                return imageBytes;
            }
        }
        public static byte[] HorizontalChart(Dictionary<string, int> data, int width = 500, int height = 500, int? legendAlignment = 5,string ? title=null)
        {
            Plot myPlot = new();

            List<(string name, CoordinateRange range)> ranges = data.Select(kv => (kv.Key, new CoordinateRange(0, kv.Value))).ToList();

            myPlot.Add.Ranges(ranges, horizontal: true); 

            myPlot.Title(title??string.Empty);

            myPlot.Axes.Left.TickLabelStyle.Rotation = 0;
            myPlot.Axes.Left.TickLabelStyle.Alignment = Alignment.MiddleRight;
            myPlot.Axes.Left.TickLabelStyle.FontSize = 16;
            myPlot.Axes.Left.MinimumSize = 150;

            ScottPlot.TickGenerators.NumericAutomatic tickGen = new();
            myPlot.Axes.Bottom.TickGenerator = tickGen;
            tickGen.LabelFormatter = (x) => $"{x}";

            var vl = myPlot.Add.VerticalLine(0, 1, Colors.Black, LinePattern.DenselyDashed);
            myPlot.MoveToBack(vl);

#if RELEASE
    myPlot.HideGrid();
#endif

            using (var stream = new MemoryStream())
            {
                myPlot.SavePng("chart.png", width, height);
                byte[] imageBytes = File.ReadAllBytes("chart.png");
                return imageBytes;
            }
        }


    }
}
