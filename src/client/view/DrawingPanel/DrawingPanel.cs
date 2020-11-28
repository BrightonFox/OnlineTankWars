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
 *   + v1.0 - Submittal - 2020/11/21
 * 
 * About:
 *   A modification of the winforms panel that draws the gameview 
 *    for the TankWars game.
 */

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

    /// <summary>
    /// A winform panel that draws the game view for the TankWars game.
    /// </summary>
    public class DrawingPanel : Panel
    {
        public delegate void ObjectDrawer(object o, PaintEventArgs e);

        private World world;
        private int viewSize;
        private static string imgDir = "../../../../../../res/img";

        #region Images
        // - All disposable graphic objects to be used in drawing the game world
        private ImageCollection BlueImages = new ImageCollection("Blue");
        private ImageCollection DarkImages = new ImageCollection("Dark");
        private ImageCollection GreenImages = new ImageCollection("Green");
        private ImageCollection LightGreenImages = new ImageCollection("LightGreen");
        private ImageCollection OrangeImages = new ImageCollection("Orange");
        private ImageCollection PurpleImages = new ImageCollection("Purple");
        private ImageCollection RedImages = new ImageCollection("Red");
        private ImageCollection YellowImages = new ImageCollection("Yellow");

        private Image WallImage = Image.FromFile($"{imgDir}/WallSprite.png");
        private Image ArenaBackgroundImage = Image.FromFile($"{imgDir}/Background.png");
        private Image HeartImage = Image.FromFile($"{imgDir}/heart.png");
        private Image EmptyHeartImage = Image.FromFile($"{imgDir}/heartEmpty.png");

        private SolidBrush TransBlackBrush = new SolidBrush(Color.FromArgb(128, 0, 0, 0));
        private SolidBrush WhiteBrush = new SolidBrush(Color.White);
        private SolidBrush LGreenBrush = new SolidBrush(Color.LightGreen);
        private SolidBrush TransRedBrush = new System.Drawing.SolidBrush(Color.FromArgb(128, 255, 0, 0));
        private SolidBrush RedBrush = new SolidBrush(Color.Red);

        private Font GameFont = new Font("Consolas", 12);
        #endregion
        

        /// <summary>
        /// A winform panel that draws the game view for the TankWars game.
        /// </summary>
        /// <param name="w">The world to be represented and used in the form</param>
        /// <param name="viewSize">The size of the drawing panel to be rendered</param>
        public DrawingPanel(World w, int viewSize)
        {
            DoubleBuffered = true;
            world = w;
            this.viewSize = viewSize;
            this.BackColor = Color.FromArgb(200, 16, 32, 32);
        }

        /// <summary>
        /// Assures that all graphic objects are disposed when this is no longer used
        /// </summary>
        ~DrawingPanel()
        {
            DisposeOfImages();
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


        /// <summary>
        /// Takes in the Id of a tank object and returns an image collection
        ///  with the requisite images of a color based upon the id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Will dispose of all of the disposable graphics items the
        /// drawing panel uses.
        /// </summary>
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

            WallImage.Dispose();
            ArenaBackgroundImage.Dispose();
            HeartImage.Dispose();
            EmptyHeartImage.Dispose();

            TransBlackBrush.Dispose();
            WhiteBrush.Dispose();
            LGreenBrush.Dispose();
            TransRedBrush.Dispose();
            RedBrush.Dispose();

            GameFont.Dispose();
        }


        /// <summary>
        /// This method performs a translation and rotation to draw an object in the world.
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
        /// Draws the player information like name, health, and score around the tanks.
        /// <para>
        /// Acts as a drawing delegate for DrawObjectWithTransform
        /// After performing the necessary transformation (translate/rotate)
        /// DrawObjectWithTransform will invoke this method. </para>
        /// </summary>
        /// <param name="o">The object to draw</param>
        /// <param name="e">The PaintEventArgs to access the graphics</param>
        private void PlayerDrawer(object o, PaintEventArgs e)
        {
            Tank p = o as Tank;

            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            for (int i = 1; i <= 3; i++)
                e.Graphics.DrawImage(
                    (i <= p.Health) ? HeartImage : EmptyHeartImage,
                    -30 + (20 * (i - 1)), -48, 20, 20);

            var playerInfo = p.PlayerName + ": " + p.Score.ToString();
            e.Graphics.DrawString(playerInfo, GameFont, WhiteBrush,
                                    -e.Graphics.MeasureString(playerInfo, GameFont).Width / 2, 32);

        }

        /// <summary>
        /// Draws the tank objects main body with color based on its Id.
        /// <para>
        /// Acts as a drawing delegate for DrawObjectWithTransform
        /// After performing the necessary transformation (translate/rotate)
        /// DrawObjectWithTransform will invoke this method. </para>
        /// </summary>
        /// <param name="o">The object to draw</param>
        /// <param name="e">The PaintEventArgs to access the graphics</param>
        private void TankDrawer(object o, PaintEventArgs e)
        {
            Tank t = o as Tank;
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            e.Graphics.DrawImage(GetColorFromId(t.Id).Body, -32, -32, 64, 64);
        }

        /// <summary>
        /// Draws the tank objects rotatable turret with color based on its Id.
        /// <para>
        /// Acts as a drawing delegate for DrawObjectWithTransform
        /// After performing the necessary transformation (translate/rotate)
        /// DrawObjectWithTransform will invoke this method. </para>
        /// </summary>
        /// <param name="o">The object to draw</param>
        /// <param name="e">The PaintEventArgs to access the graphics</param>
        private void TurretDrawer(object o, PaintEventArgs e)
        {
            Tank t = o as Tank;
            Image turret = GetColorFromId(t.Id).Turret;
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            e.Graphics.DrawImage(turret, -32, -32, 64, 64);
        }

        /// <summary>
        /// Draws the powerup object.
        /// <para>
        /// Acts as a drawing delegate for DrawObjectWithTransform
        /// After performing the necessary transformation (translate/rotate)
        /// DrawObjectWithTransform will invoke this method. </para>
        /// </summary>
        /// <param name="o">The object to draw</param>
        /// <param name="e">The PaintEventArgs to access the graphics</param>
        private void PowerupDrawer(object o, PaintEventArgs e)
        {
            Powerup p = o as Powerup;

            int width = 12;
            int height = 12;
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            Rectangle r = new Rectangle(-(width / 2), -(height / 2), width, height);
            Rectangle r2 = new Rectangle(-(width / 4), -(height / 4), width / 2, height / 2);

            e.Graphics.FillEllipse(RedBrush, r);
            e.Graphics.FillEllipse(WhiteBrush, r2);
        }


        /// <summary>
        /// Draws the projectile objects with color based on the tank that fired them.
        /// <para>
        /// Acts as a drawing delegate for DrawObjectWithTransform
        /// After performing the necessary transformation (translate/rotate)
        /// DrawObjectWithTransform will invoke this method. </para>
        /// </summary>
        /// <param name="o">The object to draw</param>
        /// <param name="e">The PaintEventArgs to access the graphics</param>
        private void ProjectileDrawer(object o, PaintEventArgs e)
        {
            Projectile p = o as Projectile;
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            e.Graphics.DrawImage(GetColorFromId(p.OwnerId).Projectile, -16, -16, 32, 32);
        }


        /// <summary>
        /// Draws the walls as a line of wall objects that is either horizontal or vertical
        ///  (the dif in X/Y in P1 & P2 should be 0 if it is vertical/horizontal, and the other diff should be divisible by 50 (respectively)).
        /// <para>
        /// Acts as a drawing delegate for DrawObjectWithTransform
        /// After performing the necessary transformation (translate/rotate)
        /// DrawObjectWithTransform will invoke this method. </para>
        /// </summary>
        /// <param name="o">The object to draw</param>
        /// <param name="e">The PaintEventArgs to access the graphics</param>
        private void WallDrawer(object o, PaintEventArgs e)
        {
            Wall w = o as Wall;

            int wallWidth = (int)Math.Abs(w.P1.X - w.P2.X);
            int wallHeight = (int)Math.Abs(w.P1.Y - w.P2.Y);
            int unitSize = 50;

            // determines if the passed wall is vertical or horizontal and tiles the graphic accordingly
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            if (wallWidth == 0)
                for (int jj = 0; jj <= wallHeight / unitSize; jj++)
                    e.Graphics.DrawImage(WallImage, -unitSize / 2, (-unitSize / 2) + (unitSize * jj), unitSize, unitSize);
            else
                for (int ii = 0; ii <= wallWidth / unitSize; ii++)
                    e.Graphics.DrawImage(WallImage, (-unitSize / 2) + unitSize * ii, -unitSize / 2, unitSize, unitSize);
        }

        /// <summary>
        /// Draws Beam object from where it was fired in the direction it was fired and for 2x the world size
        ///  (to make sure that it doesn't stop being drawn stop before its area of effect is over ).
        /// <para>
        /// Acts as a drawing delegate for DrawObjectWithTransform
        /// After performing the necessary transformation (translate/rotate)
        /// DrawObjectWithTransform will invoke this method. </para>
        /// </summary>
        /// <param name="o">The object to draw</param>
        /// <param name="e">The PaintEventArgs to access the graphics</param>
        private void BeamDrawer(object o, PaintEventArgs e)
        {
            Beam b = o as Beam;

            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            var beam = new Rectangle(0, 0, 4, -world.Size * 2);
            e.Graphics.FillEllipse(TransRedBrush, beam);
        }

        /// <summary>
        /// Draws the background of the world stretched to the size of the world.
        /// </summary>
        /// <param name="e"></param>
        private void DrawBackground(PaintEventArgs e)
        {
            e.Graphics.DrawImage(ArenaBackgroundImage, 0, 0, world.Size, world.Size);
        }
        #endregion

        /// <summary>
        /// Invoked when the DrawingPanel needs to be re-drawn. Draws all objects in the world based on their
        /// spatial relation to the user's tank.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPaint(PaintEventArgs e)
        {
            if (world.HasPlayerTank())
                lock (world)
                {
                    double playerX = world.GetTank(world.Player.Id).Location.X;
                    double playerY = world.GetTank(world.Player.Id).Location.Y;

                    // - calculate view/world size ratio
                    double ratio = (double)viewSize / (double)world.Size;
                    int halfSizeScaled = (int)(world.Size / 2.0 * ratio);

                    double inverseTranslateX = -WorldSpaceToImageSpace(world.Size, playerX) + halfSizeScaled;
                    double inverseTranslateY = -WorldSpaceToImageSpace(world.Size, playerY) + halfSizeScaled;

                    e.Graphics.TranslateTransform((float)inverseTranslateX, (float)inverseTranslateY);

                    // - Draw Background
                    DrawBackground(e);

                    // - Draw the walls
                    foreach (int wallId in world.GetWallIds())
                    {
                        var wall = world.GetWall(wallId);
                        DrawObjectWithTransform(e, wall, world.Size,
                                                ((wall.P1.X <= wall.P2.X) ? wall.P1.X : wall.P2.X),
                                                ((wall.P1.Y <= wall.P2.Y) ? wall.P1.Y : wall.P2.Y),
                                                0, WallDrawer);
                    }

                    // - Draw the tanks
                    foreach (int tankId in world.GetTankIds())
                    {
                        var tank = world.GetTank(tankId);
                        if (tank.Health == 0)
                            continue;
                        DrawObjectWithTransform(e, tank, world.Size, tank.Location.X, tank.Location.Y, tank.Direction.ToAngle(), TankDrawer);
                        DrawObjectWithTransform(e, tank, world.Size, tank.Location.X, tank.Location.Y, tank.TurretDirection.ToAngle(), TurretDrawer);
                        DrawObjectWithTransform(e, tank, world.Size, tank.Location.X, tank.Location.Y, 0, PlayerDrawer);
                    }

                    // - Draw the powerups
                    foreach (int powId in world.GetPowerupIds())
                    {
                        var pow = world.GetPowerup(powId);
                        DrawObjectWithTransform(e, pow, world.Size, pow.Location.X, pow.Location.Y, 0, PowerupDrawer);
                    }

                    // - Draw the projectiles
                    foreach (int projId in world.GetProjectileIds())
                    {
                        var proj = world.GetProjectile(projId);
                        DrawObjectWithTransform(e, proj, world.Size, proj.Location.X, proj.Location.Y, proj.Direction.ToAngle(), ProjectileDrawer);
                    }

                    // - Draw the projectiles
                    foreach (int beamId in world.GetBeamIds())
                    {
                        var beam = world.GetBeam(beamId);
                        DrawObjectWithTransform(e, beam, world.Size, beam.Origin.X, beam.Origin.Y, beam.Direction.ToAngle(), BeamDrawer);
                    }
                }

            // Do anything that Panel (from which this inherits) needs to do
            base.OnPaint(e);
        }


        /// <summary>
        /// Basically on overglorified struct,
        ///   grabs all of the appropriate image files for drawing a tank,
        ///   & its projectiles based up a string representing the color
        ///   (i.e. the color is part of the filename of the objects).
        /// </summary>
        private class ImageCollection
        {
            public Image Turret;
            public Image Body;
            public Image Projectile;

            /// <summary>
            /// Basically on overglorified struct,
            ///   grabs all of the appropriate image files for drawing a tank,
            ///   & its projectiles based up a string representing the color
            ///   (i.e. the color is part of the filename of the objects).
            /// </summary>
            /// <param name="color">Valid string representing the desired color option</param>
            public ImageCollection(string color)
            {
                Turret = Image.FromFile($"{imgDir}/{color}Turret.png");
                Body = Image.FromFile($"{imgDir}/{color}Tank.png");
                Projectile = Image.FromFile($"{imgDir}/{color}Shot.png");
            }

            /// <summary>
            /// Call dispose on all of the images in the Image collection.
            /// </summary>
            public void Dispose()
            {
                Turret.Dispose();
                Body.Dispose();
                Projectile.Dispose();
            }
        }
    }
}

