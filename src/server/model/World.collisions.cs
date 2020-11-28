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


namespace TankWars.Server.Model
{
    public partial class World
    {

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
                    if (loc.X < wall.PUp.X + WallSize / 2 && loc.X > wall.PLow.X - WallSize / 2)
                        if (Math.Abs(loc.Y - wall.PUp.Y) <= WallSize / 2 + radius)
                            return wall;
                }
                else
                    if (loc.Y < wall.PUp.Y + WallSize / 2 && loc.Y > wall.PLow.Y - WallSize / 2)
                    if (Math.Abs(loc.X - wall.PUp.X) <= WallSize / 2 + radius)
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
                if (loc.X > MaxCoordinate || loc.X < -MaxCoordinate)
                    return new VerticalBorder();
                else if (loc.Y > MaxCoordinate || loc.Y < -MaxCoordinate)
                    return new HorizontalBorder();

            // - Return null if no collisions happen ----
            return null;
        }


        private bool isCollision(Vector2D loc1, int r1, Vector2D loc2, int r2)
        {
            return (loc1 - loc2).Length() <= r1 + r2;
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
            // a = V dot V
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
