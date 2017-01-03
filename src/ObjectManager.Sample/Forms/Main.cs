using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;

namespace ObjectManager.Sample.Forms
{
    public partial class Main : Form
    {
        public Main()
        {
            InitializeComponent();

            var process = Process.GetProcessesByName("WoW").FirstOrDefault();
            ObjectManager.Start(process);

            var me = ObjectManager.Me;
            var players = ObjectManager.Players;
            var units = ObjectManager.Units;
        }
    }
}
