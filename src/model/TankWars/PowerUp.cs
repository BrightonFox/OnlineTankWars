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
    public abstract class IPowerup
    {
        public IPowerup()
        {
            // TODO: figure out what goes here...
        }

        [JsonProperty(PropertyName = "power")]
        protected int _id;

        [JsonProperty(PropertyName = "loc")]
        protected Vector2D _location;

        [JsonProperty(PropertyName = "died")]
        protected bool _isDead;
    }
}
