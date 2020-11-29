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
    /// <inheritdoc cref="TankWars.JsonObjects.Beam" />
    /// <summary>
    /// The representation of a beam as the Client understands it.
    /// </summary>
    public class Beam : TankWars.JsonObjects.Beam
    {

        private static int nextId = 0;

        public Beam(int ownerId, Vector2D origin, Vector2D dir) : base()
        {
            _id = nextId++;
            _ownerId = ownerId;
            _origin = origin;
            _direction = dir;
        }

        /// <inheritdoc cref="TankWars.JsonObjects.Beam._id" />
        public int Id { get { return _id; } private set { return; } }
        
        /// <inheritdoc cref="TankWars.JsonObjects.Beam._origin" />
        public Vector2D Origin { get { return _origin; } private set { return; } }
        
        /// <inheritdoc cref="TankWars.JsonObjects.Beam._direction" />
        public Vector2D Direction { get { return _direction; } private set { return; } }

        /// <inheritdoc cref="TankWars.JsonObjects.Beam._ownerId" />
        public int OwnerId { get { return _ownerId; } private set { return; } }

    }
}
