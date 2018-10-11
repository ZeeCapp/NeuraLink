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
    public partial class RunNetworkPage : Page
    {
        public RunNetworkPage()
        {
            InitializeComponent();
        }


        //private Canvas graph;
        //private Line xPlain;
        //private Line yPlain;
        //private List<double> data;
        //private SolidColorBrush graphColor = new SolidColorBrush(Color.FromRgb(200, 200, 200));

        //private double currentWidth;
        //private double currentHeight;
        //private double verticalStep = 0;
        //private double horizontalStep = 0;

        //public RunNetworkPageLogic(double currentWidth, double currentHeight)
        //{
        //    InitializeComponent();

        //    this.currentHeight = currentHeight;
        //    this.currentWidth = currentWidth;

        //    xPlain = new Line();
        //    yPlain = new Line();

        //    xPlain.StrokeThickness = 2;
        //    yPlain.StrokeThickness = 2;

        //    graph = (Canvas)this.FindName("DataGraph");
        //    DrawGraphPlains(currentWidth, currentHeight);
        //}

        //public RunNetworkPageLogic(double currentWidth, double currentHeight, List<double> data)
        //{
        //    InitializeComponent();


        //    this.currentHeight = currentHeight;
        //    this.currentWidth = currentWidth;

        //    xPlain = new Line();
        //    yPlain = new Line();
        //    this.data = data;

        //    xPlain.StrokeThickness = 2;
        //    yPlain.StrokeThickness = 2;

        //    graph = (Canvas)this.FindName("DataGraph");
        //    DrawGraphPlains(currentWidth,currentHeight);
        //}

        //public void UpdateGraph(double newWidth)
        //{
        //    currentWidth = newWidth;
        //    graph.Children.Clear();
        //    DrawGraphScale(data.Count,data);

        //    if(data != null)
        //    {
        //        DrawData(data, verticalStep, horizontalStep);
        //    }

        //    xPlain.X1 = 30;
        //    xPlain.Y1 = graph.Height - 30;
        //    xPlain.X2 = newWidth - 10;
        //    xPlain.Y2 = graph.Height - 30;
        //    xPlain.Stroke = graphColor;

        //    yPlain.X1 = 30;
        //    yPlain.Y1 = 5;
        //    yPlain.X2 = 30;
        //    yPlain.Y2 = graph.Height - 30;
        //    yPlain.Stroke = graphColor;

        //    graph.Children.Add(xPlain);
        //    graph.Children.Add(yPlain);
        //}

        //private void DrawGraphPlains(double actualWidth, double currentHeight)
        //{
        //    xPlain.X1 = 30;
        //    xPlain.Y1 = graph.Height - 30;
        //    xPlain.X2 = actualWidth - 10;
        //    xPlain.Y2 = graph.Height - 30;
        //    xPlain.Stroke = graphColor;

        //    yPlain.X1 = 30;
        //    yPlain.Y1 = 5;
        //    yPlain.X2 = 30;
        //    yPlain.Y2 = graph.Height - 30;
        //    yPlain.Stroke = graphColor;

        //    graph.Children.Add(xPlain);
        //    graph.Children.Add(yPlain);
        //}

        //private void DrawGraphScale(int lengthOfData, List<double> data)
        //{
        //    double max = data.Max();
        //    horizontalStep = (graph.ActualWidth - 30) / lengthOfData;
        //    verticalStep =(graph.ActualHeight - 60)/max;

        //    for (int n = 1; n <= lengthOfData; n++)
        //    {
        //        TextBlock number = CreateNewNumber();
        //        number.Margin = new Thickness(horizontalStep * n, 430, 0, 0);
        //        number.Text = n.ToString();
        //        graph.Children.Add(number);
        //    }

        //    TextBlock verticalMax = CreateNewNumber();
        //    verticalMax.Margin = new Thickness(7, 25, 0, 0);
        //    verticalMax.Text = max.ToString();
        //    graph.Children.Add(verticalMax);

        //    TextBlock verticalMidle = CreateNewNumber();
        //    verticalMidle.Margin = new Thickness(7, (graph.ActualHeight-30)/2, 0, 0);
        //    verticalMidle.Text = (max/2).ToString();
        //    graph.Children.Add(verticalMidle);

        //    TextBlock verticalMin = CreateNewNumber();
        //    verticalMin.Margin = new Thickness(7, (graph.ActualHeight - 30), 0, 0);
        //    verticalMin.Text = "0";
        //    graph.Children.Add(verticalMin);
        //}

        //private void DrawNumbers(int lengthOfData, double currentWidth)
        //{
        //    double numberGap = (currentWidth - 40) / lengthOfData;

        //    for (int n = 1; n <= lengthOfData; n++)
        //    {
        //        TextBlock number = CreateNewNumber();
        //        number.Margin = new Thickness(numberGap * n, 430, 0, 0);
        //        number.Text = n.ToString();
        //        graph.Children.Add(number);
        //    }

        //}

        //private void DrawData(List<double> data, double verticalStep, double horizontalStep)
        //{
        //    double lastX = 0, lastY = 0;

        //    for(int n = 0; n < data.Count;n++)
        //    {
        //        double currentX, currentY;
        //        double topMargin = (verticalStep * data[n]);

        //        Ellipse dataPoint = CreateNewEllipse();
        //        dataPoint.Margin = new Thickness(currentX = (n + 1)*horizontalStep, currentY = graph.ActualHeight - (topMargin + 30), 0, 0);
        //        graph.Children.Add(dataPoint);
        //        graph.Children.Add(GenerateConnectingLine(lastX, lastY, currentX, currentY));

        //        lastX = currentX;
        //        lastY = currentY;
        //    }
        //}

        //private TextBlock CreateNewNumber()
        //{
        //    TextBlock number;

        //    number = new TextBlock
        //    {
        //        FontSize = 15,
        //        FontWeight = FontWeight.FromOpenTypeWeight(50),
        //        FontFamily = new FontFamily("Microsoft New Tai Lue"),
        //        Foreground = graphColor,
        //        HorizontalAlignment = HorizontalAlignment.Left,
        //        VerticalAlignment = VerticalAlignment.Top
        //    };

        //    return number;
        //}

        //private Line GenerateConnectingLine(double lastX,double lastY, double currentX, double currentY)
        //{
        //    if(lastX == 0 || lastY == 0)
        //    {
        //        lastX = currentX;
        //        lastY = currentY;
        //    }

        //    Line connectingLine = new Line()
        //    {
        //        X1 = lastX + 3,
        //        Y1 = lastY + 3,
        //        X2 = currentX + 3,
        //        Y2 = currentY + 3,
        //        StrokeThickness = 2,
        //        Stroke = graphColor
        //    };

        //    return connectingLine;
        //}

        //private Ellipse CreateNewEllipse()
        //{
        //    Ellipse circle;
        //    circle = new Ellipse()
        //    {
        //        Fill = graphColor,
        //        Width = 6,
        //        Height = 6,
        //        HorizontalAlignment = HorizontalAlignment.Left,               
        //    };
        //    return circle;
        //}


    }
}
