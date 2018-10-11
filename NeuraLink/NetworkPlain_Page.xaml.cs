using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace NeuraLink
{
    /// <summary>
    /// Interaction logic for NetworkPlain.xaml
    /// </summary>
    public partial class Graph : Page
    {
        private Canvas graph;
        private Line xPlain;
        private Line yPlain;
        private List<double> data;

        public Graph(double currentWidth, double currentHeight)
        {
            InitializeComponent();

            xPlain = new Line();
            yPlain = new Line();

            xPlain.StrokeThickness = 2;
            yPlain.StrokeThickness = 2;

            graph = (Canvas)this.FindName("DataGraph");
            DrawGraph(currentWidth);
        }

        public Graph(double currentWidth, double currentHeight, List<double> data)
        {
            InitializeComponent();

            xPlain = new Line();
            yPlain = new Line();
            this.data = data;

            xPlain.StrokeThickness = 2;
            yPlain.StrokeThickness = 2;

            graph = (Canvas)this.FindName("DataGraph");
            DrawGraph(currentWidth);
        }

        public void UpdateGraph(double newWidth)
        {
            graph.Children.Clear();
            DrawNumbers(15,data);

            xPlain.X1 = 30;
            xPlain.Y1 = graph.Height - 30;
            xPlain.X2 = newWidth - 10;
            xPlain.Y2 = graph.Height - 30;
            xPlain.Stroke = new SolidColorBrush(Color.FromRgb(118, 34, 149));

            yPlain.X1 = 30;
            yPlain.Y1 = 5;
            yPlain.X2 = 30;
            yPlain.Y2 = graph.Height - 30;
            yPlain.Stroke = new SolidColorBrush(Color.FromRgb(118, 34, 149));

            graph.Children.Add(xPlain);
            graph.Children.Add(yPlain);
        }

        private void DrawGraph(double actualWidth)
        {
            DrawNumbers(15,actualWidth);
            xPlain.X1 = 30;
            xPlain.Y1 = graph.Height - 30;
            xPlain.X2 = actualWidth - 10;
            xPlain.Y2 = graph.Height - 30;
            xPlain.Stroke = new SolidColorBrush(Color.FromRgb(118, 34, 149));

            yPlain.X1 = 30;
            yPlain.Y1 = 5;
            yPlain.X2 = 30;
            yPlain.Y2 = graph.Height - 30;
            yPlain.Stroke = new SolidColorBrush(Color.FromRgb(118, 34, 149));

            graph.Children.Add(xPlain);
            graph.Children.Add(yPlain);
        }

        private void DrawNumbers(int lengthOfData, List<double> data)
        {
            double max = data.Max();
            double horizontalNumberGap = (graph.ActualWidth - 30) / lengthOfData;
            double verticalNumberGap = max / (graph.ActualHeight - 30);

            for (int n = 1; n <= lengthOfData; n++)
            {
                TextBlock number = CreateNewNumber();
                number.Margin = new Thickness(horizontalNumberGap * n, 430, 0, 0);
                number.Text = n.ToString();
                graph.Children.Add(number);
            }

            TextBlock verticalMax = CreateNewNumber();
            verticalMax.Margin = new Thickness(7, 25, 0, 0);
            verticalMax.Text = max.ToString();
            graph.Children.Add(verticalMax);

            TextBlock verticalMidle = CreateNewNumber();
            verticalMidle.Margin = new Thickness(7, (graph.ActualHeight-30)/2, 0, 0);
            verticalMidle.Text = (max/2).ToString();
            graph.Children.Add(verticalMidle);

            TextBlock verticalMin = CreateNewNumber();
            verticalMin.Margin = new Thickness(7, (graph.ActualHeight - 30), 0, 0);
            verticalMin.Text = "0";
            graph.Children.Add(verticalMin);
        }

        private void DrawNumbers(int lengthOfData, double currentWidth)
        {
            double numberGap = (currentWidth - 40) / lengthOfData;

            for (int n = 1; n <= lengthOfData; n++)
            {
                TextBlock number = CreateNewNumber();
                number.Margin = new Thickness(numberGap * n, 430, 0, 0);
                number.Text = n.ToString();
                graph.Children.Add(number);
            }

        }

        private void DrawData(List<double> data, double verticalStep, double horizontalStep)
        {
            foreach(double number in data)
            {
                double margin = (verticalStep * number);

                //Ellipse dataPoint = CreateNewNumber();
                //number.Margin = new Thickness(numberGap * n, 430, 0, 0);
                //number.Text = n.ToString();
                //graph.Children.Add(number);
            }
        }

        private TextBlock CreateNewNumber()
        {
            TextBlock number;

            number = new TextBlock
            {
                FontSize = 15,
                FontWeight = FontWeight.FromOpenTypeWeight(50),
                FontFamily = new FontFamily("Microsoft New Tai Lue"),
                Foreground = new SolidColorBrush(Color.FromRgb(118, 34, 149)),
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top
            };

            return number;
        }

        private Ellipse CreateEllipse()
        {
            Ellipse circle;
            circle = new Ellipse()
            {
                Fill = new SolidColorBrush(Color.FromRgb(118, 34, 149)),
                Width = 5,
                Height = 5
            };

            return circle;
        }
    }
}
