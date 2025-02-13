using Damco.Model.DataSourcing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Damco.Model.ScreenTemplating
{
    public class ChartTemplate : Template
    {
        public int DataSourceId { get; set; }
        public DataSource DataSource { get; set; }

        public List<ChartDataField> DataFields { get; set; }

        public ChartType ChartType { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }

        public int? LegendFieldId { get; set; }

        public FieldDisplay LegendField { get; set; }

        public int? AxisFieldId { get; set; }

        public FieldDisplay AxisField { get; set; }

        public int? DataFieldId { get; set; }
        public FieldDisplay DataField { get; set; }

        public LegendPosition LegendPosition { get; set; }

        public LegendAlignment LegendAlignment { get; set; }

        public int? DrillDownLinkId { get; set; }
        public TemplateLink DrillDownLink { get; set; }
        public bool ShowValuesInLegend { get; set; }
    }

    public enum ChartType
    {
        //
        // Summary:
        //     Point chart type.
        Point = 1,
        //
        // Summary:
        //     FastPoint chart type.
        FastPoint = 2,
        //
        // Summary:
        //     Bubble chart type.
        Bubble = 3,
        //
        // Summary:
        //     Line chart type.
        Line = 4,
        //
        // Summary:
        //     Spline chart type.
        Spline = 5,
        //
        // Summary:
        //     StepLine chart type.
        StepLine = 6,
        //
        // Summary:
        //     FastLine chart type.
        FastLine = 7,
        //
        // Summary:
        //     Bar chart type.
        Bar = 8,
        //
        // Summary:
        //     Stacked bar chart type.
        StackedBar = 9,
        //
        // Summary:
        //     Hundred-percent stacked bar chart type.
        StackedBar100 = 10,
        //
        // Summary:
        //     Column chart type.
        Column = 11,
        //
        // Summary:
        //     Stacked column chart type.
        StackedColumn = 12,
        //
        // Summary:
        //     Hundred-percent stacked column chart type.
        StackedColumn100 = 13,
        //
        // Summary:
        //     Area chart type.
        Area = 14,
        //
        // Summary:
        //     Spline area chart type.
        SplineArea = 15,
        //
        // Summary:
        //     Stacked area chart type.
        StackedArea = 16,
        //
        // Summary:
        //     Hundred-percent stacked area chart type.
        StackedArea100 = 17,
        //
        // Summary:
        //     Pie chart type.
        Pie = 18,
        //
        // Summary:
        //     Doughnut chart type.
        Doughnut = 19,
        //
        // Summary:
        //     Stock chart type.
        Stock = 20,
        //
        // Summary:
        //     Candlestick chart type.
        Candlestick = 21,
        //
        // Summary:
        //     Range chart type.
        Range = 22,
        //
        // Summary:
        //     Spline range chart type.
        SplineRange = 23,
        //
        // Summary:
        //     RangeBar chart type.
        RangeBar = 24,
        //
        // Summary:
        //     Range column chart type.
        RangeColumn = 25,
        //
        // Summary:
        //     Radar chart type.
        Radar = 26,
        //
        // Summary:
        //     Polar chart type.
        Polar = 27,
        //
        // Summary:
        //     Error bar chart type.
        ErrorBar = 28,
        //
        // Summary:
        //     Box plot chart type.
        BoxPlot = 29,
        //
        // Summary:
        //     Renko chart type.
        Renko = 30,
        //
        // Summary:
        //     ThreeLineBreak chart type.
        ThreeLineBreak = 31,
        //
        // Summary:
        //     Kagi chart type.
        Kagi = 32,
        //
        // Summary:
        //     PointAndFigure chart type.
        PointAndFigure = 33,
        //
        // Summary:
        //     Funnel chart type.
        Funnel = 34,
        //
        // Summary:
        //     Pyramid chart type.
        Pyramid = 35
    }

    public enum LegendPosition
    {
        Hidden = 0,
        Bottom = 1,
        Top = 2,
        Left = 3,
        Right = 4
    }

    public enum LegendAlignment
    {
        Default = 0,
        Center = 1,
        Far = 2,
        Near = 3
    }

    public class ChartDataField 
    {
        public int Id { get; set; }

        public int ChartTemplateId { get; set; }
        public ChartTemplate ChartTemplate { get; set; }

        public int DataFieldId { get; set; }
        public DataField DataField { get; set; }

        [Required()]
        public string DisplayName { get; set; }
    }

}
