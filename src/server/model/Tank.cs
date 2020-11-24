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

        /// <inheritdoc cref="TankWars.JsonObjects.Tank._id" />
        public int Id { get { return _id; } internal set { _id = value; } }

        /// <inheritdoc cref="TankWars.JsonObjects.Tank._playerName" />
        public string PlayerName { get {return _playerName;} protected set {return;}}

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
