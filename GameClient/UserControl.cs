using System.Collections;
using System.Windows.Forms;

namespace LevelRunner
{
    internal static class UserControl
    {
        private static readonly Hashtable _keyTable = new Hashtable();

        public static bool KeyPressed(Keys key)
        {
            return _keyTable[key] == null ? false : (bool)_keyTable[key];
        }

        public static void ChangeState(Keys key, bool state)
        {
            _keyTable[key] = state;
        }

        public static void Reset()
        {
            if (_keyTable.Count != 0)
            {
                foreach(Keys key in _keyTable.Keys)
                {
                    _keyTable[key] = false;
                }
            }
        }
    }
}
