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
            if (target == null)
            {
                ScanArea();
            }

            if (target != null)
            {
                if (GetDistance(Coordinates, target.Coordinates) <= AttackRange) actionStack.Push(Delegates.CurrentAct = Action_Attack);
                else actionStack.Push(Delegates.CurrentAct = Action_Move);
            }
            else actionStack.Push(Delegates.CurrentAct = Action_Guard);
        }

        protected override void Action_Attack()
        {
            if (target != null)
            {
                destination = target.Coordinates;
                if (GetDistance(Coordinates, destination) <= AttackRange)
                {
                    if (canAttack)
                    {
                        Thread dealDamageThread = new Thread(DealDamage)
                        {
                            Name = "DealDamageThread of " + name
                        };
                        if (target != null) Monitor.Enter(target);
                        dealDamageThread.Start();
                        if (target != null) Monitor.Exit(target);
                        OnAttack();
                        actionStack.Push(Delegates.CurrentAct = Action_Attack);
                    }
                    else actionStack.Push(Delegates.CurrentAct = Action_Guard);
                }
                else actionStack.Push(Delegates.CurrentAct = Action_Move);
            }
            else actionStack.Push(Delegates.CurrentAct = Action_Guard);
        }

        protected override void Action_Move() // !!!
        {
            if (target != null)
            {
                destination = target.Coordinates;
                if (GetDistance(Coordinates, destination) > AttackRange)
                {
                    if (canMove)
                    {
                        int newX;
                        int newY;

                        if (Math.Abs(destination.X - X) > 1)
                            newX = X + (destination.X - X) / Math.Abs(destination.X - X);
                        else newX = X;

                        if (Math.Abs(destination.Y - Y) > 1)
                            newY = Y + (destination.Y - Y) / Math.Abs(destination.Y - Y);
                        else newY = Y;

                        if (Mechanics.CheckPoint(new Point(newX, newY)))
                        {
                            destinationReached = true;
                            Coordinates = new Point(newX, newY);
                        }
                    }
                    else actionStack.Push(Delegates.CurrentAct = Action_Guard);
                }
                else actionStack.Push(Delegates.CurrentAct = Action_Attack);
            }
            else actionStack.Push(Delegates.CurrentAct = Action_Guard);
        }

        protected void ScanArea()
        {
            Monitor.Enter(GameClient.actors);
            foreach (Character character in GameClient.actors)
            {
                if (Math.Pow((character.X - X), 2) +
                    Math.Pow((character.Y - Y), 2) <=
                    Math.Pow(SightRange, 2))
                {
                    if (CheckStatus(character) == "hostile")
                    {
                        if (target != null)
                        {
                            if (GetDistance(Coordinates, character.Coordinates)
                                <
                                GetDistance(Coordinates, target.Coordinates))
                                target = character;
                        }
                        else target = character;
                    }
                }
            }
            Monitor.Exit(GameClient.actors);
        }

        protected string CheckStatus(Character character)
        {
            return GameClient.settings.relationsTable[fraction][character.fraction];
        }

        protected double GetDistance(Point point1, Point point2)
        {
            return Math.Sqrt(
                Math.Pow(point2.X - point1.X, 2) +
                Math.Pow(point2.Y - point1.Y, 2));
        }

        protected override void OnDeath()
        {
            base.OnDeath();
            Mechanics.RespawnNPC(name, Mechanics.GetRandomFreePoint(), image, characteristics, fraction);
        }
    }
}
