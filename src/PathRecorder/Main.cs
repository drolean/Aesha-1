using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using Aesha.Core;
using Aesha.Domain;


namespace PathRecorder
{
    public partial class Main : Form
    {
        private readonly Process _process;
        private readonly List<Location> _path;
        private bool _recording;

        public Main(Process process)
        {
            _process = process;
            _path = new List<Location>();

            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ObjectManager.Start(_process);

            var pathTask = new Task(async () =>
            {
                await Record(ObjectManager.Me.Location);
            });
            pathTask.Start();
        }

        private async Task Record(Location startPosition)
        {
            _recording = true;
            while (_recording)
            {
                var currentLocation = ObjectManager.Me.Location;
                if (Equals(currentLocation, startPosition))
                {
                    await Task.Delay(1000);
                    startPosition = currentLocation;
                    continue;
                }

                _path.Add(currentLocation);

                this.Invoke(new MethodInvoker(delegate ()
                {
                    listBox1.Items.Add($"{currentLocation.X},{currentLocation.Y}");
                }));

                startPosition = currentLocation;
                await Task.Delay(500);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            _recording = false;

            var fileName = $"{DateTime.UtcNow:yyyy-MM-dd hh-mm-ss}.path";
            var file = File.Open(fileName, FileMode.Create);
            var streamWriter = new StreamWriter(file);

            foreach (var location in _path)
            {
                streamWriter.WriteLine($"{location.X},{location.Y}");
            }

            streamWriter.Flush();
            streamWriter.Close();
            file.Close();
        }
    }
}
