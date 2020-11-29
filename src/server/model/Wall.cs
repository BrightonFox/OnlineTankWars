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

using System;
using TankWars.MathUtils;

namespace TankWars.Server.Model
{
    /// <inheritdoc cref="TankWars.JsonObjects.Wall" />
    public class Wall : TankWars.JsonObjects.Wall
    {
        private static int nextId = 0;

        public Wall(Vector2D p1, Vector2D p2) : base()
        {
            _id = nextId++;
            IsHorizontal = p1.Y==p2.Y;
            if (IsHorizontal)
                (PUp, PLow) = (p1.X > p2.X) ? (p1, p2) : (p2, p1);
            else
                (PUp, PLow) = (p1.Y > p2.Y) ? (p1, p2) : (p2, p1);
        }


        /// <summary>
        /// Boolean if the wall is vertical or horizontal.
        /// </summary>
        public bool IsHorizontal { get; private set; }


        /// <inheritdoc cref="TankWars.JsonObjects.Wall._id" />
        public int Id { get { return _id; } private set { return; } }

        /// <summary>
        /// The point that contains either the upper x/y coordinate fo the wall 
        ///  dependent on if the wall <see cref="isHorisonatal"/> 
        /// (then upper x or vise versa)
        /// </summary>
        public Vector2D PUp { get { return _p1; } private set { _p1 = value; } }

        /// <summary>
        /// The point that contains either the lower x/y coordinate fo the wall 
        ///  dependent on if the wall <see cref="isHorisonatal"/> 
        /// (then lower x or vise versa)
        /// </summary>
        public Vector2D PLow { get { return _p2; } private set { _p2 = value; } }
        
    }
}
