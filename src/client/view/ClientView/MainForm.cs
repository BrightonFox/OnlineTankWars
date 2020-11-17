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
        private const int viewSize = 500;
        private const int menuSize = 40;

        // The controller handles updates from the "server"
        // and notifies us via an event
        private Controller controller;

        // World is a simple container for Players and Powerups
        // The controller owns the World, but we have a reference to it
        private World World;

        public MainForm(Controller ctrl)
        {
            InitializeComponent();
            controller = ctrl;
            World = controller.World;
            controller.UpdateArrived += OnFrame;

            // - Set up the form. ------
            

            // - Set the window size --
            this.ClientSize = new Size(viewSize, viewSize + menuSize);

            /* Provided GUI Code, manuals generated...      //TODO: Uncomment after figureing out GUI design
            // Place and add the button
            startButton = new Button();
            startButton.Location = new Point(215, 5);
            startButton.Size = new Size(70, 20);
            startButton.Text = "Start";
            startButton.Click += StartClick;
            this.Controls.Add(startButton);

            // Place and add the name label
            nameLabel = new Label();
            nameLabel.Text = "Name:";
            nameLabel.Location = new Point(5, 10);
            nameLabel.Size = new Size(40, 15);
            this.Controls.Add(nameLabel);

            // Place and add the name textbox
            nameText = new TextBox();
            nameText.Text = "player";
            nameText.Location = new Point(50, 5);
            nameText.Size = new Size(70, 15);
            this.Controls.Add(nameText);

            // Place and add the drawing panel
            drawingPanel = new DrawingPanel(World);
            drawingPanel.Location = new Point(0, menuSize);
            drawingPanel.Size = new Size(viewSize, viewSize);
            this.Controls.Add(drawingPanel);
            */

            // - Set up key and mouse handlers ----
            this.KeyDown += HandleKeyDown;
            this.KeyUp += HandleKeyUp;
            // drawingPanel.MouseDown += HandleMouseDown;       //TODO: Uncomment this after creating the GUI elements
            // drawingPanel.MouseUp += HandleMouseUp;           //TODO: Uncomment this after creating the GUI elements
        }


        /// <summary>
        /// Simulates connecting to a "server"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StartClick(object sender, EventArgs e)
        {
            // - Disable the form controls
            // startButton.Enabled = false;                     //TODO: Uncomment this after creating the GUI elements
            // nameText.Enabled = false;
            // - Enable the global form to capture key presses
            KeyPreview = true;
            // - "connect" to the "server"
            // controller.ProcessUpdatesFromServer();           //TODO: Update this line to match our controller
        }

        /// <summary>
        /// Handler for the controller's UpdateArrived event
        /// </summary>
        private void OnFrame()
        {
            // Invalidate this form and all its children
            // This will cause the form to redraw as soon as it can
            var invalidator = new MethodInvoker(() => this.Invalidate(true));
            this.Invoke(invalidator);
        }

        /// <summary>
        /// Key down handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HandleKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
                Application.Exit();
            // if (e.KeyCode == Keys.W)
                // controller.HandleMoveRequest();      //TODO: Update this line to match our controller

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
            // if (e.KeyCode == Keys.W)
                // controller.CancelMoveRequest();      //TODO: Update this line to match our controller
        }

        /// <summary>
        /// Handle mouse down
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HandleMouseDown(object sender, MouseEventArgs e)
        {
            // if (e.Button == MouseButtons.Left)
                // controller.HandleMouseRequest();         //TODO: Update this line to match our controller
        }

        /// <summary>
        /// Handle mouse up
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HandleMouseUp(object sender, MouseEventArgs e)
        {
            // if (e.Button == MouseButtons.Left)
                // controller.CancelMouseRequest();     //TODO: Update this line to match our controller
        }

    }
}

