using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using TankWars.Client.Model;

namespace TankWars.Client.View
{
    public class DrawingPanel : Panel
    {
        public delegate void ObjectDrawer(object o, PaintEventArgs e);

        private World world;
        private int viewSize;

        public DrawingPanel(World w, int viewSize)
        {
            DoubleBuffered = true;
            world = w;
            this.viewSize = viewSize;
        }

        public void GetTargetPos(out MathUtils.Vector2D targetPos)
        {
            targetPos = new MathUtils.Vector2D(MousePosition.X, MousePosition.Y);

        }

        /// <summary>
        /// Helper method for DrawObjectWithTransform
        /// </summary>
        /// <param name="size">The world (and image) size</param>
        /// <param name="w">The worldspace coordinate</param>
        /// <returns></returns>
        private static int WorldSpaceToImageSpace(int size, double w)
        {
            return (int)w + size / 2;
        }

        private static string GetColorFromId(int id)
        {
            switch (id % 8)
            {
                case 0:
                    return "Blue";
                case 1:
                    return "DarkBlue";
                case 2:
                    return "Green";
                case 3:
                    return "LightGreen";
                case 4:
                    return "Orange";
                case 5:
                    return "Purple";
                case 6:
                    return "Red";
                case 7:
                    return "Yellow";
            }
            return "Major issue if you somehow see this";
        }


        /// <summary>
        /// This method performs a translation and rotation to drawn an object in the world.
        /// </summary>
        /// <param name="e">PaintEventArgs to access the graphics (for drawing)</param>
        /// <param name="o">The object to draw</param>
        /// <param name="worldSize">The size of one edge of the world (assuming the world is square)</param>
        /// <param name="worldX">The X coordinate of the object in world space</param>
        /// <param name="worldY">The Y coordinate of the object in world space</param>
        /// <param name="angle">The orientation of the object, measured in degrees clockwise from "up"</param>
        /// <param name="drawer">The drawer delegate. After the transformation is applied, the delegate is invoked to draw whatever it wants</param>
        private void DrawObjectWithTransform(PaintEventArgs e, object o, int worldSize, double worldX, double worldY, double angle, ObjectDrawer drawer)
        {
            // "push" the current transform
            System.Drawing.Drawing2D.Matrix oldMatrix = e.Graphics.Transform.Clone();

            int x = WorldSpaceToImageSpace(worldSize, worldX);
            int y = WorldSpaceToImageSpace(worldSize, worldY);
            e.Graphics.TranslateTransform(x, y);
            e.Graphics.RotateTransform((float)angle);
            drawer(o, e);

            // "pop" the transform
            e.Graphics.Transform = oldMatrix;
        }

        #region Object Drawers
        /// <summary>
        /// Acts as a drawing delegate for DrawObjectWithTransform
        /// After performing the necessary transformation (translate/rotate)
        /// DrawObjectWithTransform will invoke this method
        /// </summary>
        /// <param name="o">The object to draw</param>
        /// <param name="e">The PaintEventArgs to access the graphics</param>
        private void PlayerDrawer(object o, PaintEventArgs e)
        {
            Tank p = o as Tank;

            int healthWidth = 60;
            if (p.Health == 2)
                healthWidth = 40;
            else if (p.Health == 1)
                healthWidth = 20;
            else if (p.Health == 0)
                healthWidth = 1;

            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            using (SolidBrush greenBrush = new SolidBrush(Color.LightGreen))
            using (SolidBrush blackBrush = new SolidBrush(Color.Black))
            {
                var health = new Rectangle(-30, -32, healthWidth, 2);
                e.Graphics.FillRectangle(greenBrush, health);

                e.Graphics.DrawString(p.PlayerName + ": " + p.Score.ToString(), new Font("Consolas", 6), blackBrush, -32, 32);
                
                // e.Graphics.DrawString(p.Score.ToString(), new Font("Consolas", 6), blackBrush, -28, 34);
            }
        }

        private void TankDrawer(object o, PaintEventArgs e)
        {
            Tank t = o as Tank;
            string color = GetColorFromId(t.Id);
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            using (Image body = Image.FromFile($"img/{color}Tank.png"))
            using (Image turret = Image.FromFile($"img/{color}Turret.png"))
            {
                e.Graphics.DrawImage(body, -32, -32, 64, 64);
                e.Graphics.DrawImage(turret, -32, -32, 64, 64);
            }
        }

        /// <summary>
        /// Acts as a drawing delegate for DrawObjectWithTransform
        /// After performing the necessary transformation (translate/rotate)
        /// DrawObjectWithTransform will invoke this method
        /// </summary>
        /// <param name="o">The object to draw</param>
        /// <param name="e">The PaintEventArgs to access the graphics</param>
        private void PowerupDrawer(object o, PaintEventArgs e)
        {
            Powerup p = o as Powerup;

            int width = 8;
            int height = 8;
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            using (SolidBrush redBrush = new SolidBrush(Color.Red))
            using (SolidBrush whiteBrush = new SolidBrush(Color.White))
            {
                // Circles are drawn starting from the top-left corner.
                // So if we want the circle centered on the powerups location, we have to offset it
                // by half its size to the left (-width/2) and up (-height/2)
                Rectangle r = new Rectangle(-(width / 2), -(height / 2), width, height);
                Rectangle r2 = new Rectangle(-((width / 2) + width / 2), -((height / 2) + height / 2), width / 2, height / 2);

                e.Graphics.FillEllipse(redBrush, r);
                e.Graphics.FillEllipse(whiteBrush, r2);
            }
        }



        private void ProjectileDrawer(object o, PaintEventArgs e)
        {
            Projectile p = o as Projectile;
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            using (Image proj = Image.FromFile($"img/{GetColorFromId(p.OwnerId)}Shot.png"))
            {
                e.Graphics.DrawImage(proj, -16, -16, 32, 32);
            }
        }


        private void WallDrawer(object o, PaintEventArgs e)
        {
            Wall w = o as Wall;

            int wallWidth = (int)Math.Abs(w.P1.GetX()-w.P2.GetX());
            int wallHeight = (int)Math.Abs(w.P1.GetY()-w.P2.GetY());
            int width = 50;

            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            using (Image wall = Image.FromFile("img/WallSprite.png"))
            {
                if (wallWidth / width == 1)
                    for (int jj=0; jj<wallHeight/width; jj++)
                        e.Graphics.DrawImage(wall, -width/2, (-wallHeight/2)+(width*jj), width, width);
                else
                    for (int ii = 0; ii < wallWidth / width; ii++)
                        e.Graphics.DrawImage(wall, (-wallWidth/2)+(width*ii), -width/2, width, width );
            }
        }


        private void BeamDrawer(object o, PaintEventArgs e)
        {
            Beam b = o as Beam;

            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            using (SolidBrush redBrush = new System.Drawing.SolidBrush(Color.FromArgb(128, 255, 0, 0)))
            {
                var beam = new Rectangle(0, 0, world.size*2, 4);
                e.Graphics.FillEllipse(redBrush, beam);
            }
        }


        private void DrawBackground(PaintEventArgs e)
        {
            using (Image backgroundImg = Image.FromFile("img/Background.png"))
            {
                e.Graphics.DrawImage(backgroundImg, 0, 0, world.size, world.size);
            }
        }
        #endregion

        // This method is invoked when the DrawingPanel needs to be re-drawn
        protected override void OnPaint(PaintEventArgs e)
        {
            lock (world)
            {
                double playerX = world.GetTank(world.Player.Id).Location.GetX();
                double playerY = world.GetTank(world.Player.Id).Location.GetY();

                // - calculate view/world size ratio
                double ratio = (double)viewSize / (double)world.size;
                int halfSizeScaled = (int)(world.size / 2.0 * ratio);

                double inverseTranslateX = -WorldSpaceToImageSpace(world.size, playerX) + halfSizeScaled;
                double inverseTranslateY = -WorldSpaceToImageSpace(world.size, playerY) + halfSizeScaled;

                e.Graphics.TranslateTransform((float)inverseTranslateX, (float)inverseTranslateY);

                // - Draw Background
                DrawBackground(e);

                // // - Draw the Player
                // var player = world.GetTank(world.Player.Id);
                // DrawObjectWithTransform(e, player, world.size, player.Location.GetX(), player.Location.GetY(), player.Direction.ToAngle(), PlayerDrawer);

                // - Draw the walls
                foreach (int wallId in world.GetWallIds())
                {
                    var wall = world.GetWall(wallId);
                    DrawObjectWithTransform(e, wall, world.size, wall.P1.GetX(), wall.P1.GetY(), 0, WallDrawer);
                }

                // - Draw the tanks
                foreach (int tankId in world.GetTankIds())
                {
                    // if (tankId == world.Player.Id)
                    //     continue;
                    var tank = world.GetTank(tankId);
                    DrawObjectWithTransform(e, tank, world.size, tank.Location.GetX(), tank.Location.GetY(), tank.Direction.ToAngle(), TankDrawer);
                    DrawObjectWithTransform(e, tank, world.size, tank.Location.GetX(), tank.Location.GetY(), 0, PlayerDrawer);
                }

                // - Draw the powerups
                foreach (int powId in world.GetPowerupIds())
                {
                    var pow = world.GetPowerup(powId);
                    DrawObjectWithTransform(e, pow, world.size, pow.Location.GetX(), pow.Location.GetY(), 0, PowerupDrawer);
                }

                // - Draw the projectiles
                foreach (int projId in world.GetProjectileIds())
                {
                    var proj = world.GetProjectile(projId);
                    DrawObjectWithTransform(e, proj, world.size, proj.Location.GetX(), proj.Location.GetY(), proj.Direction.ToAngle(), ProjectileDrawer);
                }

                // - Draw the projectiles
                foreach (int beamId in world.GetBeamIds())
                {
                    var beam = world.GetBeam(beamId);
                    DrawObjectWithTransform(e, beam, world.size, beam.Origin.GetX(), beam.Origin.GetY(), beam.Direction.ToAngle(), BeamDrawer);
                }
            }

            // Do anything that Panel (from which we inherit) needs to do
            base.OnPaint(e);
        }
    }
}

