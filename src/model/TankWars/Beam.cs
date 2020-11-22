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
 *   A representation of a beam for the TankWars Game.
 */

using Newtonsoft.Json;
using TankWars.MathUtils;


namespace TankWars.JsonObjects
{
    /// <summary>
    /// A beam that spans accross the 
    ///  entire map causinginstant death, and ignoring walls.
    /// The Server should only send it once, 
    ///  and clients will decide for how many frames they
    ///  want to draw it.
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public abstract class Beam
    {
        /// <summary>
        /// A beam that spans accross the 
        ///  entire map causinginstant death, and ignoring walls.
        /// The Server should only send it once, 
        ///  and clients will decide for how many frames they
        ///  want to draw it.
        /// </summary>
        public Beam()
        {
            // TODO: figure out what goes here...
        }

        /// <summary>
        /// An int representing the beam's unique ID.
        /// </summary>
        [JsonProperty(PropertyName = "beam")]
        protected int _id;

        /// <summary>
        /// A Vector2D representing the origin of the beam.
        /// </summary>
        [JsonProperty(PropertyName = "org")]
        protected Vector2D _origin;

        /// <summary>
        /// A Vector2D representing the direction of the beam.
        /// </summary>
        [JsonProperty(PropertyName = "dir")]
        protected Vector2D _direction;

        /// <summary>
        /// An int representing the ID of the tank that fired
        /// the beam. You can use this to draw the beams with
        /// a different color or image for each player.
        /// </summary>
        [JsonProperty(PropertyName = "owner")]
        protected int _ownerId;
    }
}
