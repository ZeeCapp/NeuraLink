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
using System.Data;
using System.Windows.Shapes;

namespace NeuraLink.CustomControls
{
    /// <summary>
    /// Interaction logic for Graph.xaml
    /// </summary>
    public partial class Graph : UserControl
    {
        //TODO allow seting graph color in XAML
        //make connection line drawing more general

        public static readonly DependencyProperty GraphColorProperty =
            DependencyProperty.Register("GraphColor", typeof(SolidColorBrush), typeof(Graph));

        public SolidColorBrush GraphColor
        {
            get { return (SolidColorBrush)GetValue(GraphColorProperty); }
            set { SetValue(GraphColorProperty, value); }

        }

        private List<double> _CurrentData;

        public List<double> CurrentData
        {
            get { return _CurrentData; }
            set { _CurrentData = value; UpdateGraph(null,null); }
        }

        private Canvas graph;
        private Line xPlain;
        private Line yPlain;

        private double verticalStep = 0;
        private double horizontalStep = 0;

        public Graph()
        {
            InitializeComponent();
            
            graph = (Canvas)this.FindName("GraphCanvas");
            graph.VerticalAlignment = VerticalAlignment.Stretch;
            graph.HorizontalAlignment = HorizontalAlignment.Stretch;
            graph.Loaded += DrawGraphPlains;
        }

        public void UpdateGraph(object sender, EventArgs args)
        {
            graph.Children.Clear();
            graph.RaiseEvent(new RoutedEventArgs(Canvas.LoadedEvent, graph));
            DrawData(null);
        }

        public void DrawData(List<double> data)
        {
            graph.Children.Clear();
            double lastX = 0, lastY = 0;

            if(data != null && data != CurrentData)
            {
                _CurrentData = data;
            }

            DrawGraphPlains(null, null);
            DrawNumbers(CurrentData.Count);
            DrawGraphScale(CurrentData.Count, CurrentData);

            for (int n = 0; n < CurrentData.Count; n++)
            {
                double currentX, currentY;
                double topMargin = (verticalStep * CurrentData[n]);

                Ellipse dataPoint = CreateNewEllipse();
                dataPoint.Margin = new Thickness(currentX = (n + 1) * horizontalStep, currentY = graph.ActualHeight - (topMargin + 30), 0, 0);
                graph.Children.Add(dataPoint);
                graph.Children.Add(GenerateConnectingLine(lastX, lastY, currentX, currentY));

                lastX = currentX;
                lastY = currentY;
            }
        }

        private void DrawGraphPlains(object sender, EventArgs args)
        {
            graph.Children.Clear();
            xPlain = new Line();
            xPlain.X1 = 30;
            xPlain.Y1 = graph.RenderSize.Height - 30;
            xPlain.X2 = graph.ActualWidth - 10;
            xPlain.Y2 = graph.ActualHeight - 30;
            xPlain.Stroke = GraphColor;

            yPlain = new Line();
            yPlain.X1 = 30;
            yPlain.Y1 = 5;
            yPlain.X2 = 30;
            yPlain.Y2 = graph.ActualHeight - 30;
            yPlain.Stroke = GraphColor;

            graph.Children.Add(xPlain);
            graph.Children.Add(yPlain);
        }

        private void DrawGraphScale(int lengthOfData, List<double> data)
        {
            double max = data.Max();
            horizontalStep = (graph.ActualWidth - 30) / lengthOfData;
            verticalStep = (graph.ActualHeight - 60) / max;

            TextBlock verticalMax = CreateNewNumber();
            verticalMax.Margin = new Thickness(7, 25, 0, 0);
            verticalMax.Text = max.ToString();
            graph.Children.Add(verticalMax);

            TextBlock verticalMidle = CreateNewNumber();
            verticalMidle.Margin = new Thickness(7, (graph.ActualHeight - 30) / 2, 0, 0);
            verticalMidle.Text = (max / 2).ToString();
            graph.Children.Add(verticalMidle);

            TextBlock verticalMin = CreateNewNumber();
            verticalMin.Margin = new Thickness(7, (graph.ActualHeight - 30), 0, 0);
            verticalMin.Text = "0";
            graph.Children.Add(verticalMin);
        }

        private void DrawNumbers(int lengthOfData)
        {
            double numberGap = (graph.ActualWidth - 30) / lengthOfData;

            for (int n = 1; n <= lengthOfData; n++)
            {
                TextBlock number = CreateNewNumber();
                number.Margin = new Thickness(numberGap * n, graph.ActualHeight -25, 0, 0);
                number.Text = n.ToString();
                graph.Children.Add(number);
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
                Foreground = GraphColor,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top
            };

            return number;
        }

        private Line GenerateConnectingLine(double lastX, double lastY, double currentX, double currentY)
        {
            if (lastX == 0 || lastY == 0)
            {
                lastX = currentX;
                lastY = currentY;
            }

            Line connectingLine = new Line()
            {
                X1 = lastX + 3,
                Y1 = lastY + 3,
                X2 = currentX + 3,
                Y2 = currentY + 3,
                StrokeThickness = 2,
                Stroke = GraphColor
            };

            return connectingLine;
        }

        private Ellipse CreateNewEllipse()
        {
            Ellipse circle;
            circle = new Ellipse()
            {
                Fill = GraphColor,
                Width = 6,
                Height = 6,
                HorizontalAlignment = HorizontalAlignment.Left,
            };
            return circle;
        }
    }
}
