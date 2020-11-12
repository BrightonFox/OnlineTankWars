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
 *   + ...
 */

using System;
using Newtonsoft.Json;
using TankWars.MathUtils;

namespace TankWars
{
    [JsonObject(MemberSerialization.OptIn)]
    public abstract class ITank
    {
        public ITank()
        {
            //TODO: figureout what goes here....
        }

        [JsonProperty(PropertyName = "tank")]
        protected int _id;

        [JsonProperty(PropertyName = "name")]
        protected string _playerName;

        [JsonProperty(PropertyName = "loc")]
        protected Vector2D _location;

        [JsonProperty(PropertyName = "bdir")]
        protected Vector2D _barrelDirection;

        [JsonProperty(PropertyName = "tdir")]
        protected Vector2D _direction;

        [JsonProperty(PropertyName = "score")]
        protected int _score;

        [JsonProperty(PropertyName = "hp")]
        protected int _health;

        [JsonProperty(PropertyName = "died")]
        protected bool _isDead;

        [JsonProperty(PropertyName = "dc")]
        protected bool _isDisconnected;

        [JsonProperty(PropertyName = "join")]
        protected bool _hasJoined;
    }
}
