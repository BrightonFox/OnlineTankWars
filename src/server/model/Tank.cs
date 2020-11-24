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


namespace TankWars.Client.Model
{
    /// <inheritdoc cref="TankWars.JsonObjects.Tank" />
    public class Tank : TankWars.JsonObjects.Tank
    {

        /// <inheritdoc cref="TankWars.JsonObjects.Tank._id" />
        public int Id { get {return _id;} private set {return;}}

        /// <inheritdoc cref="TankWars.JsonObjects.Tank._playerName" />
        public string PlayerName { get {return _playerName;} protected set {return;}}

        /// <inheritdoc cref="TankWars.JsonObjects.Tank._location" />
        public Vector2D Location { get {return _location;} private set {return;}}
        
        /// <inheritdoc cref="TankWars.JsonObjects.Tank._turretDirection" />
        public Vector2D TurretDirection { get {return _turretDirection;} private set {return;}}

        /// <inheritdoc cref="TankWars.JsonObjects.Tank._bodyDirection" />
        public Vector2D Direction { get {return _bodyDirection;} private set {return;}}

        /// <inheritdoc cref="TankWars.JsonObjects.Tank._score" />
        public int Score { get { return _score; } private set {return;} }

        /// <inheritdoc cref="TankWars.JsonObjects.Tank._health" />
        public int Health { get { return _health; } private set {return;} }

        /// <inheritdoc cref="TankWars.JsonObjects.Tank._isDead" />
        public bool IsDead { get { return _isDead; } private set {return;} }

        /// <inheritdoc cref="TankWars.JsonObjects.Tank._isDisconnected" />
        public bool IsDisconnected { get { return _isDisconnected; } private set {return;} }

        /// <inheritdoc cref="TankWars.JsonObjects.Tank._hasJoined" />
        public bool HasJoined { get { return _hasJoined; } private set {return;} }
    }
}
