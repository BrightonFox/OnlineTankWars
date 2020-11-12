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

namespace TankWars.Client.Model
{
    public class World : IWorld
    {
        public Dictionary<int, Beam> Beams;
        
        public World(int _size) : base(_size)
        {
        }

    }
}
