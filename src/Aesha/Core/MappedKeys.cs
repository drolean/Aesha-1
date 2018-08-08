using System.Windows.Forms;

namespace Aesha.Core
{
    public static class MappedKeys
    {
        public static MappedKeyAction TargetLastTarget => new MappedKeyAction('G');
        public static MappedKeyAction Left => new MappedKeyAction('A');
        public static MappedKeyAction Forward => new MappedKeyAction('W');
        public static MappedKeyAction Right => new MappedKeyAction('D');
        public static MappedKeyAction Backward => new MappedKeyAction('S');
        public static MappedKeyAction ActionBar1 => new MappedKeyAction('1');
        public static MappedKeyAction ActionBar2 => new MappedKeyAction('2');
        public static MappedKeyAction ActionBar3 => new MappedKeyAction('3');
        public static MappedKeyAction ActionBar4 => new MappedKeyAction('4');
        public static MappedKeyAction ActionBar5 => new MappedKeyAction('5');
        public static MappedKeyAction ActionBar6 => new MappedKeyAction('6');
        public static MappedKeyAction ActionBar7 => new MappedKeyAction('7');
        public static MappedKeyAction ActionBar8 => new MappedKeyAction('8');
        public static MappedKeyAction ActionBar9 => new MappedKeyAction('9');
        public static MappedKeyAction ActionBar10 => new MappedKeyAction('0');
        public static MappedKeyAction ActionBar11 => new MappedKeyAction('-');
        public static MappedKeyAction ActionBar12 => new MappedKeyAction('=');
        public static MappedKeyAction PetBar1 => new MappedKeyAction('1',ctrl: true);
    }
}
