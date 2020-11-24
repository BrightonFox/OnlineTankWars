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
    /// <inheritdoc cref="TankWars.JsonObjects.Powerup" />
    public class Powerup : TankWars.JsonObjects.Powerup
    {
        /// <inheritdoc cref="TankWars.JsonObjects.Powerup._id" />
        public int Id { get { return _id; } internal set { _id = value; } }

        /// <inheritdoc cref="TankWars.JsonObjects.Powerup._location" />
        public Vector2D Location { get { return _location; } internal set { _location = value; } }

        /// <inheritdoc cref="TankWars.JsonObjects.Powerup._isDead" />
        public bool IsDead { get { return _isDead; } internal set { _isDead = value; } }
    }
}
