using LevelRunner.Actors.AttackTypes;
using LevelRunner.Mathematics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;

namespace LevelRunner.Actors.NPC
{
    abstract class NPC : Character
    {
        // Events
        private static event Delegates.DeleteTargetDelegate DeleteTargetEvent;

        // Properties
        protected Point Destination { get; set; }
        protected int SightRange { get; set; }

        public NPC(World parent, Fraction.Fractions fraction, Point coordinates, Bitmap image)
            : base(parent, fraction, coordinates, image)
        {
            #region Events
            DeleteTargetEvent += NPC_DeleteTarget;
            #endregion
        }

        protected override void Action_Execute()
        {
            if (ActionStack.Count == 0)
            {
                ActionStack.Push(Delegates.CurrentAct = Action_Guard);
            }
            ActionThread = new Thread(new ThreadStart(ActionStack.Pop()))
            {
                Name = Name,
                IsBackground = false
            };
            ActionThread.Start();
        }

        protected virtual void Action_Guard()
        {
            Character target = Target;
            if (target == null)
            {
                ScanArea();
            }

            if (target != null)
            {
                if (Calculate.GetDistance(Coordinates, target.Coordinates) <= UnitAttack.AttackRange) ActionStack.Push(Delegates.CurrentAct = Action_Attack);
                else ActionStack.Push(Delegates.CurrentAct = Action_Move);
            }
            else ActionStack.Push(Delegates.CurrentAct = Action_Guard);
        }

        protected virtual void Action_Attack()
        {
            Character target = Target;
            if (target != null)
            {
                Destination = target.Coordinates;
                if (Calculate.GetDistance(Coordinates, Destination) <= UnitAttack.AttackRange)
                {
                    if (CanAttack)
                    {
                        if (Target != null) Monitor.Enter(Target);
                        DealDamage();
                        if (Target != null) Monitor.Exit(Target);
                        Character_OnAttack();
                        ActionStack.Push(Delegates.CurrentAct = Action_Attack);
                    }
                    else ActionStack.Push(Delegates.CurrentAct = Action_Guard);
                }
                else ActionStack.Push(Delegates.CurrentAct = Action_Move);
            }
            else ActionStack.Push(Delegates.CurrentAct = Action_Guard);
        }

        protected virtual void Action_Move()
        {
            Character target = Target;
            if (target != null)
            {
                Destination = target.Coordinates;
                if (Calculate.GetDistance(Coordinates, Destination) > UnitAttack.AttackRange)
                {
                    int newX;
                    int newY;

                    if (Math.Abs(Destination.X - Coordinates.X) > 1)
                        newX = Coordinates.X + (Destination.X - Coordinates.X) / Math.Abs(Destination.X - Coordinates.X);
                    else newX = Coordinates.X;

                    if (Math.Abs(Destination.Y - Coordinates.Y) > 1)
                        newY = Coordinates.Y + (Destination.Y - Coordinates.Y) / Math.Abs(Destination.Y - Coordinates.Y);
                    else newY = Coordinates.Y;

                    if (Calculate.CheckPoint(new Point(newX, newY), UnitType))
                    {
                        Coordinates = new Point(newX, newY);
                    }
                    else ActionStack.Push(Delegates.CurrentAct = Action_Guard);
                }
                else ActionStack.Push(Delegates.CurrentAct = Action_Attack);
            }
            else ActionStack.Push(Delegates.CurrentAct = Action_Guard);
        }

        protected abstract void DealDamage();

        protected void ScanArea()
        {
            Monitor.Enter(Parent.Actors);
            foreach (Character character in Parent.Actors)
            {
                if ((Math.Pow((character.Coordinates.X - Coordinates.X), 2) +
                    Math.Pow((character.Coordinates.Y - Coordinates.Y), 2) <=
                    Math.Pow(SightRange, 2)) && (UnitAttack.EligibleTargets.Contains(character.UnitType)))
                {
                    if (CheckStatus(character) == Fraction.Relations.Hostile)
                    {
                        if (Target != null)
                        {
                            if (Calculate.GetDistance(Coordinates, character.Coordinates)
                                <
                                Calculate.GetDistance(Coordinates, Target.Coordinates))
                                Target = character;
                        }
                        else Target = character;
                    }
                    else if ((CheckStatus(character) == Fraction.Relations.Friendly) && (CheckStatus(character.Target) == Fraction.Relations.Hostile))
                    {
                        if (Target != null)
                        {
                            if (Calculate.GetDistance(Coordinates, character.Target.Coordinates)
                                <
                                Calculate.GetDistance(Coordinates, Target.Coordinates))
                                Target = character.Target;
                        }
                        else Target = character.Target;
                    }
                }
            }
            Monitor.Exit(Parent.Actors);
        }

        protected Fraction.Relations CheckStatus(Character character)
        {
            return Fraction.RelationsList[FractionName][character.FractionName];
        }
        
        protected void NPC_DeleteTarget(Character target)
        {
            if (Target == target)
            {
                Target = null;
            }
        }

        protected override void Character_OnDeath()
        {
            #region Unsubscribe events
            DeleteTargetEvent -= NPC_DeleteTarget;
            #endregion

            base.Character_OnDeath();

            DeleteTargetEvent(this);
        }
    }
}
