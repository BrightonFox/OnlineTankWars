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
 *   An object representing a Powerup that spawns within the
 *   world. Also contains the Json logic used to deserialize
 *   powerup Json objects.
 */

using System;
using TankWars.MathUtils;

namespace TankWars.Client.Model
{
    /// <inheritdoc cref="TankWars.JsonObjects.Powerup" />
    public class Powerup : TankWars.JsonObjects.Powerup
    {
        /// <inheritdoc cref="TankWars.JsonObjects.Powerup._id" />
        public int Id { get { return _id; } private set {return;} }

        /// <inheritdoc cref="TankWars.JsonObjects.Powerup._location" />
        public Vector2D Location { get { return _location; } private set {return;} }

        /// <inheritdoc cref="TankWars.JsonObjects.Powerup._isDead" />
        public bool IsDead { get { return _isDead; } private set {return;} }
    }
}
