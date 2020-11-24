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
 *   A representation of a player command for the TankWars Game.
 */

using System;
using Newtonsoft.Json;
using TankWars.MathUtils;


namespace TankWars.JsonObjects
{

    /// <summary>
    /// Messages that the clients send to the server to tell it
    ///  how the players want to move, fire 
    ///  and what direction they are facing when this happens.
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public abstract class Command
    {
        
        /// <summary>
        /// Messages that the clients send to the server to tell it
        ///  how the players want to move, fire 
        ///  and what direction they are facing when this happens.
        /// </summary>
        public Command()
        {
            //TODO: Figure out what to do here...
        }

        /// <summary>
        /// A string representing whether the player wants to move
        /// or not, and the desired direction. Possible values
        /// are: "none", "up", "left", "down", "right".
        /// </summary>
        [JsonProperty(PropertyName = "moving")]
        protected string _movement;
        
        /// <summary>
        /// A string representing whether the player wants to fire
        /// or not, and the desired type. Possible values are: "none",
        /// "main", (for a normal projectile) and "alt" (for a beam attack).
        /// </summary>
        [JsonProperty(PropertyName = "fire")]
        protected string _fire;
        
        /// <summary>
        /// a Vector2D representing where the player wants to aim
        /// their turret. This vector must be normalized. See the
        /// Vector2D section below.
        /// </summary>
        [JsonProperty(PropertyName = "tdir")]
        protected Vector2D _turretDirection;

        /// <summary>
        /// Return a string representation of a <see cref="Command"/>,
        ///  using json formatting.
        /// Appending a '\n' at the end for the networking protocol.
        /// </summary>
        /// <returns>json representation of the object</returns>
        public override string ToString()
        {
            return ToJson() + '\n';
        }

        /// <summary>
        /// Return a string representation of a <see cref="Command"/>,
        ///  using json formatting.
        /// </summary>
        /// <returns>json representation of the object</returns>
        public string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
