using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LevelRunner.Actors
{
    class Player : Character, IDisposable // !!!
    {
        // Variables

        public Player(string name, Point coordinates, Bitmap image, Characteristics characteristics, string fraction)
            : base(name, coordinates, image, characteristics, fraction)
        {

        }

        protected override void Action_Guard()
        {
        }

        protected override void Action_Attack()
        {
        }

        protected override void Action_Move()
        {
        }

        protected override void OnDeath()
        {
            base.OnDeath();
            RespawnCharacter();
        }

        protected override void RespawnCharacter()
        {
            // Debugging
            if (Program.World.debug)
            {
                Program.World.respawned++;
                Console.WriteLine("{0} respawned", Name);
            }

            Monitor.Enter(Program.World.Actors);
            Program.World.Actors.Add(new Player(Name, Coordinates, Image, characteristics, Fraction));
            Program.World.Actors.Last().CharacterThread.Start();
            Monitor.Exit(Program.World.Actors);
        }
    }
}
