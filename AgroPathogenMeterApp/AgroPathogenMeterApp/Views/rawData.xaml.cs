using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AgroPathogenMeterApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class rawData : ContentPage   //Allows for developers to view the raw data
    {
        public rawData()
        {
            GraphCreate();
            InitializeComponent();
        }

        private void GraphCreate()
        {
            var Model = new PlotModel { Title = "Square Wave Voltammetric Scan" };
            BindingContext = Model;
            var lineSeries = new LineSeries();
            Model.Axes.Add(new LinearAxis { Position = AxisPosition.Bottom, Minimum = 0, Maximum = 25 });
            Model.Axes.Add(new LinearAxis { Position = AxisPosition.Left, Minimum = 0, Maximum = 25 });
            lineSeries.Points.Add(new DataPoint(0, 0));
            lineSeries.Points.Add(new DataPoint(2, 18));
            Model.Series.Add(lineSeries);
            //return Model;
        }
    }
}