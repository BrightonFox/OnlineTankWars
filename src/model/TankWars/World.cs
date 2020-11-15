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
    public abstract class ProtoWorld
    {
        // In reality, these should not be public,
        // but for the purposes of this lab, the "World" 
        // class is just a wrapper around these two fields.
        // public Dictionary<int, IPlayer> Players;
        public Dictionary<int, Tank> Tanks;
        public Dictionary<int, Powerup> Powerups;
        public Dictionary<int, TankWars.JsonObjects.Wall> Walls;
        public Dictionary<int, Projectile> Projectiles;

        public int size
        { get; private set; }

        public ProtoWorld(int _size)
        {
            Tanks = new Dictionary<int, Tank>();
            Powerups = new Dictionary<int, Powerup>();
            Walls = new Dictionary<int, TankWars.JsonObjects.Wall>();
            Projectiles = new Dictionary<int, Projectile>();

            size = _size;
        }

    }
}
