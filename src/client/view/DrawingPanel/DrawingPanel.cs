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
        private static string imgDir = "../../../../../../res/img";

        #region Images
        private ImageCollection BlueImages = new ImageCollection("Blue");
        private ImageCollection DarkImages = new ImageCollection("Dark");
        private ImageCollection GreenImages = new ImageCollection("Green");
        private ImageCollection LightGreenImages = new ImageCollection("LightGreen");
        private ImageCollection OrangeImages = new ImageCollection("Orange");
        private ImageCollection PurpleImages = new ImageCollection("Purple");
        private ImageCollection RedImages = new ImageCollection("Red");
        private ImageCollection YellowImages = new ImageCollection("Yellow");

        private Image wallImage = Image.FromFile($"{imgDir}/WallSprite.png");
        private Image backgroundImage = Image.FromFile($"{imgDir}/Background.png");

        #endregion


        public DrawingPanel(World w, int viewSize)
        {
            DoubleBuffered = true;
            world = w;
            this.viewSize = viewSize;
            this.BackColor = Color.FromArgb(200, 16, 32, 32);
        }

        ~DrawingPanel()
        {
            DisposeOfImages();
        }

        public MathUtils.Vector2D GetTargetPos()
        {
            var mousePos = this.PointToClient(MousePosition);
            return new MathUtils.Vector2D(mousePos.X - (viewSize / 2),
                                                mousePos.Y - (viewSize / 2));
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

        private ImageCollection GetColorFromId(int id)
        {
            switch (id % 8)
            {
                case 0:
                    return BlueImages;
                case 1:
                    return DarkImages;
                default:
                case 2:
                    return GreenImages;
                case 3:
                    return LightGreenImages;
                case 4:
                    return OrangeImages;
                case 5:
                    return PurpleImages;
                case 6:
                    return RedImages;
                case 7:
                    return YellowImages;
            }
        }

        public void DisposeOfImages()
        {
            BlueImages.Dispose();
            DarkImages.Dispose();
            GreenImages.Dispose();
            LightGreenImages.Dispose();
            OrangeImages.Dispose();
            PurpleImages.Dispose();
            RedImages.Dispose();
            YellowImages.Dispose();
            wallImage.Dispose();
            backgroundImage.Dispose();
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

            int maxHealthWidth = 60;
            int healthWidth = 60;
            if (p.Health == 2)
                healthWidth = 40;
            else if (p.Health == 1)
                healthWidth = 20;
            else if (p.Health == 0)
                healthWidth = 1;

            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            using (SolidBrush greenBrush = new SolidBrush(Color.LightGreen))
            using (SolidBrush whiteBrush = new SolidBrush(Color.White))
            using (SolidBrush transBlackBrush = new SolidBrush(Color.FromArgb(128, 0, 0, 0)))
            {
                var healthBkgd = new Rectangle(-30, -40, maxHealthWidth, 4);
                e.Graphics.FillRectangle(transBlackBrush, healthBkgd);

                var health = new Rectangle(-30, -40, healthWidth, 4);
                e.Graphics.FillRectangle(greenBrush, health);

                e.Graphics.DrawString(p.PlayerName + ": " + p.Score.ToString(), new Font("Consolas", 12), whiteBrush, -40, 32);

                // e.Graphics.DrawString(p.Score.ToString(), new Font("Consolas", 6), blackBrush, -28, 34);
            }
        }

        private void TankDrawer(object o, PaintEventArgs e)
        {
            Tank t = o as Tank;
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            e.Graphics.DrawImage(GetColorFromId(t.Id).Body, -32, -32, 64, 64);
        }

        private void TurretDrawer(object o, PaintEventArgs e)
        {
            Tank t = o as Tank;
            Image turret = GetColorFromId(t.Id).Turret;
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            e.Graphics.DrawImage(turret, -32, -32, 64, 64);
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
                Rectangle r2 = new Rectangle(-(width / 4), -(height / 4), width / 2, height / 2);

                e.Graphics.FillEllipse(redBrush, r);
                e.Graphics.FillEllipse(whiteBrush, r2);
            }
        }



        private void ProjectileDrawer(object o, PaintEventArgs e)
        {
            Projectile p = o as Projectile;
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            e.Graphics.DrawImage(GetColorFromId(p.OwnerId).Projectile, -16, -16, 32, 32);
        }


        private void WallDrawer(object o, PaintEventArgs e)
        {
            Wall w = o as Wall;

            int wallWidth = (int)Math.Abs(w.P1.GetX() - w.P2.GetX());
            int wallHeight = (int)Math.Abs(w.P1.GetY() - w.P2.GetY());
            int unitSize = 50;

            if (wallWidth == 0)
                wallWidth = unitSize;
            if (wallHeight == 0)
                wallHeight = unitSize;

            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            if (wallWidth / unitSize == 1)
                for (int jj = 0; jj <= wallHeight / unitSize; jj++)
                    e.Graphics.DrawImage(wallImage, -unitSize / 2, (-unitSize / 2) + (unitSize * jj), unitSize, unitSize);
            else
                for (int ii = 0; ii <= wallWidth / unitSize; ii++)
                    e.Graphics.DrawImage(wallImage, (-unitSize / 2) + unitSize * ii, -unitSize / 2, unitSize, unitSize);
        }


        private void BeamDrawer(object o, PaintEventArgs e)
        {
            Beam b = o as Beam;

            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            using (SolidBrush redBrush = new System.Drawing.SolidBrush(Color.FromArgb(128, 255, 0, 0)))
            {
                var beam = new Rectangle(0, 0, 4, -world.size * 2);
                e.Graphics.FillEllipse(redBrush, beam);
            }
        }


        private void DrawBackground(PaintEventArgs e)
        {
            e.Graphics.DrawImage(backgroundImage, 0, 0, world.size, world.size);
        }
        #endregion

        // This method is invoked when the DrawingPanel needs to be re-drawn
        protected override void OnPaint(PaintEventArgs e)
        {
            if (world.HasPlayerTank())
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
                        DrawObjectWithTransform(e, wall, world.size,
                                                ((wall.P1.GetX() <= wall.P2.GetX()) ? wall.P1.GetX() : wall.P2.GetX()),
                                                ((wall.P1.GetY() <= wall.P2.GetY()) ? wall.P1.GetY() : wall.P2.GetY()),
                                                0, WallDrawer);
                    }

                    // - Draw the tanks
                    foreach (int tankId in world.GetTankIds())
                    {
                        var tank = world.GetTank(tankId);
                        if (tank.Health == 0)
                            continue;
                        DrawObjectWithTransform(e, tank, world.size, tank.Location.GetX(), tank.Location.GetY(), tank.Direction.ToAngle(), TankDrawer);
                        DrawObjectWithTransform(e, tank, world.size, tank.Location.GetX(), tank.Location.GetY(), tank.TurretDirection.ToAngle(), TurretDrawer);
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


        private class ImageCollection
        {
            public Image Turret;
            public Image Body;
            public Image Projectile;

            public ImageCollection(string color)
            {
                Turret = Image.FromFile($"{imgDir}/{color}Turret.png");
                Body = Image.FromFile($"{imgDir}/{color}Tank.png");
                Projectile = Image.FromFile($"{imgDir}/{color}Shot.png");
            }

            public void Dispose()
            {
                Turret.Dispose();
                Body.Dispose();
                Projectile.Dispose();
            }


        }
    }
}

