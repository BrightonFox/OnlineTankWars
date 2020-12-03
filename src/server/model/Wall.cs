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
 *   + v1.0 - submittal - 2020/12/2
 *   
 * About:
 *   An object representing a wall present in the game
 *   world. Also contains the logic used to convert
 *   the wall into JSON for the clients.
 */

using System;
using TankWars.MathUtils;

namespace TankWars.Server.Model
{
    /// <inheritdoc cref="TankWars.JsonObjects.Wall" />
    public class Wall : TankWars.JsonObjects.Wall
    {
        private static int nextId = 0;

        /// <summary>
        /// Like <see cref="TankWars.JsonObjects.Wall"/>, 
        ///  but extended with logic for the server.
        /// Autogenerates the <see cref="Wall.Id"/>, 
        ///  based upon how many walls have been created before.
        /// </summary>
        /// <param name="p1">One point that represents a wall</param>
        /// <param name="p2">Another point that represents a wall</param>
        public Wall(Vector2D p1, Vector2D p2) : base()
        {
            _id = nextId++;
            _p1 = p1;
            _p2 = p2;

            // verifies that the provided walls are axis aligned an a multiple of the wallSize
            if (p1.Y == p2.Y)
            {
                if (Math.Abs(p1.X - p2.X) % World.WallSize != 0 || Math.Abs(p1.Y - p2.Y) % World.WallSize != 0)
                    throw new Exception($"Error: the provided points are not defining a wall with a length divisible by {World.WallSize} !!  [Server.Model.Wall.Constructor]");
            }
            else if (p1.X != p2.X)
                throw new ArgumentException($"Error: the provided points do not create an axis-aligned wall !!  [Server.Model.Wall.Constructor]");

            // assign the upper-right and lower-left coordinates of the wall for collision detection
            var pMod = new Vector2D(World.WallSize/2, World.WallSize/2);
            PUp = new Vector2D(Math.Max(p1.X, p2.X), Math.Max(p1.Y, p2.Y)) + pMod;
            PLow = new Vector2D(Math.Min(p1.X, p2.X), Math.Min(p1.Y, p2.Y)) + (pMod * -1);
        }

        /// <inheritdoc cref="TankWars.JsonObjects.Wall._id" />
        public int Id { get { return _id; } private set { return; } }

        /// <summary>
        /// A point/<see cref="Vector2D"/> representing the Upper bound corner
        ///  (aka: Lower-Right corner) 
        ///  of the <see cref="Wall"/>.
        /// </summary>
        public Vector2D PUp { get; private set; }

        /// <summary>
        /// A point/<see cref="Vector2D"/> representing the Lower bound corner
        ///  (aka: Upper-Left corner) 
        ///  of the <see cref="Wall"/>. 
        /// </summary>
        public Vector2D PLow { get; private set; }
        
    }
}
