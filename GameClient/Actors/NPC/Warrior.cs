using LevelRunner.Actors.AttackTypes;
using LevelRunner.Actors.Fractions;
using LevelRunner.Properties;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LevelRunner.Actors.NPC
{
    class AIWarrior : NPC
    {
        // Propereties
        public override string Name { get; }

        public AIWarrior(World parent, Fraction fraction, Point coordinates)
            : base(parent, fraction, UnitTypes.GroundUnit, new GroundOnly(5, 1, 1.5), 40, 1, 80, coordinates, Resources.AIWarrior)
        {
            Name = Fraction.Name + " Warrior";
        }

        protected override void Action_Guard()
        {
            Character target = Target;
            if (target == null)
            {
                ScanArea();
            }

            if (target != null)
            {
                if (Mathematics.GetDistance(Coordinates, target.Coordinates) <= UnitAttack.AttackRange) ActionStack.Push(Delegates.CurrentAct = Action_Attack);
                else ActionStack.Push(Delegates.CurrentAct = Action_Move);
            }
            else ActionStack.Push(Delegates.CurrentAct = Action_Guard);
        }

        protected override void Action_Attack()
        {
            Character target = Target;
            if (target != null)
            {
                Destination = target.Coordinates;
                if (Mathematics.GetDistance(Coordinates, Destination) <= UnitAttack.AttackRange)
                {
                    if (CanAttack)
                    {
                        if (Target != null) Monitor.Enter(Target);
                        DealDamage();
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
            Character target = Target;
            if (target != null)
            {
                Destination = target.Coordinates;
                if (Mathematics.GetDistance(Coordinates, Destination) > UnitAttack.AttackRange)
                {
                    if (CanMove)
                    {
                        int newX;
                        int newY;

                        if (Math.Abs(Destination.X - Coordinates.X) > 1)
                            newX = Coordinates.X + (Destination.X - Coordinates.X) / Math.Abs(Destination.X - Coordinates.X);
                        else newX = Coordinates.X;

                        if (Math.Abs(Destination.Y - Coordinates.Y) > 1)
                            newY = Coordinates.Y + (Destination.Y - Coordinates.Y) / Math.Abs(Destination.Y - Coordinates.Y);
                        else newY = Coordinates.Y;

                        if (Mathematics.CheckPoint(new Point(newX, newY), UnitType))
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

        protected override void DealDamage()
        {
            if (Target != null)
            {
                Target.Health = -UnitAttack.Damage;
            }
        }

        protected override void RespawnCharacter()
        {
            #region Debugging
            if (Parent.debug)
            {
                Parent.respawned++;
                Console.WriteLine("{0} respawned", Name);
            }
            #endregion

            Monitor.Enter(Parent.Actors);
            Parent.Actors.Add(new AIWarrior(Parent, Fraction, Coordinates));
            Parent.Actors.Last().CharacterThread.Start();
            Monitor.Exit(Parent.Actors);
        }
    }
}
