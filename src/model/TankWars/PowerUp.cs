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
    public class PowerUp
    {
        public PowerUp()
        {
            // TODO: figure out what goes here...
        }

        [JsonProperty(PropertyName = "power")]
        private int _id;

        [JsonProperty(PropertyName = "loc")]
        private Vector2D _location;

        [JsonProperty(PropertyName = "died")]
        private bool _isDead;
    }
}
