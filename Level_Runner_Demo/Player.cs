using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Level_Runner_Demo
{
    class Player : Character, IDisposable // !!!
    {
        // Variables

        public Player(string name, Point coordinates, Bitmap image, Characteristics characteristics, string fraction)
            : base(name, coordinates, image, characteristics, fraction)
        {
            destination = new Point(X, Y);
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
            Mechanics.RespawnPlayer(name, Mechanics.GetRandomFreePoint(), image, characteristics, fraction);
        }
    }
}
