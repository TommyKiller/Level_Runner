using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LevelRunner.Actors;

namespace LevelRunner
{
    public static class Delegates
    {
        public delegate void EventDelegate();
        public delegate void ActDelegate();
        public delegate void DeleteTargetDelegate(Character target);
        public static ActDelegate CurrentAct;
    }
}
