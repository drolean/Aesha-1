using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using ObjectManager.Infrastructure;
using ObjectManager.Model;

namespace ObjectManager.Sample.Forms
{
    public partial class Main : Form
    {
        private CancellationTokenSource _cancellationSource;
        private Task _pulseTask;
        private Process _process;

        public Main()
        {
            InitializeComponent();

           
        }

        private void Main_Load(object sender, System.EventArgs e)
        {
            _process = Process.GetProcessesByName("WoW").FirstOrDefault();
            ObjectManager.Start(_process);
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
            ObjectManager.SetTarget(obj.Address);
            var kcd = new KeyboardCommandDispatcher();
            kcd.SendG(_process);

            Win32Imports.SetForegroundWindow(_process.MainWindowHandle);

            //ObjectManager.SetPlayerFacing(obj.Location);

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

