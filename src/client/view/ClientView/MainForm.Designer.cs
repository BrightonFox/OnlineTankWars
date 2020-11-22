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
            // - Set the window size --
            this.Icon = new Icon("../../../../../../res/img/TankWarsIcon.ico");
            this.ClientSize = new Size(viewSize, menuSize+viewSize);
            // this.BackColor = Color.FromArgb(32, 64, 64);
            // this.ForeColor = Color.LightGreen;
            this.Font = new Font("Consolas", 8);
            var emojiFont = new Font("Segoe UI Emoji", 14);

            // - Place and add the help button
            aboutButton = new Label();
            aboutButton.Size = new Size(20, 20);
            aboutButton.Location = new Point(viewSize-aboutButton.Size.Width - 7, 5);
            aboutButton.Font = emojiFont;
            aboutButton.Text = "\uD83D\uDEC8";
            // aboutButton.FlatStyle = FlatStyle.Flat;
            this.Controls.Add(aboutButton);

            // - Place and add the controls button
            controlsButton = new Label();
            controlsButton.Size = new Size(24, 20);
            controlsButton.Location = new Point(aboutButton.Location.X - controlsButton.Size.Width - 4, 4);
            controlsButton.Font = emojiFont;
            controlsButton.Text = "\uD83D\uDDAE";
            this.Controls.Add(controlsButton);

            // - Place and add the disconnect button
            disconnectButton = new Button();
            disconnectButton.Size = new Size(80, 20);
            disconnectButton.Location = new Point(controlsButton.Location.X - disconnectButton.Size.Width - 22, 8);
            disconnectButton.Text = "Disconnect";
            disconnectButton.Enabled = false;
            disconnectButton.FlatStyle = FlatStyle.Flat;
            this.Controls.Add(disconnectButton);
            
            // - Place and add the connect button
            connectButton = new Button();
            connectButton.Size = new Size(70, 20);
            connectButton.Location = new Point(disconnectButton.Location.X - connectButton.Size.Width - 5, 8);
            connectButton.Text = "Connect";
            connectButton.FlatStyle = FlatStyle.Flat;
            this.Controls.Add(connectButton);

            // - Place and add the name label
            nameLabel = new Label();
            nameLabel.Text = "Name:";
            nameLabel.Location = new Point(5, 12);
            nameLabel.Size = new Size(38, 15);
            this.Controls.Add(nameLabel);

            // - Place and add the name textbox
            nameText = new TextBox();
            nameText.Text = "player";
            nameText.Location = new Point(nameLabel.Location.X + nameLabel.Size.Width+1, 8);
            nameText.Size = new Size(70, 15);
            nameText.BorderStyle = BorderStyle.FixedSingle;
            this.Controls.Add(nameText);

            // - Place and add the server address label
            serverAddressLabel = new Label();
            serverAddressLabel.Text = "Server Address:";
            serverAddressLabel.Location = new Point(nameText.Location.X + nameText.Size.Width + 10, 12);
            serverAddressLabel.Size = new Size(100, 15);
            this.Controls.Add(serverAddressLabel);

            // - Place and add the serverAddress textbox
            serverAddressText = new TextBox();
            serverAddressText.Enabled = true;
            serverAddressText.Location = new Point(serverAddressLabel.Location.X + serverAddressLabel.Size.Width+1, 8);
            serverAddressText.Size = new Size(connectButton.Location.X - serverAddressText.Location.X - 10, 15);
            this.Controls.Add(serverAddressText);

            
        }

        private Button connectButton;
        private Button disconnectButton;
        private Label aboutButton;
        private Label controlsButton;
        private Label nameLabel;
        private Label serverAddressLabel;
        private TextBox nameText;
        private TextBox serverAddressText;
        private DrawingPanel drawingPanel;

        #endregion
    }
}

