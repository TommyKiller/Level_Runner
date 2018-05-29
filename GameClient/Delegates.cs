using LevelRunner.Actors;
using System.Windows.Forms;

namespace LevelRunner
{
    public static class Delegates
    {
        public delegate void EventDelegate();
        public delegate void ActDelegate();
        public delegate void VolumeChangeEventDelegate(float value);
        public delegate void FormBorderStyleChangeEventDelegate(FormBorderStyle value);
        public delegate void DeleteTargetDelegate(Character target);
        public static ActDelegate CurrentAct;
    }
}
