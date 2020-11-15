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


namespace TankWars.JsonObjects
{
    [JsonObject(MemberSerialization.OptIn)]
    public abstract class Command
    {
        public Command()
        {
            //TODO: Figure out what to do here...
        }

        [JsonProperty(PropertyName = "moving")]
        protected string _movement;

        [JsonProperty(PropertyName = "fire")]
        protected string _fire;

        [JsonProperty(PropertyName = "tdir")]
        protected Vector2D _turretDirection;
    }
}
