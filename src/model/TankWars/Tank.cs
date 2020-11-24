using System.ComponentModel;
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
 *   + v1.0 - submittal - 2020/11/21
 *   
 * About:
 *  A representation of a tank for the TankWars Game.
 */

using System;
using Newtonsoft.Json;
using TankWars.MathUtils;

namespace TankWars.JsonObjects
{
    /// <summary>
    /// A representation of a tank for the TankWars Game.
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public abstract class Tank
    {
        public Tank()
        {
            //TODO: figureout what goes here....
        }

        /// <summary>
        /// An int representing the tank's unique ID.
        /// </summary>
        [JsonProperty(PropertyName = "tank")]
        protected int _id;

        /// <summary>
        /// A string representing the player's name.
        /// </summary>
        [JsonProperty(PropertyName = "name")]
        protected string _playerName;

        /// <summary>
        /// A Vector2D representing the tank's location. 
        ///</summary>
        [JsonProperty(PropertyName = "loc")]
        protected Vector2D _location;

        /// <summary>
        /// A Vector2D representing the tank's orientation. This
        /// will always be an axis-aligned vector (purely horizontal or vertical).
        /// </summary>
        [JsonProperty(PropertyName = "bdir")]
        protected Vector2D _bodyDirection;

        /// <summary>
        /// A Vector2D representing the direction of the tank's
        /// turret (where it's aiming). 
        /// </summary>
        [JsonProperty(PropertyName = "tdir")]
        protected Vector2D _turretDirection;

        /// <summary>
        /// An int representing the player's score.
        /// </summary>
        [JsonProperty(PropertyName = "score")]
        protected int _score;

        /// <summary>
        /// An int representing the hit points of the tank. 
        /// This value ranges from 0 - 3. 
        /// If it is 0, then this tank is temporarily destroyed, and waiting to respawn.
        /// </summary>
        [JsonProperty(PropertyName = "hp")]
        protected int _health;

        /// <summary>
        /// A bool indicating if the tank died on that frame. This will
        /// only be true on the exact frame in which the tank died.
        /// </summary>
        [JsonProperty(PropertyName = "died")]
        protected bool _isDead;

        /// <summary>
        /// A bool indicating if the player controlling that tank
        /// disconnected on that frame. The server will send the tank with
        /// this flag set to true only once, then it will discontinue
        /// sending that tank for the rest of the game. You can use this to
        /// remove disconnected players from your model.
        /// </summary>
        [JsonProperty(PropertyName = "dc")]
        protected bool _isDisconnected;

        /// <summary>
        /// A bool indicating if the player joined on this frame. This will
        /// only be true for one frame. This field may not be needed, but
        /// may be useful for certain additional View related features.
        /// </summary>
        [JsonProperty(PropertyName = "join")]
        protected bool _hasJoined;


        /// <summary>
        /// Return a string representation of a <see cref="Tank"/>,
        ///  using json formatting.
        /// Appending a '\n' at the end for the networking protocol.
        /// </summary>
        /// <returns>json representation of the object</returns>
        public override string ToString()
        {
            return ToJson() + '\n';
        }

        /// <summary>
        /// Return a string representation of a <see cref="Tank"/>,
        ///  using json formatting.
        /// </summary>
        /// <returns>json representation of the object</returns>
        public string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
