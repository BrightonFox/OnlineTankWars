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
 *  A representation of a wall for the TankWars Game.
 */

using System;
using Newtonsoft.Json;
using TankWars.MathUtils;


namespace TankWars.JsonObjects
{
    /// <summary>
    /// Representation of a wall in the world,
    /// it will either represent a vertical or horizontal wall.
    /// In which case the x/y coordinates not describign the length of the wall should be the same 
    /// and represent the center of where the wall should be.
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public abstract class Wall
    {
        public Wall()
        {
            //TODO: figure out what goes here...
        }

        /// <summary>
        /// An int representing the wall's unique ID.
        /// </summary>
        [JsonProperty(PropertyName = "wall")]
        protected int _id;

        /// <summary>
        /// A Vector2D representing one endpoint of the wall.
        /// </summary>
        [JsonProperty(PropertyName = "p1")]
        protected Vector2D _p1;

        /// <summary>
        /// A Vector2D representing the other endpoint of the wall.
        /// </summary>
        [JsonProperty(PropertyName = "p2")]
        protected Vector2D _p2;
    }
}
