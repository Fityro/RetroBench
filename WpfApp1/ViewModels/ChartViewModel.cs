using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using System;
using System.ComponentModel;

namespace Aida64Clone.ViewModels
{
    public class ChartViewModel : INotifyPropertyChanged
    {
        public PlotModel PlotModel { get; set; }

        public ChartViewModel()
        {
            PlotModel = new PlotModel { Title = "CPU Usage" };
            PlotModel.Series.Add(new LineSeries { Title = "CPU Usage" });
            PlotModel.Axes.Add(new DateTimeAxis { Position = AxisPosition.Bottom, StringFormat = "HH:mm:ss" });
            PlotModel.Axes.Add(new LinearAxis { Position = AxisPosition.Left, Minimum = 0, Maximum = 100, Title = "Usage (%)" });
        }

        public void UpdateChartData(double value)
        {
            var series = PlotModel.Series[0] as LineSeries;
            series.Points.Add(new DataPoint(DateTimeAxis.ToDouble(DateTime.Now), value));
            if (series.Points.Count > 100)
            {
                series.Points.RemoveAt(0);
            }
            PlotModel.InvalidatePlot(true);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}