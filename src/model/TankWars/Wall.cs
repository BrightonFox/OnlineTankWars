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
using Newtonsoft.Json;
using TankWars.MathUtils;


namespace TankWars
{
    [JsonObject(MemberSerialization.OptIn)]
    public class Wall
    {
        public Wall()
        {
            //TODO: figure out what goes here...
        }

        [JsonProperty(PropertyName = "wall")]
        protected int _id;

        [JsonProperty(PropertyName = "p1")]
        protected Vector2D _p1;

        [JsonProperty(PropertyName = "p2")]
        protected Vector2D _p2;
    }
}
