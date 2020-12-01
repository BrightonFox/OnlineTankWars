using System.Globalization;
using System.Security.Cryptography;
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
using System.Xml;

using TankWars.MathUtils;


namespace TankWars.Server.Model
{
    public partial class World
    {
        public static int MSPerFrame { get; private set; }

        public static int MaxPowerupDelay { get; private set; }

        public static int MaxPowerups { get; private set; }

        public static int StartingHealth { get; private set; }

        public static double TankSpeed { get; private set; }

        public static double ProjectileSpeed { get; private set; }

        public static int MaxCoordinate { get; private set; }

        public static int MaxSpawnAttempts { get; private set; }

        public static int WallSize { get; private set; }

        public static int TankSize { get; private set; }

        public static int ProjectileFireDelay { get; private set; }

        /// <summary>
        /// How many frames must a tank wait before respawning? 
        /// This is not in units of time. It is in units of frames.
        /// </summary>
        public static int RespawnDelay { get; private set; }


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
            MaxCoordinate = Size / 2;
            ProjectileFireDelay = 80;
            RespawnDelay = 300;
        }


        /// <summary>
        /// !! Should only be called from the Constructor, 
        ///   after calling <see cref="World.SetDefaultValues"/> !!
        /// <para>
        /// Reads in a settings xml file.
        /// </para>
        /// </summary>
        private void ReadSettings(string fileDir)
        {
            using (var reader = XmlReader.Create(fileDir))
            {
                try
                {
                    reader.ReadToFollowing("GameSettings");
                    while (reader.Read())
                        if (reader.IsStartElement())
                            switch (reader.Name)
                            {
                                case "UniverseSize":
                                case "WorldSize":
                                case "MapSize":
                                case "Size":
                                    reader.Read();
                                    Size = Int32.Parse(reader.Value);
                                    MaxCoordinate = Size/2;
                                    break;

                                case "MSPerFrameSize":
                                case "MSPerFrame":
                                    reader.Read();
                                    MSPerFrame = Int32.Parse(reader.Value);
                                    break;
                                case "FramePerShot":
                                case "ProjectileFireDelay":
                                case "ProjFireDelay":
                                    reader.Read();
                                    ProjectileFireDelay = Int32.Parse(reader.Value);
                                    break;
                                case "RespawnDelay":
                                case "RespawnTimer":
                                    reader.Read();
                                    RespawnDelay = Int32.Parse(reader.Value);
                                    break;
                                case "TankSpeed":
                                case "TankStrength":
                                case "EngineStrength":
                                case "EngineSpeed":
                                    reader.Read();
                                    TankSpeed = double.Parse(reader.Value, dblParseStyle);
                                    break;
                                case "StartingHealth":
                                case "MaxHealth":
                                    reader.Read();
                                    StartingHealth = Int32.Parse(reader.Value);
                                    break;
                                case "MaxPowerupDelay":
                                case "MaxPowerupSpawnDelay":
                                    reader.Read();
                                    MaxPowerupDelay = Int32.Parse(reader.Value);
                                    break;
                                case "MaxPowerups":
                                    reader.Read();
                                    MaxPowerups = Int32.Parse(reader.Value);
                                    break;
                                case "ProjectileSpeed":
                                case "ProjSpeed":
                                    reader.Read();
                                    ProjectileSpeed = double.Parse(reader.Value, dblParseStyle);
                                    break;
                                case "TankHitboxSize":
                                case "TankSize":
                                    reader.Read();
                                    TankSize = Int32.Parse(reader.Value);
                                    break;
                                case "Wall":
                                    ReadWall(reader);
                                    break;
                                default:
                                    break;
                            }
                }
                catch (Exception ex)
                {
                    throw new Exception($"Error: While Reading XML File !!  [Tag: <{reader.Name}> | Location: (l:{((IXmlLineInfo)reader).LineNumber}, c:{((IXmlLineInfo)reader).LinePosition})] \n" +
                                        $"Error: {ex.Message} !!  [Server.World.Settings.ReadSettings]");
                }
            }
        }

        private static readonly NumberStyles dblParseStyle = (NumberStyles)((int)NumberStyles.Number+(int)NumberStyles.AllowExponent);

        /// <summary>
        /// Handle Reading in Walls
        /// </summary>
        private void ReadWall(XmlReader reader)
        {
            Vector2D p1 = null;
            Vector2D p2 = null;
            while (reader.Read())
                if (reader.IsStartElement())
                    switch (reader.Name)
                    {
                        case "p1":
                        case "P1":
                        case "Point1":
                        case "point1":
                            p1 = ReadPoint(reader);
                            break;
                        case "p2":
                        case "P2":
                        case "Point2":
                        case "point2":
                            p2 = ReadPoint(reader);
                            break;
                        case "p":
                        case "P":
                        case "Point":
                        case "point":
                            if (p1 == null)
                                p1 = ReadPoint(reader);
                            else
                                p2 = ReadPoint(reader);
                            break;
                        default:
                            break;
                    }
                else if (reader.Name == "Wall")
                    if (p2 != null && p1 != null)
                    {
                        this.Walls.Add(new Wall(p1, p2));
                        return;
                    }
                    else
                        throw new Exception("Error: Bad settings XML !!  [Server.World.Settings.ReadWall]\n" +
                                            $"    <Wall> does not contain both a <p1> and a <p2> !!  [l:{((IXmlLineInfo)reader).LineNumber}, c:{((IXmlLineInfo)reader).LinePosition}]");

        }

        /// <summary>
        /// Handle Reading in points/vectors from the xml Setttings location.
        /// </summary>
        private Vector2D ReadPoint(XmlReader reader)
        {
            double x = Size;
            double y = Size;
            while (reader.Read())
                if (reader.IsStartElement())
                    switch (reader.Name)
                    {
                        case "x":
                        case "X":
                            reader.Read();
                            x = double.Parse(reader.Value, dblParseStyle);
                            break;
                        case "y":
                        case "Y":
                            reader.Read();
                            y = double.Parse(reader.Value, dblParseStyle);
                            break;
                        default:
                            break;
                    }
                else if (char.ToLower(reader.Name[0]) == 'p')
                    if (x > MaxCoordinate || x < -MaxCoordinate)
                        throw new Exception("Error: Bad settings XML !! [Server.World.Settings.ReadWall]\n" +
                                            $"    <x> is out of bounds for the world size or was not specified !!  [l:{((IXmlLineInfo)reader).LineNumber}, c:{((IXmlLineInfo)reader).LinePosition}]");
                    else if (y > MaxCoordinate || y < -MaxCoordinate)
                        throw new Exception("Error: Bad settings XML !! [Server.World.Settings.ReadWall]\n" +
                                            $"    <y> is out of bounds for the world size or was not specified !!  [l:{((IXmlLineInfo)reader).LineNumber}, c:{((IXmlLineInfo)reader).LinePosition}]");
                    else
                        return new Vector2D(x, y);

            return null;
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
