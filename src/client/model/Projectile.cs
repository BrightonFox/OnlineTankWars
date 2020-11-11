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
    public class Projectile : TankWars.Projectile
    {

        public int Id { get { return _id; } private set {return;} }

        public Vector2D Location { get { return _location; } private set {return;} }

        public Vector2D Direction { get { return _direction; } private set {return;} }

        public bool IsDead { get {return _isDead;} private set {return;} }

        public int OwnerId { get { return _ownerId; } private set {return;} }
    }
}
