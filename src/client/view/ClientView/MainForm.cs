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
    public partial class MainForm : Form
    {   
        public MethodInvoker DoOnConnectInvoker;
        public MethodInvoker DoOnDisconnectInvoker;
        public MethodInvoker DoOnFrameInvoker;
        private const int viewSize = 900;
        private const int menuSize = 40;

        private MathUtils.Vector2D mousePos;

        // The controller handles updates from the "server"
        // and notifies via an event
        private Controller controller;

        // World is a container for the game objects
        private World world;

        public MainForm(Controller ctrl)
        {
            InitializeComponent();
            controller = ctrl;
            // world = controller.World;
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
            
            connectButton.Click += OnBtnConnect;
            disconnectButton.Click += OnBtnDisconnect;
            serverAddressText.KeyDown += OnAddressSubmitted;
            this.FormClosing += OnMyFormClosing;

            // - Set the window size --
            this.ClientSize = new Size(viewSize, viewSize + menuSize);

            // - Set up key and mouse handlers ----
            this.KeyDown += HandleKeyDown;
            this.KeyUp += HandleKeyUp;
            
            
        }


        /// <summary>
        /// Simulates connecting to a "server"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnBtnConnect(object sender, EventArgs e)
        {
            // - Disable the form controls
            connectButton.Enabled = false;
            nameText.Enabled = false;
            serverAddressText.Enabled = false;
            disconnectButton.Enabled = true;
            // - Enable the global form to capture key presses
            this.KeyPreview = true;
            // - "connect" to the "server"
            controller.ConnectToServer(serverAddressText.Text, nameText.Text);
        }

        private void OnConnect()
        {
            this.Invoke(this.DoOnConnectInvoker);
        }


        private void DoOnConnect()
        {
            // - Place and add the drawing panel
            drawingPanel = new DrawingPanel(controller.World, viewSize);
            drawingPanel.Location = new Point(0, menuSize);
            drawingPanel.Size = new Size(viewSize, viewSize);
            this.Controls.Add(drawingPanel);

            drawingPanel.MouseDown += HandleMouseDown;
            drawingPanel.MouseUp += HandleMouseUp;
            drawingPanel.MouseMove += HandleMouseMove;
            // controller.ServerDisconnected += drawingPanel.DisposeOfImages;
        }


        private void OnBtnDisconnect(object sender, EventArgs e)
        {
            controller.DisconnectFromServer();
        }

        private void OnDisconnect()
        {
            this.Invoke(this.DoOnDisconnectInvoker);
        }

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

        private void OnMyFormClosing(object sender, EventArgs e)
        {
            DoOnDisconnect();
            drawingPanel.DisposeOfImages();
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
                case Keys.Enter:
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
                case Keys.Enter:
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

        private void HandleMouseMove(object sender, MouseEventArgs e)
        {
           controller.SetPlayerTurretDir(drawingPanel.GetTargetPos());
        }


        private void OnAddressSubmitted(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                OnBtnConnect(sender, e);
            // Prevent other key handlers from running
            e.SuppressKeyPress = true;
            e.Handled = true;
        }


        private void DisplayErrorMsg(string msg)
        {
            MessageBox.Show(msg, "ERROR!!", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

    }
}

