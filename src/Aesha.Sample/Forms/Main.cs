using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Aesha.Core;
using Aesha.Objects;
using Aesha.Objects.Infrastructure;
using Aesha.Objects.Model;
using Aesha.Robots;

namespace Aesha.Forms
{
    public partial class Main : Form
    {

        private Process _process;
        private CommandManager _commandManager;
        private ProcessMemoryReader _reader;

        public Main()
        {
            InitializeComponent();

           
        }

        private void Main_Load(object sender, System.EventArgs e)
        {
            _process = Process.GetProcessesByName("WoW").FirstOrDefault();
            ObjectManager.Start(_process);
            
            _reader = new ProcessMemoryReader(_process);

            _commandManager = new CommandManager(_process,_reader);
            

            var bot = new Fubars();
            bot.Start(_process);

           // timer1.Start();


            //var botTask = new Task(() =>
            //{
            //    var bot = new Fubars();
            //    bot.Start(_process);
            //});

            //botTask.Start();


        }

        private void ShowGrid()
        {
            dataGridView1.DataSource = ObjectManager.Units.Where(u => u.Health.Current > 0).Select(f => new RowUnit()
            {
                Name = f.Name,
                Health = f.Health.Current,
                Level = f.Level,
                Target = f.Target?.Name,
                Tapped = f.Attributes.TappedByMe,
                Location = f.Location,
                Distance = f.Distance,
                Address = f.Guid

            }).OrderBy(ru => ru.Distance).ToList();
            dataGridView1.Show();

            var autoTargetObj = ObjectManager.Units
                    .Where(u => u.Health.Current > 0)
                    .OrderBy(u => u.Distance)
                    .FirstOrDefault(u => u.Name.Contains(textBox1.Text));

            if (autoTargetObj != null &&
                ObjectManager.Me.Target == null &&
                textBox1.Text.Length > 0 &&
                autoTargetObj.Health.Percentage == 100 &&
                autoTargetObj.Attributes.Tapped == false)
                SetTarget(autoTargetObj.Guid, autoTargetObj.Location);

        }

        private void SetTarget(ulong address, Location location)
        {
            _commandManager.SetTarget(address);
            _commandManager.SendG(_process);
            Win32Imports.SetForegroundWindow(_process.MainWindowHandle);
            _commandManager.SetPlayerFacing(location);
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {

            if (e.RowIndex == -1) return;

            var obj = (RowUnit)dataGridView1.Rows[e.RowIndex].DataBoundItem;
            _commandManager.SetTarget(obj.Address);
            _commandManager.SendG(_process);

            Win32Imports.SetForegroundWindow(_process.MainWindowHandle);

            _commandManager.SetPlayerFacing(obj.Location);


        }

        private void button1_Click(object sender, EventArgs e)
        {
         
            var botTask = new Task(() =>
            {
                var bot = new Fubars();
                bot.Start(_process);
            });
               
            botTask.Start();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            ShowGrid();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var recorder = new WaypathRecorder(_process);
            recorder.ShowDialog(this);
        }
    }

    public class RowUnit
    {
        public string Name { get; set; }
        public int Health { get; set; }
        public int Level { get; set; }
        public string Target { get; set; }
        public bool Tapped { get; set; }
        public ulong Address { get; set; }
        public Location Location { get; set; }
        public float Distance { get; set; }
    }
}

