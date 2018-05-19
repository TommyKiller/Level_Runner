using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Level_Runner_Demo
{
    class NPC : Character
    {
        // Variables

        public NPC(string name, Point coordinates, Bitmap image, Characteristics characteristics, string fraction)
            : base(name, coordinates, image, characteristics, fraction)
        {

        }

        protected override void Action_Guard()
        {
            if (Target == null)
            {
                ScanArea();
            }

            if (Target != null)
            {
                if (Mechanics.GetDistance(Coordinates, Target.Coordinates) <= AttackRange) ActionStack.Push(Delegates.CurrentAct = Action_Attack);
                else ActionStack.Push(Delegates.CurrentAct = Action_Move);
            }
            else ActionStack.Push(Delegates.CurrentAct = Action_Guard);
        }

        protected override void Action_Attack()
        {
            if (Target != null)
            {
                Destination = Target.Coordinates;
                if (Mechanics.GetDistance(Coordinates, Destination) <= AttackRange)
                {
                    if (CanAttack)
                    {
                        Thread dealDamageThread = new Thread(DealDamage)
                        {
                            Name = "DealDamageThread of " + Name
                        };
                        if (Target != null) Monitor.Enter(Target);
                        dealDamageThread.Start();
                        if (Target != null) Monitor.Exit(Target);
                        OnAttack();
                        ActionStack.Push(Delegates.CurrentAct = Action_Attack);
                    }
                    else ActionStack.Push(Delegates.CurrentAct = Action_Guard);
                }
                else ActionStack.Push(Delegates.CurrentAct = Action_Move);
            }
            else ActionStack.Push(Delegates.CurrentAct = Action_Guard);
        }

        protected override void Action_Move() // !!!
        {
            if (Target != null)
            {
                Destination = Target.Coordinates;
                if (Mechanics.GetDistance(Coordinates, Destination) > AttackRange)
                {
                    if (CanMove)
                    {
                        int newX;
                        int newY;

                        if (Math.Abs(Destination.X - X) > 1)
                            newX = X + (Destination.X - X) / Math.Abs(Destination.X - X);
                        else newX = X;

                        if (Math.Abs(Destination.Y - Y) > 1)
                            newY = Y + (Destination.Y - Y) / Math.Abs(Destination.Y - Y);
                        else newY = Y;

                        if (Mechanics.CheckPoint(new Point(newX, newY)))
                        {
                            DestinationReached = true;
                            Coordinates = new Point(newX, newY);
                        }
                    }
                    else ActionStack.Push(Delegates.CurrentAct = Action_Guard);
                }
                else ActionStack.Push(Delegates.CurrentAct = Action_Attack);
            }
            else ActionStack.Push(Delegates.CurrentAct = Action_Guard);
        }

        protected void ScanArea()
        {
            Monitor.Enter(Program.GameClient.actors);
            foreach (Character character in Program.GameClient.actors)
            {
                if (Math.Pow((character.X - X), 2) +
                    Math.Pow((character.Y - Y), 2) <=
                    Math.Pow(SightRange, 2))
                {
                    if (CheckStatus(character) == "hostile")
                    {
                        if (Target != null)
                        {
                            if (Mechanics.GetDistance(Coordinates, character.Coordinates)
                                <
                                Mechanics.GetDistance(Coordinates, Target.Coordinates))
                                Target = character;
                        }
                        else Target = character;
                    }
                }
            }
            Monitor.Exit(Program.GameClient.actors);
        }

        protected string CheckStatus(Character character)
        {
            GameClient.Settings settings = Program.GameClient.settings;
            return settings.relationsTable[Fraction][character.Fraction];
        }

        protected override void OnDeath()
        {
            base.OnDeath();
            Mechanics.RespawnNPC(Name, Mechanics.GetRandomFreePoint(), Image, characteristics, Fraction);
        }
    }
}
