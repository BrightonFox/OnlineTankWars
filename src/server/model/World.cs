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

        private int _nextProjId = 0;
        private int NextProjId
        {
            // ! this has a problem of overflow if the server runs for >7 years and 10 projectiles are fired every second
            get { return _nextProjId++; }
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
                // - Process Fire ----
                HandleFire(command, frame);
                // - Process Movement ----
                HandleMovement(command);
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
                frame.Append(powerup);

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

                Type t = obj.GetType();
                if (t == typeof(Tank))
                {
                    var tank = obj as Tank;
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
            Powerups.Add(id, pow);
        }

        /// <summary>
        /// Handle creating projectiles and beams.
        /// For beams process them and append them to the from.
        /// </summary>
        private void HandleFire(Command cmd, StringBuilder frame)
        {
            var tank = (Tank)Tanks[cmd.OwnerId];
            switch (cmd.Fire)
            {
                case "main":
                    int projId = NextProjId;
                    Projectiles.Add(projId, new Projectile(projId, cmd.OwnerId, tank.Location, cmd.Direction));
                    return;
                case "alt":
                    var beam = new Beam(NextProjId, cmd.OwnerId, tank.Location, cmd.Direction);
                    frame.Append(beam);
                    foreach (Tank otherTank in Tanks.Values)
                        if (BeamIntersects(beam.Origin, beam.Direction, tank.Location, TankSize/2))
                        {
                            otherTank.Health = 0;
                            otherTank.IsDead = true;
                            tank.Score++;
                        }
                    return;
                case "none":
                default:
                    return;
            }
        }


        /// <summary>
        /// Determines if a ray intersects a circle, used to determine if a beam hits a tank
        /// </summary>
        /// <param name="rayOrig">The origin of the ray</param>
        /// <param name="rayDir">The direction of the ray</param>
        /// <param name="center">The center of the circle</param>
        /// <param name="r">The radius of the circle</param>
        /// <returns></returns>
        public static bool BeamIntersects(Vector2D rayOrig, Vector2D rayDir, Vector2D center, double r)
        {
            // ray-circle intersection test
            // P: hit point
            // ray: P = O + tV
            // circle: (P-C)dot(P-C)-r^2 = 0
            // substituting to solve for t gives a quadratic equation:
            // a = VdotV
            // b = 2(O-C)dotV
            // c = (O-C)dot(O-C)-r^2
            // if the discriminant is negative, miss (no solution for P)
            // otherwise, if both roots are positive, hit

            double a = rayDir.Dot(rayDir);
            double b = ((rayOrig - center) * 2.0).Dot(rayDir);
            double c = (rayOrig - center).Dot(rayOrig - center) - r * r;

            // discriminant
            double disc = b * b - 4.0 * a * c;

            if (disc < 0.0)
                return false;

            // find the signs of the roots
            // technically we should also divide by 2a
            // but all we care about is the sign, not the magnitude
            double root1 = -b + Math.Sqrt(disc);
            double root2 = -b - Math.Sqrt(disc);

            return (root1 > 0.0 && root2 > 0.0);
        }


        /// <summary>
        /// Handle the movement of the tanks.
        /// </summary>
        private void HandleMovement(Command cmd)
        {
            var tank = (Tank)Tanks[cmd.OwnerId];
            tank.TurretDirection = cmd.Direction;
            Vector2D modLoc;
            switch (cmd.Movement)
            {
                case "up":
                    modLoc = new Vector2D(0, -TankSpeed);
                    break;
                case "down":
                    modLoc = new Vector2D(0, TankSpeed);
                    break;
                case "left":
                    modLoc = new Vector2D(-TankSpeed, 0);
                    break;
                case "right":
                    modLoc = new Vector2D(TankSpeed, 0);
                    break;
                case "none":
                default:
                    return;
            }

            // - Check Collisions ----
            var newLoc = tank.Location + modLoc;
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
                var polarity = TankSpeed / modLoc.GetY();
                var borderLoc = polarity * MaxCoordinate;
                tank.Location = new Vector2D(tank.Location.GetX(),
                                        -1 * polarity * (MaxCoordinate - (TankSpeed - Math.Abs(borderLoc - tank.Location.GetY()))));
            }
            else if (t == typeof(VerticalBorder))       // Reappear on the other Vertical Border
            {
                var polarity = TankSpeed / modLoc.GetY();
                var borderLoc = polarity * MaxCoordinate;
                tank.Location = new Vector2D(-1 * polarity * (MaxCoordinate - (TankSpeed - Math.Abs(borderLoc - tank.Location.GetX()))),
                                                tank.Location.GetY());
            }
            else if (t == typeof(Wall))         // Move up to the border of the wall object
            {
                var wall = obj as Wall;
                if (wall.isHorizontal)
                    if (cmd.Movement[0] == 'u' || cmd.Movement[0] == 'd')
                        tank.Location = new Vector2D(tank.Location.GetX(),
                                                wall.PUp.GetY() + (TankSpeed / modLoc.GetY()) * (TankSize + WallSize) / 2);
                    else if ((wall.PUp - newLoc).Length() < (wall.PLow - newLoc).Length())
                        tank.Location = new Vector2D(wall.PUp.GetX() + (TankSpeed / modLoc.GetX()) * (TankSize + WallSize) / 2,
                                                        tank.Location.GetY());
                    else
                        tank.Location = new Vector2D(wall.PLow.GetX() + (TankSpeed / modLoc.GetX()) * (TankSize + WallSize) / 2,
                                                        tank.Location.GetY());
                else
                    if (cmd.Movement[0] == 'l' || cmd.Movement[0] == 'r')
                    tank.Location = new Vector2D(wall.PUp.GetX() + (TankSpeed / modLoc.GetX()) * (TankSize + WallSize) / 2,
                                                    tank.Location.GetY());
                else if ((wall.PUp - newLoc).Length() < (wall.PLow - newLoc).Length())
                    tank.Location = new Vector2D(tank.Location.GetX(),
                                            wall.PUp.GetY() + (TankSpeed / modLoc.GetY()) * (TankSize + WallSize) / 2);
                else
                    tank.Location = new Vector2D(tank.Location.GetX(),
                                            wall.PLow.GetY() + (TankSpeed / modLoc.GetY()) * (TankSize + WallSize) / 2);
            }

        }


        /// <summary>
        /// Generate a new tank for a player to control and assign it to them
        /// </summary>
        /// <param name="id">An int representing the player's unique id</param>
        /// <param name="playerName">A string to be used as the player's name</param>
        public bool CreateNewPlayer(int id, string playerName)
        {
            var tank = new Tank(id, playerName, RespawnDelay);
            lock (this)
            {
                Tanks.Add(id, tank);
                tank.HasJoined = true;
                return SpawnTank(tank);
            }
        }
        

        /// <summary>
        /// Attempts to find a random location in the world that the passed
        /// <paramref name="obj"/> can spawn without collisions. 
        /// </summary>
        private Vector2D GetRandomValidLocation(object obj, int radius)
        {
            int attempts = 64;
            Vector2D loc;
            do
            {
                loc = new Vector2D(rand.Next(-MaxCoordinate, MaxCoordinate),
                                    rand.Next(-MaxCoordinate, MaxCoordinate));
                if (attempts-- <= 0)
                    return null;
            } while (CheckCollision(obj, loc, radius) == null);
            return loc;
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
        /// Takes in an object (<paramref name="obj"/>), 
        ///  its centerpoint location adjusted for movement (<paramref name="loc"/>), 
        ///  and its radius (<paramref name="radius"/>) 
        ///  as the collision engine cares about it.
        /// <para>
        /// <paramref name="obj"/> Should be a <see cref="Tank"/>,
        ///   <see cref="Projectile"/>, or a <see cref="Powerup"/>
        /// </para>
        /// !! <paramref name="obj"/> Should never be a <see cref="Wall"/> 
        ///   or a <see cref="Beam"/> Object !!
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="loc">adjusted to movement</param>
        /// <param name="radius"></param>
        /// <returns>
        /// A <seealso cref="ValueTuple"/> that contins the (int) id, 
        ///  the type of the object <paramref name="obj"/> object collides with,
        ///  and the object <paramref name="obj"/> colidded with.
        /// </returns>
        private object CheckCollision(object obj, Vector2D loc, int radius)
        {
            Type type = obj.GetType();

            // - Check Collision with wall ----
            foreach (Wall wall in Walls.Values)
                if (wall.isHorizontal)
                {
                    if (loc.GetX() < wall.PUp.GetX() + WallSize / 2 && loc.GetX() > wall.PLow.GetX() - WallSize / 2)
                        if (Math.Abs(loc.GetY() - wall.PUp.GetY()) <= WallSize / 2 + radius)
                            return wall;
                }
                else
                    if (loc.GetY() < wall.PUp.GetY() + WallSize / 2 && loc.GetY() > wall.PLow.GetY() - WallSize / 2)
                    if (Math.Abs(loc.GetX() - wall.PUp.GetX()) <= WallSize / 2 + radius)
                        return wall;

            // - Check Tank collision ----
            if (type != typeof(Tank))
                foreach (Tank tank in Tanks.Values)
                    if (tank.Health > 0 && isCollision(loc, radius, tank.Location, TankSize / 2))
                        return tank;

            // - Check Projectile Collision ----
            if (type != typeof(Powerup))
                foreach (Projectile proj in Projectiles.Values)
                    if (isCollision(loc, radius, proj.Location, 0))
                        return proj;

            // - Check Powerup Collision ----
            if (type != typeof(Projectile))
                foreach (Powerup powerup in Powerups.Values)
                    if (isCollision(loc, radius, powerup.Location, 0))
                        return powerup;

            // - Check Colision with edge of world ----
            if (type != typeof(Powerup))
                if (loc.GetX() > MaxCoordinate || loc.GetX() < -MaxCoordinate)
                    return new VerticalBorder();
                else if (loc.GetY() > MaxCoordinate || loc.GetY() < -MaxCoordinate)
                    return new HorizontalBorder();

            // - Return null if no collisions happen ----
            return null;
        }


        private bool isCollision(Vector2D loc1, int r1, Vector2D loc2, int r2)
        {
            return (loc1 - loc2).Length() <= r1 + r2;
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
        public IEnumerable<Wall> GetWalls()
        {
            return Walls.Values.AsEnumerable() as IEnumerable<Wall>;
        }


        /// <summary>
        /// Used in <see cref="World.CheckCollision"/> 
        ///  to indicate when an item has collided with a horizontal border of the world.
        /// </summary>
        private class HorizontalBorder
        {
        }

        /// <summary>
        /// Used in <see cref="World.CheckCollision" /> 
        ///  to indicate when an item has collided with a vertical border of the world.
        /// </summary>
        private class VerticalBorder
        {
        }
    }
}
