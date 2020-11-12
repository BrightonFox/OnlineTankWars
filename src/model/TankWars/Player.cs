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
    public abstract class IPlayer
    {
        private bool idSet = false;
        public IPlayer(string name)
        {
            Name = name;
        }

        public string Name { get; private set; }
        public int Id
        {
            get
            {
                return Id;
            }
            set
            {
                if (!idSet)
                {
                    Id = value;
                    idSet = true;
                }
            }
        }
    }
}
