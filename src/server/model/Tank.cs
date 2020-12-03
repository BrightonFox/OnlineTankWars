/**
 * Team: JustBroken
 * Authors: 
 *   + Andrew Osterhout (u1317172)
 *   + Brighton Fox (u0981544)
 * Organization: University of Utah
 *   Course: CS3500: Software Practice
 *   Semester: Fall 2020
 * 
 * Version Data: 
 *   + v1.0 - submittal - 2020/12/2
 *   
 * About:
 *   An object representing a Tank in the world that
 *   is controlled by a client. Also contains the
 *   logic used to convert the tank into JSON for the clients.
 */

using System;
using TankWars.MathUtils;


namespace TankWars.Server.Model
{
    /// <inheritdoc cref="TankWars.JsonObjects.Tank" />
    public class Tank : TankWars.JsonObjects.Tank
    {

        /// <summary>
        /// The tank object as the server needs to understand it.
        /// </summary>
        /// <param name="id">the ID of the client & tank.</param>
        /// <param name="playerName">The strign representation of 16 chars that will be displayed as teh player name</param>
        public Tank(int id, string playerName) : base()
        {
            _id = id;
            _playerName = playerName.Substring(0, (playerName.Length>16) ? 16 : PlayerName.Length);
            _bodyDirection = new Vector2D(0, -1);
            _turretDirection = new Vector2D(0, -1);
            BeamChargeCount = 0;
            _framesTillRespawn = World.RespawnDelay;
        }


        private int _framesTillRespawn;

        /// <summary>
        /// The number of frames a tank must wait before being respawned.
        /// <para>
        /// NOTE: this property automatically decrements this counter every time it's checked
        ///  and <see cref="Tank.Health"/> is 0,
        ///  and resets to <see cref="World.RespawnDelay"/>,
        ///  when it reaches 0.
        /// </para>
        /// </summary>
        public int FramesTillRespawn
        {
            get
            {
                if (_health <= 0 && --_framesTillRespawn <= 0)
                {
                    _framesTillRespawn = World.RespawnDelay;
                    return 0;
                }
                return _framesTillRespawn;
            }
            private set { return; }
        }

        /// <summary>
        /// Number of beams a tank can fire.
        /// Increased by 1 when tank picks up a <see cref="Powerup"/>.
        /// </summary>
        public int BeamChargeCount { get; internal set; }


        private int _turretCoolDown = 0;

        /// <summary>
        /// The number of frames before the <see cref="Tank"/> 
        ///  can fire a <see cref="Projectile"/> again.
        /// </summary>
        public int TurretCoolDown
        {
            get { return _turretCoolDown; }
            private set
            {
                if (value <= 0)     //stops the cooldown countdown at 0
                {
                    _turretCoolDown = 0;
                }
                else
                    _turretCoolDown = value;
            }
        }

        /// <summary>
        /// States wether the tank is able to fire or not 
        ///  (determined by <see cref="World.ProjectileFireDelay"/>)
        /// </summary>
        public bool CanFire
        {
            get
            {
                return _turretCoolDown == 0;
            }
            internal set
            {
                if (!value)
                    _turretCoolDown = World.ProjectileFireDelay;
            }
        }


        /// <summary>
        /// Perform all internal tasks that a tank must do every frame.
        /// For example decrementing the cooldown on <see cref="Tank.TurretCoolDown"/>.
        /// </summary>
        internal void TickTank()
        {
            TurretCoolDown--;
        }



        /// <inheritdoc cref="TankWars.JsonObjects.Tank._id" />
        public int Id { get { return _id; } private set { return; } }

        /// <inheritdoc cref="TankWars.JsonObjects.Tank._playerName" />
        public string PlayerName { get { return _playerName; } private set { return; } }

        /// <inheritdoc cref="TankWars.JsonObjects.Tank._location" />
        public Vector2D Location { get { return _location; } internal set { _location = value; } }

        /// <inheritdoc cref="TankWars.JsonObjects.Tank._turretDirection" />
        public Vector2D TurretDirection { get { return _turretDirection; } internal set { _turretDirection = value; } }

        /// <inheritdoc cref="TankWars.JsonObjects.Tank._bodyDirection" />
        public Vector2D Direction { get { return _bodyDirection; } internal set { _bodyDirection = value; } }

        /// <inheritdoc cref="TankWars.JsonObjects.Tank._score" />
        public int Score { get { return _score; } internal set { _score = value; } }

        /// <inheritdoc cref="TankWars.JsonObjects.Tank._health" />
        public int Health { get { return _health; } internal set { _health = value; } }

        /// <inheritdoc cref="TankWars.JsonObjects.Tank._isDead" />
        public bool IsDead { get { return _isDead; } internal set { _isDead = value; } }

        /// <inheritdoc cref="TankWars.JsonObjects.Tank._isDisconnected" />
        public bool IsDisconnected { get { return _isDisconnected; } internal set { _isDisconnected = value; } }

        /// <inheritdoc cref="TankWars.JsonObjects.Tank._hasJoined" />
        public bool HasJoined { get { return _hasJoined; } internal set { _hasJoined = value; } }
    }
}
