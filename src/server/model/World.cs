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
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Text;

using TankWars.MathUtils;


namespace TankWars.Server.Model
{
    public partial class World : IWorld
    {
        private Queue<Command> FrameCommands = new Queue<Command>();
        private Random rand = new Random();



        public World(string fileDir) : base(2000)
        {
            SetDefaultValues();
        }


        private int _framesTillNextPowerupSpawn;
        private int FramesTillNextPowerupSpawn
        {
            get
            {
                if (--_framesTillNextPowerupSpawn <= 0)
                {
                    _framesTillNextPowerupSpawn = rand.Next(MaxPowerupDelay);
                    return 0;
                }
                return _framesTillNextPowerupSpawn;
            }
            set { return; }
        }



        /// <summary>
        /// Get a string of json representations, seperated by '\n',
        ///  that need to be sent to the clients as a frame.
        /// </summary>
        /// <returns></returns>
        public string NextFrame()
        {
            StringBuilder frame = new StringBuilder();

            // - Handle per tick/frame game mechanics ----
            HandleProjectiles();
            HandlePowerupSpawn();
            RespawnTanks();

            // - Process the Commands from the clients ----
            while (FrameCommands.Count > 0)
            {
                var command = FrameCommands.Dequeue();
                var tank = (Tank)Tanks[command.OwnerId];
                tank.TickTank();
                // - check if tank is alive ----
                if (tank.Health <= 0)
                    continue;
                // - Process Fire ----
                HandleFire(command, tank, frame);
                // - Process Movement ----
                HandleMovement(command, tank);
            }

            // - Build the Frame String ----
            foreach (Tank tank in Tanks.Values)
            {
                frame.Append(tank);
                if (tank.IsDead)
                    tank.IsDead = false;
                else if (tank.HasJoined)
                    tank.HasJoined = false;
                if (tank.IsDisconnected)
                    Tanks.Remove(tank.Id);
            }
            foreach (Projectile proj in Projectiles.Values)
            {
                frame.Append(proj);
                if (proj.IsDead)
                    Projectiles.Remove(proj.Id);
            }
            foreach (Powerup powerup in Powerups.Values)
            {
                frame.Append(powerup);
                if (powerup.IsDead)
                    Powerups.Remove(powerup.Id);
            }

            // - return the frame to send to all of the clients ----
            return frame.ToString();
        }


        /// <summary>
        /// Handle determining projectiles movements and if they
        /// collide, deletes the projectile and deals with the object
        /// it collided with.
        /// </summary>
        private void HandleProjectiles()
        {
            foreach (Projectile proj in Projectiles.Values)
            {
                proj.Location += proj.Direction * ProjectileSpeed;

                var obj = CheckCollision(proj, proj.Location, 0);

                if (obj == null)
                    continue;
                else if (obj.GetType() == typeof(Tank))
                {
                    var tank = obj as Tank;
                    if (proj.OwnerId == tank.Id)
                        continue;
                    proj.IsDead = true;
                    if (--tank.Health > 0)
                        continue;
                    if (Tanks.Keys.Contains(proj.OwnerId))
                        ((Tank)Tanks[proj.OwnerId]).Score++;
                    tank.IsDead = true;
                }
                else
                    proj.IsDead = true;
            }
        }


        /// <summary>
        /// Generates a new powerup if a certain amount of time has passed and
        /// there exists less powerups than MaxPowerups.
        /// </summary>
        private void HandlePowerupSpawn()
        {
            // - stop proccessing if it's not time to spawn a powerup ----
            if (FramesTillNextPowerupSpawn != 0 || Powerups.Count >= MaxPowerups)
                return;

            // - figure out powerup Ids ----
            int id = 0;
            while (Powerups.Keys.Contains(id) && id <= MaxPowerups)
                id++;

            // - Create the powerup object ----
            var pow = new Powerup(id);

            // - Figure out powerup location ----
            var loc = GetRandomValidLocation(pow, 0);

            if (loc == null)    // failed to get a location and therefore create a powerup
                return;

            pow.Location = loc;

            // - add powerup to the world ----
            Powerups.Add(pow);
        }

        /// <summary>
        /// Handle creating projectiles and beams.
        /// For beams process them and append them to the from.
        /// </summary>
        private void HandleFire(Command cmd, Tank tank, StringBuilder frame)
        {
            switch (cmd.Fire)
            {
                case "main":
                    if (tank.CanFire)
                    {
                        Projectiles.Add(new Projectile(cmd.OwnerId, tank.Location, cmd.Direction));
                        tank.CanFire = false;
                    }
                    return;
                case "alt":
                    if (tank.BeamChargeCount > 0)
                    {
                        var beam = new Beam(cmd.OwnerId, tank.Location, cmd.Direction);
                        frame.Append(beam);
                        tank.BeamChargeCount--;
                        foreach (Tank otherTank in Tanks.Values)
                            if (otherTank.Id != tank.Id && BeamIntersects(beam.Origin, beam.Direction, otherTank.Location, TankSize / 2))
                            {
                                otherTank.Health = 0;
                                otherTank.IsDead = true;
                                tank.Score++;
                            }
                    }
                    return;
                case "none":
                default:
                    return;
            }
        }


        /// <summary>
        /// Handle the movement of the tanks.
        /// </summary>
        private void HandleMovement(Command cmd, Tank tank)
        {
            tank.TurretDirection = cmd.Direction;
            switch (cmd.Movement)
            {
                case "up":
                    tank.Direction = V_UP;
                    break;
                case "down":
                    tank.Direction = V_DOWN;
                    break;
                case "left":
                    tank.Direction = V_LEFT;
                    break;
                case "right":
                    tank.Direction = V_RIGHT;
                    break;
                case "none":
                default:
                    return;
            }

            // - Check Collisions ----
            var newLoc = tank.Location + tank.Direction * TankSpeed;
            var obj = CheckCollision(tank, newLoc, TankSize / 2);

            if (obj == null)
            {
                tank.Location = newLoc;
                return;
            }

            Type t = obj.GetType();
            if (t == typeof(Powerup))       // Obtain powerup and move
            {
                tank.BeamChargeCount++;
                ((Powerup)Powerups[(obj as Powerup).Id]).IsDead = true;
                tank.Location = newLoc;
                return;
            }
            else if (t == typeof(HorizontalBorder))     // Reappear on the other Horizontal Border
            {
                var borderLoc = tank.Direction.Y * MaxCoordinate;
                tank.Location = new Vector2D(tank.Location.X,
                                        -1 * tank.Direction.Y * (MaxCoordinate - (TankSpeed - Math.Abs(borderLoc - tank.Location.Y))));
            }
            else if (t == typeof(VerticalBorder))       // Reappear on the other Vertical Border
            {
                var borderLoc = tank.Direction.X * MaxCoordinate;
                tank.Location = new Vector2D(-1 * tank.Direction.X * (MaxCoordinate - (TankSpeed - Math.Abs(borderLoc - tank.Location.X))),
                                                tank.Location.Y);
            }
            else if (t == typeof(Wall))         // Move up to the border of the wall object
            {
                return;
                // var wall = obj as Wall;
                // if (wall.IsHorizontal)
                //     if (cmd.Movement[0] == 'u' || cmd.Movement[0] == 'd')
                //         tank.Location = new Vector2D(tank.Location.X,
                //                                 wall.PUp.Y + -tank.Direction.Y * ((TankSize + WallSize) / 2 + TankSpeed));
                //     else if ((wall.PUp - newLoc).Length() < (wall.PLow - newLoc).Length())
                //         tank.Location = new Vector2D(wall.PUp.X + -tank.Direction.X * ((TankSize + WallSize) / 2 + TankSpeed),
                //                                         tank.Location.Y);
                //     else
                //         tank.Location = new Vector2D(wall.PLow.X + -tank.Direction.X * ((TankSize + WallSize) / 2 + TankSpeed),
                //                                         tank.Location.Y);
                // else
                //     if (cmd.Movement[0] == 'l' || cmd.Movement[0] == 'r')
                //     tank.Location = new Vector2D(wall.PUp.X + -tank.Direction.X * ((TankSize + WallSize) / 2 + TankSpeed),
                //                                     tank.Location.Y);
                // else if ((wall.PUp - newLoc).Length() < (wall.PLow - newLoc).Length())
                //     tank.Location = new Vector2D(tank.Location.X,
                //                             wall.PUp.Y + -tank.Direction.Y * (TankSize + WallSize) / 2 + TankSpeed);
                // else
                //     tank.Location = new Vector2D(tank.Location.X,
                //                             wall.PLow.Y + -tank.Direction.Y * (TankSize + WallSize) / 2 + TankSpeed);
            }

        }


        /// <summary>
        /// Generate a new tank for a player to control and assign it to them
        /// </summary>
        /// <param name="id">An int representing the player's unique id</param>
        /// <param name="playerName">A string to be used as the player's name</param>
        public bool CreateNewPlayer(int id, string playerName)
        {
            var tank = new Tank(id, playerName);
            bool success;
            lock (this)
            {
                tank.HasJoined = true;
                success = SpawnTank(tank);
                if (success)
                    Tanks.Add(tank);
            }
            return success;
        }


        /// <summary>
        /// Set <paramref name="tank"/>'s location to a new random but valid location,
        ///  then set the health to <see cref="StartingHealth"/>.
        /// </summary>
        /// <param name="tank"></param>
        /// <returns><c>true</c> if the tank was successfully respawned
        ///  (primarily dependint on if a valid random location can be generated in a 
        ///    reasonable amount of attempts).</returns>
        private bool SpawnTank(Tank tank)
        {
            // - Figure out a location to spawn a tank ----
            var loc = GetRandomValidLocation(tank, TankSize / 2);

            // failed to get a location this frame and therefore didn't place the tank
            if (loc == null)
                return false;

            tank.Location = loc;

            // - Reset Tank life stats ---
            tank.Health = StartingHealth;

            return true;
        }


        /// <summary>
        /// Checks all tanks in the world to see if they are dead, if so, increments
        /// their respawn timer or respawns them if it is time to do so.
        /// </summary>
        private void RespawnTanks()
        {
            foreach (Tank tank in Tanks.Values)
                if (tank.Health <= 0 && tank.FramesTillRespawn <= 0)
                    SpawnTank(tank);
        }


        /// <summary>
        /// Prepares to remove a player and their Tank from the game world
        /// </summary>
        /// <param name="id">the id of the player/tank to remove</param>
        /// <returns><code>true</code> if player was removed, or does not exist in the game world anyway.</returns>
        public bool RemovePlayer(int id)
        {
            try
            {
                ((Tank)Tanks[id]).IsDisconnected = true;
                return true;
            }
            catch {/* DO NOTHING */}
            return false;
        }


        /// <summary>
        /// Accept a new command created by a client to be excecuted
        /// </summary>
        /// <param name="id">An int with the unique identifier of the client that sent the command</param>
        /// <param name="command">A command object representing the clients request</param>
        public void RegisterCommand(int id, string commandJson)
        {
            var command = JsonConvert.DeserializeObject<Command>(commandJson);
            command.OwnerId = id;

            if (FrameCommands.Contains(command))
                return;

            lock (this)
            {
                FrameCommands.Enqueue(command);
            }
        }


        /// <summary>
        /// Returns a collection of all walls in the world
        /// </summary>
        /// <returns></returns>
        public IEnumerable<string> GetWalls()
        {
            foreach (Wall wall in Walls.Values)
                yield return wall.ToString();
        }

    }
}
