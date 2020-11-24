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
 *   + v1.0 - Submittal - 2020/11/21
 * 
 * About:
 *   The object representing what the world is for the client,
 *    also contains the logic to decode a json string into the appropriate 
 *    client view object.
 *   See the main TankWars World object for the generic version
 *    shared between the client and the server. 
 */

using System;
using System.Collections.Generic;
using System.Linq;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


namespace TankWars.Client.Model
{

    /// <summary>
    /// The representation of the world as the Client understands it.
    /// </summary>
    public class World : IWorld
    {
        private Dictionary<int, Beam> Beams;

        /// <summary>
        /// A representation of the player the client belong to.
        /// Contains the player name and id.
        /// </summary>
        public Player Player {get; private set;}


        /// <summary>
        /// The representation of the world as the Client understands it.
        /// </summary>
        /// <param name="_size">the width of the world</param>
        /// <param name="_player">a representation of the player</param>
        /// <returns></returns>
        public World(int _size, Player _player) : base(_size)
        {
            Player = _player;
            Beams = new Dictionary<int, Beam>();
        }


        /// <summary>
        /// returns a bool representing if the tank that belongs to the player using the client 
        ///  is contained in the world or not.
        /// </summary>
        /// <returns>bool of if the world contains the tank that belongs to the player</returns>
        public bool HasPlayerTank()
        {
            if (Player == null) return false;
            return Tanks.ContainsKey(Player.Id);
        }


        #region Item Getters

        /// <summary>
        /// Returns the <see cref="TankWars.Client.Model.Tank"/> associated with <paramref name="id"/>
        /// </summary>
        /// <param name="id">the of the Tank in question</param>
        /// <returns>the tank associated with <paramref name="id"/></returns>
        public Tank GetTank(int id)
        {
            return (Tank) Tanks[id];
        }

        /// <summary>
        /// Returns the <see cref="TankWars.Client.Model.Powerup"/> associated with <paramref name="id"/>
        /// </summary>
        /// <param name="id">the id of the Powerup in question</param>
        /// <returns>the Powerup associated with <paramref name="id"/></returns>
        public Powerup GetPowerup(int id)
        {
            return (Powerup) Powerups[id];
        }

        /// <summary>
        /// Returns the <see cref="TankWars.Client.Model.Wall"/> associated with <paramref name="id"/>
        /// </summary>
        /// <param name="id">the Id of the wall in question</param>
        /// <returns>the wall associated with <paramref name="id"/></returns>
        public Wall GetWall(int id)
        {
            return (Wall) Walls[id];
        }

        /// <summary>
        /// Returns the <see cref="TankWars.Client.Model.Projectile"/> associated with <paramref name="id"/>
        /// </summary>
        /// <param name="id">the id of the projectile in question</param>
        /// <returns>the Powerup associated with <paramref name="id"/></returns>
        public Projectile GetProjectile(int id)
        {
            return (Projectile) Projectiles[id];
        }

        /// <summary>
        /// Returns the <see cref="TankWars.Client.Model.Beam"/> associated with <paramref name="id"/>
        /// </summary>
        /// <param name="id">the id of the Beam in question</param>
        /// <returns>the Beam associated with <paramref name="id"/></returns>
        public Beam GetBeam(int id)
        {
            return (Beam) Beams[id];
        }
        #endregion


        #region ID getters
        /// <summary>
        /// Returns an list/<see cref="Enumerable"/>
        ///  that contains all of the <see cref="TankWars.Client.Model.Tank.Id"/>s
        ///  that the world contains.
        /// </summary>
        /// <returns>list of all of the <see cref="TankWars.Client.Model.Tank.Id"/>s <see cref="World"/> contains</returns>
        public IEnumerable<int> GetTankIds()
        {
            return Tanks.Keys;
        }

        /// <summary>
        /// Returns an list/<see cref="Enumerable"/>
        ///  that contains all of the <see cref="TankWars.Client.Model.Powerup.Id"/>s
        ///  that the world contains.
        /// </summary>
        /// <returns>list of all of the <see cref="TankWars.Client.Model.Powerup.Id"/>s <see cref="World"/> contains</returns>
        public IEnumerable<int> GetPowerupIds()
        {
            return Powerups.Keys;
        }

        /// <summary>
        /// Returns an list/<see cref="Enumerable"/>
        ///  that contains all of the <see cref="TankWars.Client.Model.Projectile.Id"/>s
        ///  that the world contains.
        /// </summary>
        /// <returns>list of all of the <see cref="TankWars.Client.Model.Projectile.Id"/>s <see cref="World"/> contains</returns>
        public IEnumerable<int> GetProjectileIds()
        {
            return Projectiles.Keys;
        }

        /// <summary>
        /// Returns an list/<see cref="Enumerable"/>
        ///  that contains all of the <see cref="TankWars.Client.Model.Wall.Id"/>s
        ///  that the world contains.
        /// </summary>
        /// <returns>list of all of the <see cref="TankWars.Client.Model.Wall.Id"/>s <see cref="World"/> contains</returns>
        public IEnumerable<int> GetWallIds()
        {
            return Walls.Keys;
        }

        /// <summary>
        /// Returns an list/<see cref="Enumerable"/>
        ///  that contains all of the <see cref="TankWars.Client.Model.Beam.Id"/>s
        ///  that the world contains.
        /// </summary>
        /// <returns>list of all of the <see cref="TankWars.Client.Model.Beam.Id"/>s <see cref="World"/> contains</returns>
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
        /// <param name="lifeSpan">the number of frames the beam should be drawn for</param>
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
