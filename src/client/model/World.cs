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
using System.Linq;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


namespace TankWars.Client.Model
{
    public class World : IWorld
    {
        private Dictionary<int, Beam> Beams;
        // new public Dictionary<int, Tank> Tanks;

        public Player Player {get; private set;}
        
        public World(int _size, Player _player) : base(_size)
        {
            Player = _player;
            Beams = new Dictionary<int, Beam>();
        }


        public bool HasPlayerTank()
        {
            if (Player == null) return false;
            return Tanks.ContainsKey(Player.Id);
        }

        public Tank GetPlayerTank()
        {
            return (Tank)Tanks[Player.Id];
        }

        // public T Get<T>(int id) where T : new()
        // {
        //     if (typeof(T) == typeof(Tank))
        //         return Tanks[id] as T;
        //     else if (typeof(T) == typeof(Projectile))
        //     else if (typeof(T) == typeof(Powerup))
        //     else if (typeof(T) == typeof(Wall))
        //     else if (typeof(T) == typeof(Beam))
        // }

        #region Item Getters
        public Tank GetTank(int id)
        {
            return (Tank) Tanks[id];
        }

        public Powerup GetPowerup(int id)
        {
            return (Powerup) Powerups[id];
        }

        public Wall GetWall(int id)
        {
            return (Wall) Walls[id];
        }

        public Projectile GetProjectile(int id)
        {
            return (Projectile) Projectiles[id];
        }

        public Beam GetBeam(int id)
        {
            return (Beam) Beams[id];
        }
        #endregion


        #region ID getters
        public IEnumerable<int> GetTankIds()
        {
            return Tanks.Keys;
        }

        public IEnumerable<int> GetPowerupIds()
        {
            return Powerups.Keys;
        }

        public IEnumerable<int> GetProjectileIds()
        {
            return Projectiles.Keys;
        }

        public IEnumerable<int> GetWallIds()
        {
            return Walls.Keys;
        }

        public IEnumerable<int> GetBeamIds()
        {
            return Beams.Keys;
        }
        #endregion


        /// <summary>
        /// Parses passed json string into the appropriate type,
        ///   and deserializes recognized objects to update client model world.
        /// </summary>
        /// <param name="json">String containing encoded Json Object</param>
        public void ParseJsonString(string json)
        {
            JObject jObj = JObject.Parse(json);
            IList<string> keys = jObj.Properties().Select(p => p.Name).ToList();
            foreach (string key in keys)
            {
                switch (key)
                {
                    case "tank":
                        var tank = JsonConvert.DeserializeObject<Tank>(json);
                            if (Tanks.ContainsKey(tank.Id))
                                if (tank.IsDisconnected)
                                    Tanks.Remove(tank.Id);
                                else
                                    Tanks[tank.Id] = tank;
                            else if (!tank.IsDisconnected)
                                Tanks.Add(tank.Id, tank);
                        return;
                    case "proj":
                        var proj = JsonConvert.DeserializeObject<Projectile>(json);
                            if (Projectiles.ContainsKey(proj.Id))
                                if (proj.IsDead)
                                    Projectiles.Remove(proj.Id);
                                else
                                    Projectiles[proj.Id] = proj;
                            else if (!proj.IsDead)
                                Projectiles.Add(proj.Id, proj);
                            return;
                    case "beam":
                        var beam = JsonConvert.DeserializeObject<Beam>(json);
                            Beams.Add(beam.Id, beam);
                        return;
                    case "power":
                        var powerup = JsonConvert.DeserializeObject<Powerup>(json);
                            if (Powerups.ContainsKey(powerup.Id))
                                if (powerup.IsDead)
                                    Powerups.Remove(powerup.Id);
                                else
                                    Powerups[powerup.Id] = powerup;
                            else if (!powerup.IsDead)
                                Powerups.Add(powerup.Id, powerup);
                            return;
                    case "wall":
                        var wall = JsonConvert.DeserializeObject<Wall>(json);
                        if (!Walls.ContainsKey(wall.Id))
                            Walls.Add(wall.Id, wall);
                        return;
                    default:
                        continue;
                }
            }
            throw new JsonException("ERROR: JSON not of Recognized type !!  [Controller.ParseJsonString]");
        }

        
        /// <summary>
        /// Check to see if the lifespan of a beam has expired.
        /// </summary>
        /// <param name="lifeSpan">the number of frames a beam should be drawn for</param>
        public void ManageBeamLifeTimes(int lifeSpan)
        {
            lock (this)
            {
                int[] beamIds = new int[Beams.Count];
                Beams.Keys.CopyTo(beamIds, 0);
                foreach (int beamId in beamIds)
                    if (Beams[beamId].LifeSpan > lifeSpan)
                        Beams.Remove(beamId);
            }
        }
    }
}
