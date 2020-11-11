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
    public class Beam : TankWars.Beam
    {

        public int Id { get {return _id;} private set {return;} }
        
        public Vector2D Origin { get {return _origin;} private set {return;} }
        
        public Vector2D Direction { get {return _direction;} private set {return;} }

        public int OwnerId { get { return _ownerId; } private set { return; } }
    }
}
