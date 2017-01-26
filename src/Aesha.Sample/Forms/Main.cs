using System;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using Aesha.Core;
using Aesha.Objects;
using Aesha.Objects.Infrastructure;
using Aesha.Objects.Model;

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
            
            timer1.Start();
           
   
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
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {

            if (e.RowIndex == -1) return;

            var obj = (RowUnit)dataGridView1.Rows[e.RowIndex].DataBoundItem;
            _commandManager.SetTarget(obj.Address);
            var kcd = new KeyboardCommandDispatcher();
            kcd.SendG(_process);

            Win32Imports.SetForegroundWindow(_process.MainWindowHandle);

            _commandManager.SetPlayerFacing(obj.Location);

        }

        private void button1_Click(object sender, EventArgs e)
        {
            ShowGrid();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            ShowGrid();
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

