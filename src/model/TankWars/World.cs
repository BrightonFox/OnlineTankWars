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

namespace TankWars
{
    public class World
  {
    // In reality, these should not be public,
    // but for the purposes of this lab, the "World" 
    // class is just a wrapper around these two fields.
    public Dictionary<int, Player> Players;
    public Dictionary<int, Powerup> Powerups;
    public int size
    { get; private set; }

    public World(int _size)
    {
      Players = new Dictionary<int, Player>();
      Powerups = new Dictionary<int, Powerup>();
      size = _size;
    }

  }
}
