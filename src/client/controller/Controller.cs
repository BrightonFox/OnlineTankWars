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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
// using Json;
using TankWars.Client.Model;
using TankWars.NetworkUtil;

namespace TankWars.Client.Control
{

    public delegate void NetworkErrorOccuredHandler(string msg);
    public delegate void ServerUpdateHandler();


    public class Controller
    {
        private readonly Regex InitMsgPattern = new Regex(@"^(\d+)\n(\d+)\n$");
        // readonly Regex ObjectMsgPattern = new Regex(@"\n");
        private string tempData = "";
        private bool movingPressed = false;
        private bool mousePressed = false;

        public event NetworkErrorOccuredHandler OnNetworkError;
        public event NetworkErrorOccuredHandler OnNetworkConnectionError;
        public event ServerUpdateHandler UpdateArrived;

        public Player player;
        public World world;


        public Controller()
        {
            this.UpdateArrived += this.ProcessInputs;
        }

        /// <summary>
        /// Connects to server.
        /// <para>
        /// !! IT IS RECOMMENDED TO RUN THIS IN A SEPARATE THREAD !!
        /// </para>
        /// </summary>
        /// <param name="addr"></param>
        /// <param name="name"></param>
        public void ConnectToServer(string addr, string name)
        {
            if (name.Length > 16)
                name = name.Substring(0, 16);
            player = new Player(name);
            Networking.ConnectToServer(OnConnect, addr, 11000);
        }

        /// <summary>
        /// Sends a single '\n' terminated string representing the player's name to the connection
        /// held in passed SocketState.
        /// </summary>
        /// <param name="state"></param>
        private void OnConnect(SocketState state)
        {
            if (state.ErrorOccured)
            {
                OnNetworkConnectionError(state.ErrorMessage);
                return;
            }
            Networking.Send(state.TheSocket, player.Name + "\n");
            state.OnNetworkAction = OnInitMsgReceive;
            Networking.GetData(state);
        }

        /// <summary>
        /// Retrieves player ID and world size from server initial messages
        /// </summary>
        /// <param name="state"></param>
        private void OnInitMsgReceive(SocketState state)
        {
            if (state.ErrorOccured)
                OnNetworkError(state.ErrorMessage);


            Match match = InitMsgPattern.Match(state.GetData());
            if (!match.Success)
            {
                Networking.GetData(state);
                return;
            }
            try
            {
                player.Id = Int32.Parse(match.Groups[1].Value);
                world = new World(Int32.Parse(match.Groups[2].Value));
            }
            catch (Exception)
            {
                OnNetworkError("ERROR: failed to parse player ID or World size from Server message !!  [Controller.OnMsgReceive]");
                state.ClearData();
                return;
            }
            state.ClearData();
            state.OnNetworkAction = OnMsgReceive;
        }


        /// <summary
        /// Retrieves messages from server representing state of all game objects
        /// to update client model world
        /// </summary>
        /// <param name="state"></param>
        private void OnMsgReceive(SocketState state)
        {
            if (state.ErrorOccured)
                OnNetworkError(state.ErrorMessage);
            // Match match = ObjectMsgPattern.Match(state.GetData());
            var items = Regex.Split(tempData + state.GetData(), @"\n");
            state.ClearData();

            try
            {
                int ii = 0;
                for (ii = 0; ii < items.Length - 1; ii++)
                {
                    ParseJsonString(items[ii]);
                }
                if (items[++ii] == "")
                    ParseJsonString(items[ii]);
                else
                    tempData = items[ii];

            }
            catch (Exception ex)
            {
                OnNetworkError("ERROR: Network Error !!  [Controller.OnWallReceive]" +
                                "\n    " + ex.Message);
                return;
            }
            UpdateArrived();
            Networking.GetData(state);
        }

        /// <summary>
        /// Parses passed json string and deserializes recognized objects to update client model world
        /// </summary>
        /// <param name="json"></param>
        private void ParseJsonString(string json)
        {
            JObject jObj = JObject.Parse(json);
            IList<string> keys = jObj.Properties().Select(p => p.Name).ToList();
            foreach (string key in keys)
            {
                switch (key)
                {
                    case "wall":
                        var wall = JsonConvert.DeserializeObject<Wall>(json);
                        world.Walls.Add(wall.Id, wall);
                        return;
                    case "tank":
                        var tank = JsonConvert.DeserializeObject<Tank>(json);
                        if (world.Tanks.ContainsKey(tank.Id))
                            if (tank.IsDead)
                                world.Tanks.Remove(tank.Id);
                            else
                                world.Tanks[tank.Id] = tank;
                        else if (!tank.IsDead)
                            world.Tanks.Add(tank.Id, tank);
                        return;
                    case "proj":
                        var proj = JsonConvert.DeserializeObject<Projectile>(json);
                        if (world.Projectiles.ContainsKey(proj.Id))
                            if (proj.IsDead)
                                world.Projectiles.Remove(proj.Id);
                            else
                                world.Projectiles[proj.Id] = proj;
                        else if (!proj.IsDead)
                            world.Projectiles.Add(proj.Id, proj);
                        return;
                    case "beam":
                        var beam = JsonConvert.DeserializeObject<Beam>(json);
                        world.Beams.Add(beam.Id, beam);
                        return;
                    case "power":
                        var powerup = JsonConvert.DeserializeObject<Powerup>(json);
                        if (world.Powerups.ContainsKey(powerup.Id))
                            if (powerup.IsDead)
                                world.Powerups.Remove(powerup.Id);
                            else
                                world.Powerups[powerup.Id] = powerup;
                        else if (!powerup.IsDead)
                            world.Powerups.Add(powerup.Id, powerup);
                        return;
                    default:
                        continue;
                }
            }
            throw new JsonException("ERROR: JSON not of Recognized type !!  [Controller.ParseJsonString]");
        }


        /// <summary>
        /// Checks which inputs are currently held down
        /// Normally this would send a message to the server
        /// </summary>
        private void ProcessInputs()
        {
            if (movingPressed)
                Console.WriteLine("moving");        //TODO: Handle Move Event
            if (mousePressed)
                Console.WriteLine("mouse pressed"); //TODO: Handle Fire Event
        }

        /// <summary>
        /// Example of handling movement request
        /// </summary>
        public void HandleMoveRequest(/* pass info about which command here */)
        {
            movingPressed = true;
        }

        /// <summary>
        /// Example of canceling a movement request
        /// </summary>
        public void CancelMoveRequest(/* pass info about which command here */)
        {
            movingPressed = false;
        }

        /// <summary>
        /// Example of handling mouse request
        /// </summary>
        public void HandleMouseRequest(/* pass info about which button here */)
        {
            mousePressed = true;
        }

        /// <summary>
        /// Example of canceling mouse request
        /// </summary>
        public void CancelMouseRequest(/* pass info about which button here */)
        {
            mousePressed = false;
        }
    }
}
