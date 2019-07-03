using OxyPlot;
using Microsoft.AppCenter.Analytics;
using OxyPlot.Axes;
using OxyPlot.Series;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AgroPathogenMeterApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class rawData : ContentPage   //Allows for developers to view the raw data
    {
        public rawData()   //Allow a developer to view the raw data
        {
            Analytics.TrackEvent("Raw Data Opened");
            GraphCreate();
            InitializeComponent();
        }

        private void GraphCreate()   //Work on getting a graph working on the phone
        {
            var Model = new PlotModel { Title = "Square Wave Voltammetric Scan" };
            var lineSeries = new LineSeries();
            Model.Axes.Add(new LinearAxis { Position = AxisPosition.Bottom, Minimum = 0, Maximum = 25 });
            Model.Axes.Add(new LinearAxis { Position = AxisPosition.Left, Minimum = 0, Maximum = 25 });
            lineSeries.Points.Add(new DataPoint(0, 0));
            lineSeries.Points.Add(new DataPoint(2, 18));
            Model.Series.Add(lineSeries);
            BindingContext = Model;
            //return Model;
        }
    }
}