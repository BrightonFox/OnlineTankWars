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
 *   The primary GUI code for the tank wars game.
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;

using TankWars.Client.Control;
using TankWars.Client.Model;


namespace TankWars.Client.View
{

    /// <summary>
    /// The primary Gui Design for the Tank Wars Client
    ///  (Uses MVC design principals).
    /// </summary>
    public partial class MainForm : Form
    {
        // The Mothod invokers used to with the Invoke call to launch tasks on the main GUI thread.
        private MethodInvoker DoOnConnectInvoker;
        private MethodInvoker DoOnDisconnectInvoker;
        private MethodInvoker DoOnFrameInvoker;

        // Parameters defining the size of the interface.
        private const int viewSize = 900;
        private const int menuSize = 40;


        // The controller handles updates from the server
        // and notifies via an event
        private Controller controller;

        /// <summary>
        /// The primary Gui Design for the TaNk Wars Client
        ///  (Uses MVC design principals).
        /// </summary>
        /// <param name="ctrl">The controller for the Tank Wars game</param>
        public MainForm(Controller ctrl)
        {
            InitializeComponent();
            controller = ctrl;
            controller.UpdateArrived += OnFrame;
            DoOnFrameInvoker = () => this.Invalidate(true);
            controller.ServerConnectionMade += OnConnect;
            DoOnConnectInvoker = DoOnConnect;
            controller.ServerDisconnected += OnDisconnect;
            DoOnDisconnectInvoker = DoOnDisconnect;
            controller.OnNetworkConnectionError += DisplayErrorMsg;
            controller.OnNetworkError += DisplayErrorMsg;

            // - Set up the form. ------
            this.Text = "Tank Wars - Client (JustBroken)";
            serverAddressText.Text = "localhost"; //"tankwars.eng.utah.edu";
            nameText.MaxLength = 16;

            // - Assign listeners for all form items interactions
            controlsButton.Click += OnControlsBtn;
            aboutButton.Click += OnAboutBtn;
            connectButton.Click += OnBtnConnect;
            disconnectButton.Click += OnBtnDisconnect;
            serverAddressText.KeyDown += OnAddressSubmitted;
            this.FormClosing += OnMyFormClosing;

            // - Set up key and mouse handlers ----
            this.KeyDown += HandleKeyDown;
            this.KeyUp += HandleKeyUp;


        }


        /// <summary>
        /// Validates the player name and server address and begins the connection process to the server given
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnBtnConnect(object sender, EventArgs e)
        {
            // - Check and deal with playername and host ----
            var playerName = nameText.Text.Trim();
            var serverAddress = serverAddressText.Text.Trim();
            if (playerName == "" || playerName.Length < 3)
            {
                DisplayErrorMsg("Name must be between 3 & 16 characters!");
                return;
            }
            if (serverAddress == "")
            {
                DisplayErrorMsg("Please enter a server name!");
                return;
            }

            // - Connect to the server
            controller.ConnectToServer(serverAddress, playerName);
        }


        /// <summary>
        /// Private helper used to invoke DoOnConnect method on main thread
        /// </summary>
        private void OnConnect()
        {
            this.Invoke(this.DoOnConnectInvoker);
        }

        /// <summary>
        /// Sets initial values and properties for Form items.
        /// Creates a new Drawing Panel.
        /// Configures the drawing panel.
        /// Attaches the mouse handeling methods to the drawing panel.
        /// </summary>
        private void DoOnConnect()
        {
            // - Disable the form controls
            connectButton.Enabled = false;
            nameText.Enabled = false;
            serverAddressText.Enabled = false;
            disconnectButton.Enabled = true;
            // - Enable the global form to capture key presses
            this.KeyPreview = true;
            // - Place and add the drawing panel
            drawingPanel = new DrawingPanel(controller.World, viewSize);
            drawingPanel.Location = new Point(0, menuSize);
            drawingPanel.Size = new Size(viewSize, viewSize);
            this.Controls.Add(drawingPanel);

            // - Assign listeners for input events
            drawingPanel.MouseDown += HandleMouseDown;
            drawingPanel.MouseUp += HandleMouseUp;
            drawingPanel.MouseMove += HandleMouseMove;
            // controller.ServerDisconnected += drawingPanel.DisposeOfImages;
        }

        /// <summary>
        /// Properly ends the connection to the connected server
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnBtnDisconnect(object sender, EventArgs e)
        {
            controller.DisconnectFromServer();
        }

        /// <summary>
        /// Private helper used to invoke DoOnDisconnect method on main thread
        /// </summary>
        private void OnDisconnect()
        {
            this.Invoke(this.DoOnDisconnectInvoker);
        }

        /// <summary>
        /// Resets values of Form items to allow user to create a new connection.
        ///  Also removed the drawing panel from the view.
        /// </summary>
        private void DoOnDisconnect()
        {
            // - Disable the form controls
            connectButton.Enabled = true;
            nameText.Enabled = true;
            serverAddressText.Enabled = true;
            disconnectButton.Enabled = false;
            this.Controls.Remove(drawingPanel);
            // - Disable the global form to capture key presses
            this.KeyPreview = false;
        }

        /// <summary>
        /// Private helper to insure that the form closing is handled cleanly and used graphics are disposed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnMyFormClosing(object sender, EventArgs e)
        {
            DoOnDisconnect();
            drawingPanel?.DisposeOfImages();
        }


        /// <summary>
        /// Handler for the controller's UpdateArrived event
        /// </summary>
        private void OnFrame()
        {
            // Invalidate this form and all its children
            // This will cause the form to redraw as soon as it can
            this.Invoke(DoOnFrameInvoker);
        }

        /// <summary>
        /// Key down handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HandleKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.W:
                case Keys.Up:
                    controller.HandleMoveRequest("up");
                    break;
                case Keys.S:
                case Keys.Down:
                    controller.HandleMoveRequest("down");
                    break;
                case Keys.A:
                case Keys.Left:
                    controller.HandleMoveRequest("left");
                    break;
                case Keys.D:
                case Keys.Right:
                    controller.HandleMoveRequest("right");
                    break;
                case Keys.Space:
                    controller.HandleMouseRequest("main");
                    break;
                case Keys.E:
                    controller.HandleMouseRequest("alt");
                    break;
            }

            // Prevent other key handlers from running
            e.SuppressKeyPress = true;
            e.Handled = true;
        }


        /// <summary>
        /// Key up handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HandleKeyUp(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.W:
                case Keys.Up:
                    controller.CancelMoveRequest("up");
                    break;
                case Keys.S:
                case Keys.Down:
                    controller.CancelMoveRequest("down");
                    break;
                case Keys.A:
                case Keys.Left:
                    controller.CancelMoveRequest("left");
                    break;
                case Keys.D:
                case Keys.Right:
                    controller.CancelMoveRequest("right");
                    break;
                case Keys.Space:
                    controller.CancelMouseRequest("main");
                    break;
                case Keys.E:
                    controller.CancelMouseRequest("alt");
                    break;
                case Keys.Escape:
                    controller.DisconnectFromServer();
                    break;
            }
        }

        /// <summary>
        /// Handle mouse down
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HandleMouseDown(object sender, MouseEventArgs e)
        {
            switch (e.Button)
            {
                case MouseButtons.Left:
                    controller.HandleMouseRequest("main");
                    break;
                case MouseButtons.Right:
                    controller.HandleMouseRequest("alt");
                    break;
            }
        }

        /// <summary>
        /// Handle mouse up
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HandleMouseUp(object sender, MouseEventArgs e)
        {
            switch (e.Button)
            {
                case MouseButtons.Left:
                    controller.CancelMouseRequest("main");
                    break;
                case MouseButtons.Right:
                    controller.CancelMouseRequest("alt");
                    break;
            }
        }

        /// <summary>
        /// Handle mouse movement.
        /// Get the mouse position in terms of the drawing panel, offset to coordinates relative to player tank.
        /// Set the player turret position with a controller method.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HandleMouseMove(object sender, MouseEventArgs e)
        {
            var mousePos = drawingPanel.PointToClient(MousePosition);
            controller.SetPlayerTurretDir(new MathUtils.Vector2D(mousePos.X - (viewSize / 2),
                                                                  mousePos.Y - (viewSize / 2)));
        }

        /// <summary>
        /// Called to initiate connection to provided server when "Enter" key is pressed
        ///  (only when in the server address text box).
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnAddressSubmitted(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                OnBtnConnect(sender, e);
            // Prevent other key handlers from running
            e.SuppressKeyPress = true;
            e.Handled = true;
        }

        /// <summary>
        /// Creates and displays an error message box with the given <paramref name="msg"/> as its contents.
        ///  (attached to the various Error events in of the controller)
        /// </summary>
        /// <param name="msg"></param>
        private void DisplayErrorMsg(string msg)
        {
            MessageBox.Show(msg, "ERROR!!", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        /// <summary>
        /// Creates and displays a message box displaying the program's credit information
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnAboutBtn(object sender, EventArgs e)
        {
            MessageBox.Show(
                "Solution By:\n" +
                "  Team: JustBroken\n" +
                "    Members:\n" +
                "      Brighton Fox (u0981544)\n" +
                "      Andrew Osterhout (u1317172)\n" +
                "\n" +
                "Artwork By:\n" +
                "  Jolie Uk & Alex Smith\n" +
                "\n" +
                "Game Design By:\n" +
                "  Daniel Kopta\n" +
                "\n" +
                "Designed For:\n" +
                "  CS3500 Fall 2020, University of Utah.",
                "About: TankWars", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        /// Creates and displays a message box showing the uses of the different control inputs (key and mouse bindings)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnControlsBtn(object sender, EventArgs e)
        {
            MessageBox.Show(
                "W: \t \t Move up\n" +
                "A: \t \t Move left\n" +
                "S: \t \t Move down\n" +
                "D: \t \t Move right\n" +
                "E: \t \t Fire beam\n" +
                "Space bar: \t Fire projectile\n" +
                "Mouse Move: \t Aim\n" +
                "Left click: \t Fire projectile\n" +
                "Right click: \t Fire beam",
                "TankWars Controls", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

    }
}

