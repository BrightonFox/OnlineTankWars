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

namespace TankWars.Server.Model
{
    public partial class World
    {
        public int MSPerFrame {get; private set;}

        public int MaxPowerupDelay { get; private set; }
        public int MaxPowerups {get; private set;}

        public int StartingHealth {get; private set;}

        public double TankSpeed { get; private set; }
        public double ProjectileSpeed { get; private set; }

        public int MaxCoordinate {get; private set;}

        public int WallSize { get; private set; }

        public int TankSize { get; private set; }

        public int ProjectileFireDelay { get; private set; }

        /// <summary>
        /// How many frames must a tank wait before respawning? 
        /// This is not in units of time. It is in units of frames.
        /// </summary>
        public int RespawnDelay {get; private set;}


        /// <summary>
        /// Sets the vales that can be defined in the settings xml
        ///  to default values in case they were not provided.
        /// </summary>
        private void SetDefaultValues()
        {
            MSPerFrame = 17;
            MaxPowerups = 2;
            MaxPowerupDelay = 1650;
            TankSpeed = 3d;
            TankSize = 60;
            WallSize = 50;
            StartingHealth = 3;
            ProjectileSpeed = 25d;
            Size = 2000;
            ProjectileFireDelay = 80;
            RespawnDelay = 300;
        }

    }
}
