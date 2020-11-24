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
    /// <inheritdoc cref="TankWars.JsonObjects.Wall" />
    public class Wall : TankWars.JsonObjects.Wall
    {
        /// <inheritdoc cref="TankWars.JsonObjects.Wall._id" />
        public int Id { get { return _id; } internal set { _id = value; } }

        /// <inheritdoc cref="TankWars.JsonObjects.Wall._p1" />
        public Vector2D P1 { get { return _p1; } internal set { _p1 = value; } }

        /// <inheritdoc cref="TankWars.JsonObjects.Wall._p2" />
        public Vector2D P2 { get { return _p2; } internal set { _p2 = value; } }
        
    }
}
