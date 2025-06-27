using EasyTemplate.Ava.Common;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView.Avalonia;

namespace EasyTemplate.Ava.Features;

public partial class DashboardViewModel : ViewModelBase
{
    public ISeries[] Series { get; set; } =
 {
        new LineSeries<double> { Values = new double[] { 2, 5, 3 }, Name = "数据A" },
        new ColumnSeries<double> { Values = new double[] { 4, 2, 7 }, Name = "数据B" }
    };

    //public DashboardViewModel(CartesianChart chart)
    //{
    //    // 创建数据系列
    //    var series = new ISeries[]
    //    {
    //        new LineSeries<double>
    //        {
    //            Values = new double[] { 2, 5, 3, 8, 4 },
    //            Name = "销售额"
    //        }
    //    };

    //    // 绑定到图表
    //    chart.Series = series;
    //}
}
