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
 *   A representation of a world for the TankWars Game.
 */

using System;
using System.Collections.Generic;

using TankWars.JsonObjects;


namespace TankWars
{
    /// <summary>
    /// An object that holds references to all of the objects 
    ///  currently contained in the TankWars game.
    /// It holds them in dictionaries indexed by their unique id's
    ///  that the server assigns.
    /// </summary>
    public abstract class IWorld
    {
        /// <summary>
        /// Dictionary containing the <see cref="Tank"/>s in the world,
        ///  indexed by their unique id.
        ///  </summary>
        protected Dictionary<int, Tank> Tanks { get; private set; }

        /// <summary>
        /// Dictionary containing the <see cref="Powerup"/>s in the world,
        ///  indexed by their unique id.
        /// </summary>
        protected Dictionary<int, Powerup> Powerups { get; private set; }

        /// <summary>
        /// Dictionary containing the <see cref="Wall"/>s in the world,
        ///  indexed by their unique id.
        /// </summary>
        protected Dictionary<int, TankWars.JsonObjects.Wall> Walls { get; private set; }

        /// <summary>
        /// Dictionary containing the <see cref="Projectile"/>s in the world,
        ///  indexed by their unique id.
        /// </summary>
        protected Dictionary<int, Projectile> Projectiles { get; private set; }

        /// <summary>
        /// The length of each side of the world
        /// </summary>
        public int size { get; protected set; }

        /// <summary>
        /// An object that holds references to all of the objects 
        ///  currently contained in the TankWars game.
        /// It holds them in dictionaries indexed by their unique id's
        ///  that the server assigns.
        /// </summary>
        /// <param name="_size">the width of the playing field (and height since it should be square)</param>
        public IWorld(int _size)
        {
            Tanks = new Dictionary<int, Tank>();
            Powerups = new Dictionary<int, Powerup>();
            Walls = new Dictionary<int, TankWars.JsonObjects.Wall>();
            Projectiles = new Dictionary<int, Projectile>();

            size = _size;
        }

    }
}
