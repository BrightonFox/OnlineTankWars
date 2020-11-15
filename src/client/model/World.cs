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
    public class World : ProtoWorld
    {
        public Dictionary<int, Beam> Beams;
        // new public Dictionary<int, Tank> Tanks;
        
        public World(int _size) : base(_size)
        {
        }

        public Tank GetTank(int id)
        {
            return (Tank) Tanks[id];
        }

    }
}
