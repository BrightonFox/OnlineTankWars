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

using TankWars.MathUtils;

namespace TankWars.Client.Model
{
    public class Player
    {
        public Player(string name) : base()
        {
            Name = name;
        }
        
        public bool IdSet { get; private set;}

        public string Name { get; private set;}

        private int _id;

        public int Id
        {
            get
            {
                return _id;
            }
            set
            {
                if (!IdSet)
                {
                    _id = value;
                    IdSet = true;
                }
            }
        }
    }
}
