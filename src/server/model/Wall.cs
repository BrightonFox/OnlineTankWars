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
        
        public Wall(int id, Vector2D p1, Vector2D p2, int worldSize) : base()
        {
            
        }


        /// <summary>
        /// Boolean if the wall is vertical or horizontal.
        /// </summary>
        public bool isHorizontal { get; internal set; }


        /// <inheritdoc cref="TankWars.JsonObjects.Wall._id" />
        public int Id { get { return _id; } private set { return; } }

        /// <summary>
        /// The point that contains either the upper x/y coordinate fo the wall 
        ///  dependent on if the wall <see cref="isHorisonatal"/> 
        /// (then upper x or vise versa)
        /// </summary>
        public Vector2D PUp { get { return _p1; } private set { return; } }

        /// <summary>
        /// The point that contains either the lower x/y coordinate fo the wall 
        ///  dependent on if the wall <see cref="isHorisonatal"/> 
        /// (then lower x or vise versa)
        /// </summary>
        public Vector2D PLow { get { return _p2; } private set { return; } }
        
    }
}
