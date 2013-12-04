using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Xml;
using SmartWatch.Core;
using SmartWatch.Core.Mocks;
using WobbrockLib;

namespace SmartWatch.Training
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private IArduino _arduino;

        private List<TimePointF> _points;

        public MainWindow()
        {
            _points = new List<TimePointF>();

            //TODO - Remove comment for real arduino
            _arduino = new Arduino("COM4");
            _arduino.DataRecieved += ArduinoDataRecieved;

            InitializeComponent();
        }

        private void StartButtonClick(object sender, RoutedEventArgs e)
        {
            _arduino.Connect();

            //TODO - Comment out for real arduino
            //_arduino = new ArduinoMock();

            if (String.IsNullOrWhiteSpace(FilenameTextBox.Text))
            {
                MessageBox.Show("Enter a name first");
                return;
            }

            IsRecordingTextBlock.Visibility = Visibility.Visible;
        }


        private void StopButtonClick(object sender, RoutedEventArgs e)
        {
            _arduino.Dispose();

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
        }


        private void ArduinoDataRecieved(object sender, List<TimePointF> e)
        {
            _points.Add(e[0]);
        }
    }
}