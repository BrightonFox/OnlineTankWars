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

using System.Text;
using TankWars.MathUtils;


namespace TankWars.Client.Model
{
    public class Command : TankWars.JsonObjects.Command
    {

        public Command(string movement, string fire, Vector2D turretDir) : base()
        {
            _movement = movement;
            _fire = fire;
            _turretDirection = turretDir;
        }

    }
}
