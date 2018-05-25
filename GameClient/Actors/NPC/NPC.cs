using LevelRunner.Actors.AttackTypes;
using LevelRunner.Actors.Fractions;
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

        // Propereties
        public override Point Coordinates
        {
            get => base.Coordinates;
            protected set
            {
                if (CanMove)
                {
                    base.Coordinates = value;
                    Character_OnMove();
                }
            }
        }
        protected Point Destination { get; set; }
        protected Character Target { get; set; }
        protected int SightRange { get; set; }

        public NPC(World parent, Fraction fraction, Point coordinates, Bitmap image)
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

        protected abstract void RespawnCharacter();

        protected void ScanArea()
        {
            Monitor.Enter(Parent.Actors);
            foreach (Character character in Parent.Actors)
            {
                if ((Math.Pow((character.Coordinates.X - Coordinates.X), 2) +
                    Math.Pow((character.Coordinates.Y - Coordinates.Y), 2) <=
                    Math.Pow(SightRange, 2)) && (UnitAttack.EligibleTargets.Contains(character.UnitType)))
                {
                    if (CheckStatus(character))
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
                }
            }
            Monitor.Exit(Parent.Actors);
        }

        protected bool CheckStatus(Character character)
        {
            if (Fraction.Name != character.Fraction.Name)
            {
                if (Fraction.RelationsList[character.Fraction.Name] == Relations.Hostile)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
        
        protected void NPC_DeleteTarget(Character target)
        {
            #region Debuggin
            if (Parent.debug)
            {
                Console.WriteLine("Delete target for {0} is managed by {1}", Name, Thread.CurrentThread.Name);
            }
            #endregion

            if (Target == target)
            {
                Target = null;
            }
        }

        protected override void Character_OnDeath()
        {
            base.Character_OnDeath();
            
            DeleteTargetEvent(this);

            #region Unsubscribe events
            DeleteTargetEvent -= NPC_DeleteTarget;
            #endregion

            RespawnCharacter();
        }
    }
}
