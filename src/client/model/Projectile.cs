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
 *   + v1.0 - Submittal - 2020/11/21
 * 
 * About:
 *   An object representing a Projectile that is tied to a
 *   tank. Also contains the Json logic used to deserialize
 *   projectile Json objects.
 */

using System;
using TankWars.MathUtils;

namespace TankWars.Client.Model
{
    /// <inheritdoc cref="TankWars.JsonObjects.Projectile" />
    public class Projectile : TankWars.JsonObjects.Projectile
    {
        /// <inheritdoc cref="TankWars.JsonObjects.Projectile._id" />
        public int Id { get { return _id; } private set {return;} }

        /// <inheritdoc cref="TankWars.JsonObjects.Projectile._location" />
        public Vector2D Location { get { return _location; } private set {return;} }

        /// <inheritdoc cref="TankWars.JsonObjects.Projectile._direction" />
        public Vector2D Direction { get { return _direction; } private set {return;} }

        /// <inheritdoc cref="TankWars.JsonObjects.Projectile._isDead" />
        public bool IsDead { get {return _isDead;} private set {return;} }

        /// <inheritdoc cref="TankWars.JsonObjects.Projectile._ownerId" />
        public int OwnerId { get { return _ownerId; } private set {return;} }
    }
}
