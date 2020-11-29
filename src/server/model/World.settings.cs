using System.Collections.Generic;
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


namespace TankWars.Server.Model
{
    public partial class World
    {
        public static int MSPerFrame {get; private set;}

        public static int MaxPowerupDelay { get; private set; }
        public static int MaxPowerups {get; private set;}

        public static int StartingHealth {get; private set;}

        public static double TankSpeed { get; private set; }
        public static double ProjectileSpeed { get; private set; }

        public static int MaxCoordinate {get; private set;}

        public static int MaxSpawnAttempts {get; private set;}

        public static int WallSize { get; private set; }

        public static int TankSize { get; private set; }

        public static int ProjectileFireDelay { get; private set; }

        /// <summary>
        /// How many frames must a tank wait before respawning? 
        /// This is not in units of time. It is in units of frames.
        /// </summary>
        public static int RespawnDelay {get; private set;}


        /// <summary>
        /// Sets the vales that can be defined in the settings xml
        ///  to default values in case they were not provided.
        /// </summary>
        private void SetDefaultValues()
        {
            MSPerFrame = 17; //2000;
            MaxPowerups = 2;
            MaxSpawnAttempts = 64;
            MaxPowerupDelay = 1650;
            TankSize = 60;
            TankSpeed = 3d; //TankSize/2;
            WallSize = 50;
            StartingHealth = 3;
            ProjectileSpeed = 25d;
            Size = 1000;
            MaxCoordinate = Size/2;
            ProjectileFireDelay = 80;
            RespawnDelay = 300;
            Walls.Add(new Wall(new Vector2D(0, 0), new Vector2D(0, 0)));
            Walls.Add(new Wall(new Vector2D(-250, 200), new Vector2D(-250, -200)));
            Walls.Add(new Wall(new Vector2D(200, -250), new Vector2D(-200, -250)));
        }

    }

    internal static class DictExtension
    {
        public static void Add(this Dictionary<int, TankWars.JsonObjects.Wall> self, Wall wall)
        {
            self.Add(wall.Id, wall);
        }
        public static void Add(this Dictionary<int, TankWars.JsonObjects.Tank> self, Tank tank)
        {
            self.Add(tank.Id, tank);
        }
        public static void Add(this Dictionary<int, TankWars.JsonObjects.Projectile> self, Projectile proj)
        {
            self.Add(proj.Id, proj);
        }
        public static void Add(this Dictionary<int, TankWars.JsonObjects.Powerup> self, Powerup pow)
        {
            self.Add(pow.Id, pow);
        }
    } 
}
