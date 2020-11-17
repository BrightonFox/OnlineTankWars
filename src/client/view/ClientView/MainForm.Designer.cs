using System.Windows.Forms;
using System.Drawing;

namespace TankWars.Client.View
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.SuspendLayout();
            // 
            // MainForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            // this.Name = "Tank Wars - Client (JustBroken)";
            this.ResumeLayout(false);

            // - Place and add the disconnect button
            disconnectButton = new Button();
            disconnectButton.Size = new Size(80, 20);
            disconnectButton.Location = new Point(viewSize-disconnectButton.Size.Width - 5, 5);
            disconnectButton.Text = "Disconnect";
            disconnectButton.Enabled = false;
            this.Controls.Add(disconnectButton);
            
            // - Place and add the connect button
            connectButton = new Button();
            connectButton.Size = new Size(70, 20);
            connectButton.Location = new Point(disconnectButton.Location.X - connectButton.Size.Width - 5, 5);
            connectButton.Text = "Connect";
            this.Controls.Add(connectButton);

            // - Place and add the name label
            nameLabel = new Label();
            nameLabel.Text = "Name:";
            nameLabel.Location = new Point(5, 10);
            nameLabel.Size = new Size(38, 15);
            this.Controls.Add(nameLabel);

            // - Place and add the name textbox
            nameText = new TextBox();
            nameText.Text = "player";
            nameText.Location = new Point(nameLabel.Location.X + nameLabel.Size.Width, 5);
            nameText.Size = new Size(70, 15);
            this.Controls.Add(nameText);

            // - Place and add the server address label
            serverAddressLabel = new Label();
            serverAddressLabel.Text = "Server Address:";
            serverAddressLabel.Location = new Point(nameText.Location.X + nameText.Size.Width + 10, 10);
            serverAddressLabel.Size = new Size(82, 15);
            this.Controls.Add(serverAddressLabel);

            // - Place and add the serverAddress textbox
            serverAddressText = new TextBox();
            serverAddressText.Text = "tankwars.eng.utah.edu";
            serverAddressText.Location = new Point(serverAddressLabel.Location.X + serverAddressLabel.Size.Width, 5);
            serverAddressText.Size = new Size(connectButton.Location.X - serverAddressText.Location.X - 10, 15);
            this.Controls.Add(serverAddressText);

            
        }

        private Button connectButton;
        private Button disconnectButton;
        private Label nameLabel;
        private Label serverAddressLabel;
        private TextBox nameText;
        private TextBox serverAddressText;
        private DrawingPanel drawingPanel;

        #endregion
    }
}

