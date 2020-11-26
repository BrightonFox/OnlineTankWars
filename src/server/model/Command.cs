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
        /// <inheritdoc cref="TankWars.JsonObjects.Command._movement" />
        public string Movement { get { return _movement; } set { return; } }

        /// <inheritdoc cref="TankWars.JsonObjects.Command._fire" />
        public string Fire { get { return _fire; } set { return; } }

        /// <inheritdoc cref="TankWars.JsonObjects.Command._turretDirection" />
        public Vector2D Direction { get { return _turretDirection; } set { return; } }

        private int _ownerId = -1;

        /// <inheritdoc cref="TankWars.JsonObjects.Projectile._ownerId" />
        public int OwnerId
        {
            get { return _ownerId; }
            set {
                if (_ownerId == -1)
                    _ownerId = value;
            }
        }

        /// <summary>
        /// Overides the default Equals so that commands are equivalent if their 
        /// <see cref="Command.OwnerId"/>'s are equivalent.
        /// <para>
        /// Returns false if <paramref name="obj"/> is not of the <see cref="Command"/> type.
        /// </para>
        /// We use this to make a "queue function as a set as well, kind of,
        ///  for our world processing.
        /// I order to ensure we only process one command per client per frame.
        /// </summary>
        /// <param name="obj">Command to be compared to called Command</param>
        /// <returns>true of Id's match and false if types don't match or id's don't match.</returns>
        public override bool Equals(object obj)
        {
            return (obj.GetType() == typeof(Command)) ? (obj as Command).OwnerId == this.OwnerId : false;
        }


        /// <inheritdoc cref="Object.GetHashCode"/>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

    }
}
