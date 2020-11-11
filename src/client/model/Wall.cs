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
using TankWars.MathUtils;

namespace TankWars.Client.Model
{
    public class Wall : TankWars.Wall
    {
        public int Id { get { return _id; } private set {return;} }

        public Vector2D P1 { get { return _p1; } private set {return;} }

        public Vector2D P2 { get { return _p2; } private set {return;} }
    }
}
