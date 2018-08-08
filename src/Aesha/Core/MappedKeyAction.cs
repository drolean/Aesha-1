namespace Aesha.Core
{
    public class MappedKeyAction
    {
        public MappedKeyAction(char key, bool shift = false, bool ctrl = false, bool alt = false)
        {
            Key = key;
            Shift = shift;
            Ctrl = ctrl;
            Alt = alt;
        }

        public char Key { get; }
        public bool Shift { get; }
        public bool Ctrl { get; }
        public bool Alt { get; }
    }
}