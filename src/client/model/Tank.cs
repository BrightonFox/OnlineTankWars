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

namespace TankWars.Client.Model
{
    public class Tank : TankWars.Tank
    {

        public int Id { get {return _id;} private set {return;}}

        public string PlayerName { get {return _playerName;} private set {return;}}

        public TankWars.Vector2D Location { get {return _location;} private set {return;}}
        
        public TankWars.Vector2D BarrelDirection { get {return _barrelDirection;} private set {return;}}

        public TankWars.Vector2D Direction { get {return _direction;} private set {return;}}

        public int Score { get { return _score; } private set {return;} }

        public int Health { get { return _health; } private set {return;} }

        public bool IsDead { get { return _isDead; } private set {return;} }

        public bool IsDisconnected { get { return _isDisconnected; } private set {return;} }

        public bool HasJoined { get { return _hasJoined; } private set {return;} }
    }
}
