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
 *   + ...
 */

using System;
using System.Collections.Generic;

using TankWars.JsonObjects;


namespace TankWars
{
    public abstract class IWorld
    {
        // In reality, these should not be public,
        // but for the purposes of this lab, the "World" 
        // class is just a wrapper around these two fields.
        // public Dictionary<int, IPlayer> Players;
        protected Dictionary<int, Tank> Tanks {get; private set;}
        protected Dictionary<int, Powerup> Powerups {get; private set;}
        protected Dictionary<int, TankWars.JsonObjects.Wall> Walls {get; private set;}
        protected Dictionary<int, Projectile> Projectiles { get; private set; }

        public int size { get; private set; }

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
