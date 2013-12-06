using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Xml;
using SmartWatch.Core;
using SmartWatch.Core.Arduino;
using WobbrockLib;

namespace SmartWatch.Training
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly IArduino _arduino;
        private readonly List<TimePointF> _points;

        private string _trainingType;

        public MainWindow()
        {
            _points = new List<TimePointF>();

            //TODO - Remove comment for real arduino
            _arduino = new Arduino("COM3");

            InitializeComponent();
        }

        private void HorizontalScrollTrainingClick(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrWhiteSpace(FilenameTextBox.Text))
            {
                MessageBox.Show("Enter a name first");
                return;
            }
            _trainingType = "horizontal";

            _arduino.DataRecieved += ArduinoDataRecieved;


            IsRecordingTextBlock.Visibility = Visibility.Visible;
        }


        private void StopButtonClick(object sender, RoutedEventArgs e)
        {
            var filename = FilenameTextBox.Text + ".xml";
            var name = FilenameTextBox.Text;

            if (_points.Count > 1)
            {
                XmlTextWriter writer = null;
                try
                {
                    // save the prototype as an Xml file
                    writer = new XmlTextWriter(filename, Encoding.UTF8) {Formatting = Formatting.Indented};
                    writer.WriteStartDocument(true);
                    writer.WriteStartElement("Gesture");
                    writer.WriteAttributeString("Name", name);
                    writer.WriteAttributeString("NumPts", XmlConvert.ToString(_points.Count));
                    writer.WriteAttributeString("Millseconds",
                        XmlConvert.ToString(_points[_points.Count - 1].Time - _points[0].Time));
                    writer.WriteAttributeString("AppName", Assembly.GetExecutingAssembly().GetName().Name);
                    writer.WriteAttributeString("AppVer", Assembly.GetExecutingAssembly().GetName().Version.ToString());
                    writer.WriteAttributeString("Date", DateTime.Now.ToLongDateString());
                    writer.WriteAttributeString("TimeOfDay", DateTime.Now.ToLongTimeString());

                    // write out the raw individual _points
                    foreach (var p in _points)
                    {
                        writer.WriteStartElement("Point");
                        writer.WriteAttributeString("X", XmlConvert.ToString(p.X));
                        writer.WriteAttributeString("Y", XmlConvert.ToString(p.Y));
                        writer.WriteAttributeString("T", XmlConvert.ToString(p.Time));
                        writer.WriteEndElement(); // <Point />
                    }

                    writer.WriteEndDocument(); // </Gesture>
                }
                catch (XmlException xex)
                {
                    Console.Write(xex.Message);
                }
                catch (Exception ex)
                {
                    Console.Write(ex.Message);
                }
                finally
                {
                    if (writer != null)
                        writer.Close();
                }
            }


            IsRecordingTextBlock.Visibility = Visibility.Hidden;

            // Clear text fields to avoid overwriting files
            FilenameTextBox.Text = String.Empty;
            _points.Clear();
            _arduino.DataRecieved -= ArduinoDataRecieved;
        }


        private void ArduinoDataRecieved(object sender, List<TimePointF> e)
        {
            var largest = e[0];
            for (var i = 1; i < e.Count; i++)
            {
                if (e[i].X > largest.X)
                    largest = e[i];
            }
            _points.Add(largest);
        }

        private void VerticalScrollTrainingClick(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrWhiteSpace(FilenameTextBox.Text))
            {
                MessageBox.Show("Enter a name first");
                return;
            }


            _trainingType = "vertical";
            _arduino.DataRecieved += ArduinoDataRecieved;


            IsRecordingTextBlock.Visibility = Visibility.Visible;
        }
    }
}