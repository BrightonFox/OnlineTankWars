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
 *   + v1.0 - submittal - 2020/12/2
 *   
 * About:
 *   An object representing a Projectile that is tied to a
 *   tank/client. Also contains the logic used to convert the
 *   projectile into JSON for the clients.
 */

using System;
using TankWars.MathUtils;

namespace TankWars.Server.Model
{
    /// <inheritdoc cref="TankWars.JsonObjects.Projectile" />
    public class Projectile : TankWars.JsonObjects.Projectile
    {
        private static int nextId = 0;
        
        /// <summary>
        /// Like <see cref="TankWars.JsonObjects.Projectile"/>, 
        ///  but extended with logic for the server.
        /// Autogenerates the <see cref="Projectile.Id"/>, 
        ///  based upon how many walls have been created before.
        /// </summary>
        /// <param name="ownerId">The ID of the Tank that fired the projectile</param>
        /// <param name="loc">The starting location for the projectile.</param>
        /// <param name="dir">The direction the projectile should go in.</param>
        public Projectile(int ownerId, Vector2D loc, Vector2D dir) : base()
        {
            _id = nextId++;
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
