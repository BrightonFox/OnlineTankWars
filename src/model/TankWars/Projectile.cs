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
 *   A representation of a projectile for the TankWars Game.
 */

using System;
using Newtonsoft.Json;
using TankWars.MathUtils;


namespace TankWars.JsonObjects
{
    /// <summary>
    /// An object that represents a projectile that has been fired from a tank,
    ///  in the TankWars game.
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public abstract class Projectile
    {
        public Projectile()
        {
            // TODO: figure out what goes here...
        }

        /// <summary>
        /// an int representing the projectile's unique ID
        /// </summary>
        [JsonProperty(PropertyName = "proj")]
        protected int _id;

        /// <summary>
        /// a Vector2D representing the projectile's location.
        /// </summary>
        [JsonProperty(PropertyName = "loc")]
        protected Vector2D _location;

        /// <summary>
        /// a Vector2D representing the projectile's orientation.
        /// </summary>
        [JsonProperty(PropertyName = "dir")]
        protected Vector2D _direction;

        /// <summary>
        /// A bool representing if the projectile died on this frame
        /// (hit something or left the bounds of the world). The
        /// server will send the dead projectiles only once.
        /// </summary>
        [JsonProperty(PropertyName = "died")]
        protected bool _isDead;

        /// <summary>
        /// An int representing the ID of the tank that created the
        /// projectile. You can use this to draw the projectiles
        /// with a different color or image for each player.
        /// </summary>
        [JsonProperty(PropertyName = "owner")]
        protected int _ownerId;


        /// <summary>
        /// Return a string representation of a <see cref="Projectile"/>,
        ///  using json formatting.
        /// Appending a '\n' at the end for the networking protocol.
        /// </summary>
        /// <returns>json representation of the object</returns>
        public override string ToString()
        {
            return ToJson() + '\n';
        }

        /// <summary>
        /// Return a string representation of a <see cref="Projectile"/>,
        ///  using json formatting.
        /// </summary>
        /// <returns>json representation of the object</returns>
        public string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
