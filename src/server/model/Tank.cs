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
 *   + <>
 * 
 * About:
 *   <>
 */

using System;
using TankWars.MathUtils;


namespace TankWars.Server.Model
{
    /// <inheritdoc cref="TankWars.JsonObjects.Tank" />
    public class Tank : TankWars.JsonObjects.Tank
    {

        public Tank(int id, string playerName, int respawnDelay) : base()
        {
            _id = id;
            _playerName = playerName;
            RespawnDelay = respawnDelay;
        }


        private int _framesTillRespawn;
        public int FramesTillRespawn
        {
            get
            {
                if (_health <= 0 && --_framesTillRespawn <= 0)
                {
                    _framesTillRespawn = RespawnDelay;
                    return 0;
                }
                return _framesTillRespawn;
            }
            private set { return; }
        }

        /// <inheritdoc cref="World.RespawnDelay"/>
        public int RespawnDelay { get; private set; }

        /// <summary>
        /// Number of beams a tank can fire.
        /// Increased by 1 when tank picks up a <see cref="Powerup"/>.
        /// </summary>
        public int BeamChargeCount {get; internal set;} 



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
