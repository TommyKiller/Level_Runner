﻿using LevelRunner.Actors.AttackTypes;
using LevelRunner.Actors.Fractions;
using LevelRunner.GameWorld.Map;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

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
                base.Coordinates = value;
                if (DestinationReached) OnMove();
            }
        }
        protected Point Destination { get; set; }
        protected bool DestinationReached { get; set; }
        protected bool CanAttack { get; set; }
        protected bool CanMove { get; set; }
        protected System.Timers.Timer CoolDownTimer { get; }
        protected System.Timers.Timer MovementSpeedTimer { get; }
        protected Character Target { get; set; }

        public NPC(World parent, Fraction fraction, UnitTypes unitType, UnitAttack attackType,
            int health, int speed, int sightRange, Point coordinates, Bitmap image)
            : base(parent, fraction, unitType, attackType, health, speed, sightRange, coordinates, image)
        {
            CanAttack = true;
            CanMove = true;
            Alive = true;
            CoolDownTimer = new System.Timers.Timer
            {
                Interval = UnitAttack.AttackSpeed * 1000,
                Enabled = false,
                AutoReset = false
            };
            MovementSpeedTimer = new System.Timers.Timer
            {
                Interval = 1000 / Speed,
                Enabled = false,
                AutoReset = false
            };

            // Events
            DeleteTargetEvent += DeleteTarget;
            CoolDownTimer.Elapsed += CoolDownTimer_Elapsed;
            MovementSpeedTimer.Elapsed += MovementSpeedTimer_Elapsed;
            Parent.OnTimer += Action_Execute;
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

        protected abstract void Action_Guard();

        protected abstract void Action_Attack();

        protected abstract void Action_Move();

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
                            if (Mathematics.GetDistance(Coordinates, character.Coordinates)
                                <
                                Mathematics.GetDistance(Coordinates, Target.Coordinates))
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
        
        protected void DeleteTarget(Character target)
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

        protected void CoolDownTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            CoolDownTimer.Stop();
            CanAttack = true;
        }

        protected void MovementSpeedTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            MovementSpeedTimer.Stop();
            CanMove = true;
        }

        protected void OnAttack()
        {
            CanAttack = false;
            CoolDownTimer.Start();
        }

        protected void OnMove()
        {
            CanMove = false;
            DestinationReached = false;
            MovementSpeedTimer.Start();
        }

        protected override void OnDeath()
        {
            #region Debugging
            if (Parent.debug)
            {
                Parent.died++;
                Console.WriteLine("{0} died", Name);
            }
            #endregion
            
            DeleteTargetEvent(this);

            // Unsubscribe events
            DeleteTargetEvent -= DeleteTarget;
            CoolDownTimer.Elapsed -= CoolDownTimer_Elapsed;
            MovementSpeedTimer.Elapsed -= MovementSpeedTimer_Elapsed;
            Parent.OnTimer -= Action_Execute;

            RespawnCharacter();
        }
    }
}