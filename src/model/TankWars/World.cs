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

namespace TankWars
{
    public abstract class IWorld
  {
    // In reality, these should not be public,
    // but for the purposes of this lab, the "World" 
    // class is just a wrapper around these two fields.
    // public Dictionary<int, IPlayer> Players;
    public Dictionary<int, ITank> Tanks;
    public Dictionary<int, IPowerup> Powerups;
    public Dictionary<int, IWall> Walls;
    public Dictionary<int, IProjectile> Projectiles;
    
    public int size
    { get; private set; }

    public IWorld(int _size)
    {
      Tanks = new Dictionary<int, ITank>();
      Powerups = new Dictionary<int, IPowerup>();
      Walls = new Dictionary<int, IWall>();
      Projectiles = new Dictionary<int, IProjectile>();

      size = _size;
    }

  }
}
