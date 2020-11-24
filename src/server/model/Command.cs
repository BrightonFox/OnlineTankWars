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
 *   + <>
 * 
 * About:
 *   <>
 */

using System.Text;
using TankWars.MathUtils;


namespace TankWars.Server.Model
{
    /// <inheritdoc cref="TankWars.JsonObjects.Command" />
    public class Command : TankWars.JsonObjects.Command
    {
        /// <inheritdoc cref="TankWars.JsonObjects.Command" />
        /// <param name="movement">What direction to move on this frame ("none", "up", "down", "left", "right")</param>
        /// <param name="fire">How the player wants to fire on this turn ("none", "main" for normal projectile, or "alt" for beam)</param>
        /// <param name="turretDir">What direction the player turret is facing when the player fired or just a the end of the frame.</param>
        public Command(string movement, string fire, Vector2D turretDir) : base()
        {
            _movement = movement;
            _fire = fire;
            _turretDirection = turretDir;
        }

    }
}
