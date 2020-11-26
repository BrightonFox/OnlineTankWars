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
    /// <inheritdoc cref="TankWars.JsonObjects.Projectile" />
    public class Projectile : TankWars.JsonObjects.Projectile
    {
        public Projectile(int id, int ownerId, Vector2D loc, Vector2D dir) : base()
        {
            _id = id;
            _ownerId = ownerId;
            _location = loc;
            _direction = dir;
        }

        /// <inheritdoc cref="TankWars.JsonObjects.Projectile._id" />
        public int Id { get { return _id; } private set { return; } }

        /// <inheritdoc cref="TankWars.JsonObjects.Projectile._location" />
        public Vector2D Location { get { return _location; } internal set { _location = value; } }

        /// <inheritdoc cref="TankWars.JsonObjects.Projectile._direction" />
        public Vector2D Direction { get { return _direction; } private set { return; } }

        /// <inheritdoc cref="TankWars.JsonObjects.Projectile._isDead" />
        public bool IsDead { get { return _isDead; } internal set { _isDead = value; } }

        /// <inheritdoc cref="TankWars.JsonObjects.Projectile._ownerId" />
        public int OwnerId { get { return _ownerId; } private set { return; } }
    }
}
