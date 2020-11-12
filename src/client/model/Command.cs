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
    public class Command : ICommand
    {

        public string Moving
        {
            get { return _moving; }
            set { _moving = value; }
        }

        public string Fire
        {
            get { return _fire; }
            set { _fire = value; }
        }

        public Vector2D TankDirection
        {
            get { return _tankDirection; }
            set { _tankDirection = value; }
        }
    }
}
