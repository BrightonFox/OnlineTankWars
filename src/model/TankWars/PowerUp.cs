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
 *   A representation of a powerup for the TankWars Game.
 */

using System;
using Newtonsoft.Json;
using TankWars.MathUtils;


namespace TankWars.JsonObjects
{

    /// <summary>
    /// Objects that appear on the map and when collected by players
    ///  in their tanks allow them to fire teh superchared Beam attack.
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public abstract class Powerup
    {
        /// <summary>
        /// Objects that appear on the map and when collected by players
        ///  in their tanks allow them to fire teh superchared Beam attack.
        /// </summary>
        public Powerup()
        {
            // TODO: figure out what goes here...
        }

        /// <summary>
        /// An int representing the powerup's unique ID.
        /// </summary>
        [JsonProperty(PropertyName = "power")]
        protected int _id;
        
        /// <summary>
        /// A Vector2D representing the location of the powerup.
        /// </summary>
        [JsonProperty(PropertyName = "loc")]
        protected Vector2D _location;
        
        /// <summary>
        /// A bool indicating if the powerup "died" (was collected by a player)
        /// on this frame. The server will send the dead powerups only once.
        /// </summary>
        [JsonProperty(PropertyName = "died")]
        protected bool _isDead;
    }
}
