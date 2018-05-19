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
                if (Mathematics.GetDistance(Coordinates, Target.Coordinates) <= AttackRange) ActionStack.Push(Delegates.CurrentAct = Action_Attack);
                else ActionStack.Push(Delegates.CurrentAct = Action_Move);
            }
            else ActionStack.Push(Delegates.CurrentAct = Action_Guard);
        }

        protected override void Action_Attack()
        {
            if (Target != null)
            {
                Destination = Target.Coordinates;
                if (Mathematics.GetDistance(Coordinates, Destination) <= AttackRange)
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
                if (Mathematics.GetDistance(Coordinates, Destination) > AttackRange)
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

                        if (Mathematics.CheckPoint(new Point(newX, newY)))
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
            Monitor.Enter(Program.World.actors);
            foreach (Character character in Program.World.actors)
            {
                if (Math.Pow((character.X - X), 2) +
                    Math.Pow((character.Y - Y), 2) <=
                    Math.Pow(SightRange, 2))
                {
                    if (CheckStatus(character) == "hostile")
                    {
                        if (Target != null)
                        {
                            if (Mathematics.GetDistance(Coordinates, character.Coordinates)
                                <
                                Mathematics.GetDistance(Coordinates, Target.Coordinates))
                                Target = character;
                        }
                        else Target = character;
                    }
                }
            }
            Monitor.Exit(Program.World.actors);
        }

        protected string CheckStatus(Character character)
        {
            return Program.World.settings.relationsTable[Fraction][character.Fraction];
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

            Monitor.Enter(Program.World.actors);
            Program.World.actors.Add(new NPC(Name, Coordinates, Image, characteristics, Fraction));
            Program.World.actors.Last().CharacterThread.Start();
            Monitor.Exit(Program.World.actors);
        }
    }
}
