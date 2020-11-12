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
        
        Controller controller;

        public MainForm(Controller ctrl)
        {
            controller = ctrl;
        }

    }
}

