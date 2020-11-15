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

        public int Id
        {
            get
            {
                return Id;
            }
            set
            {
                if (!IdSet)
                {
                    Id = value;
                    IdSet = true;
                }
            }
        }

        public Vector2D TurretDirection;
    }
}
