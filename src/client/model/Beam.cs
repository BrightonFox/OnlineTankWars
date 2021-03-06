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
 *   An object representing the beams to be fired from tanks. Contains
 *    the json logic needed for serializing and deserializing the
 *    object. Also contains logic for managing lifespan of the object
 *    for drawing.
 */

using System;
using TankWars.MathUtils;

namespace TankWars.Client.Model
{
    /// <inheritdoc cref="TankWars.JsonObjects.Beam" />
    /// <summary>
    /// The representation of a beam as the Client understands it.
    /// </summary>
    public class Beam : TankWars.JsonObjects.Beam
    {
    
        private int _lifeSpan = 0;
        
        /// <summary>
        /// Int representing how long a beam has been alive.
        /// <para>Every time it is called it's value will be incremented by 1</para>
        /// </summary>
        public int LifeSpan {
            get { return _lifeSpan++;}
            private set {_lifeSpan = value;}
        }

        /// <inheritdoc cref="TankWars.JsonObjects.Beam._id" />
        public int Id { get {return _id;} private set {return;} }
        
        /// <inheritdoc cref="TankWars.JsonObjects.Beam._origin" />
        public Vector2D Origin { get {return _origin;} private set {return;} }
        
        /// <inheritdoc cref="TankWars.JsonObjects.Beam._direction" />
        public Vector2D Direction { get {return _direction;} private set {return;} }

        /// <inheritdoc cref="TankWars.JsonObjects.Beam._ownerId" />
        public int OwnerId { get { return _ownerId; } private set { return; } }

    }
}
