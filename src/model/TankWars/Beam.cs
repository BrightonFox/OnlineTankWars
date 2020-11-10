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

namespace TankWars
{
    [JsonObject(MemberSerialization.OptIn)]
    public class Beam
    {
        public Beam()
        {
            // TODO: figure out what goes here...
        }

        [JsonProperty(PropertyName = "beam")]
        protected int _id;

        [JsonProperty(PropertyName = "org")]
        protected Vector2D _origin;

        [JsonProperty(PropertyName = "dir")]
        protected Vector2D _direction;

        [JsonProperty(PropertyName = "owner")]
        protected int _ownerId;
    }
}
