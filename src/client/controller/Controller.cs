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
using System.Windows.Input;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using TankWars.Client.Model;
using TankWars.NetworkUtil;
using TankWars.MathUtils;

namespace TankWars.Client.Control
{

    public delegate void NetworkErrorOccuredHandler(string msg);
    public delegate void ServerUpdateHandler();
    public delegate void GetTargetPosHandler(out Vector2D targetPos);
    // public delegate Vector2D GetTargetPosHandler();

    public class Controller
    {
        private static readonly Regex MsgSplitPattern = new Regex(@"(?<=[\n])");

        private SocketState State;
        private bool movingPressed = false;
        private bool mousePressed = false;
        private string moveDir;
        private string fireType;
        private Vector2D fireDir;


        public event NetworkErrorOccuredHandler OnNetworkError;
        public event NetworkErrorOccuredHandler OnNetworkConnectionError;
        public event ServerUpdateHandler UpdateArrived;
        public event GetTargetPosHandler GetTargetPos;


        public Player Player;   //! If there is a compile error this might be it!
        public World World;     //! If there is a compile error this might be it!


        public Controller()
        {
            this.UpdateArrived += this.SendCommand;
            this.UpdateArrived += this.GameLoop;
            //TODO: whatever else needs to be done here...
        }


        #region GameLoop
        /// <summary>
        /// Logic that needs to be done every frame agnostic of the view.
        /// </summary>
        private void GameLoop()
        {
            // Handle the life span of a beam ----
            lock (World.Beams)
            {
                foreach (Beam beam in World.Beams.Values)
                    if (beam.LifeSpan > 30)
                        World.Beams.Remove(beam.Id);
            }

            //TODO: handle the rest of the logic that should be void of the view...
        }

        #endregion


        #region NetworkConnect
        /// <summary>
        /// Connects to server.
        /// <para>
        /// !! IT IS RECOMMENDED TO RUN THIS IN A SEPARATE THREAD FROM THE VIEWS MAIN THREAD !!
        /// </para>
        /// </summary>
        /// <param name="addr">An ipv4 address or domain name for the server</param>
        /// <param name="name">The name of the player, max 16 characters</param>
        public void ConnectToServer(string addr, string name)
        {
            if (name.Length > 16)
                name = name.Substring(0, 16);
            Player = new Player(name);
            Networking.ConnectToServer(OnConnect, addr, 11000);
        }

        /// <summary>
        /// Sends a single '\n' terminated string representing the player's name to the connection
        /// held in passed SocketState.
        /// </summary>
        /// <param name="state"></param>
        private void OnConnect(SocketState state)
        {
            State = state;
            if (state.ErrorOccured)
            {
                OnNetworkConnectionError(state.ErrorMessage);
                return;
            }
            Networking.Send(state.TheSocket, Player.Name + "\n");
            state.OnNetworkAction = OnInitMsgReceive;
            Networking.GetData(state);
        }

        /// <summary>
        /// Retrieves player ID and world size from server initial messages
        /// !! If we never draw anyting this is prob.s the cultrate !!
        /// /// </summary>
        /// <param name="state"></param>
        private void OnInitMsgReceive(SocketState state)
        {
            if (state.ErrorOccured)
                OnNetworkError(state.ErrorMessage);


            var items = MsgSplitPattern.Split(state.GetData());
            try
            {
                int ii;
                for (ii = 0; ii < items.Length - 1; ii++)
                {
                    if (items[ii].Length == 0) // is it empty?
                        continue;
                    else if (items[ii].Contains("{"))   // is it a json object that is not a wall?
                    {
                        state.OnNetworkAction = OnMsgReceive;
                        ParseJsonMsgs((new ArraySegment<string>(items, ii, items.Length)).ToArray<string>(), state);
                    }
                    else if (!Player.IdSet) // if not a json do we have the player id yet? (is it the player id?)
                        Player.Id = Int32.Parse(items[ii].Substring(0, items[ii].Length - 2));
                    else                    // we have player id it must be the world size !
                        World = new World(Int32.Parse(items[ii].Substring(0, items[ii].Length - 2)));

                    // Clear out the appropriate section of the data section
                    state.RemoveData(0, items[ii].Length);
                }

                // check last item to see if it is complete ----
                if (items[++ii][items[ii].Length - 1] == '\n')  // is it a wall?
                {
                    if (items[ii].Contains("{"))   // is it a json object that is not a wall?
                    {
                        state.OnNetworkAction = OnMsgReceive;
                        ParseJsonString(items[ii]);
                    }
                    else if (!Player.IdSet)        // if not a json do we have the player id yet? (is it the player id?)
                        Player.Id = Int32.Parse(items[ii].Substring(0, items[ii].Length - 2));
                    else if (World == null)        // we have player id it must be the world size !
                    {
                        World = new World(Int32.Parse(items[ii].Substring(0, items[ii].Length - 2)));
                        state.OnNetworkAction = OnMsgReceive;
                    }                    

                    // Clear out the appropriate section of the data section
                    state.RemoveData(0, items[ii].Length);
                }
                // check to see of we hae received all walls and therefore finished init process
                if (World?.Tanks.Count > 0)
                    state.OnNetworkAction = OnMsgReceive;
            }
            catch (Exception ex)
            {
                OnNetworkError("ERROR: failed to parse player ID or World size from Server message !!  [Controller.OnMsgReceive]\n" +
                                "   " + ex.Message);
                state.ClearData();
                return;
            }
            Networking.GetData(state);
        }

        #endregion


        #region NetworkReceive
        /// <summary>
        /// Retrieves messages from server representing state of all game objects,
        ///   to update client model world.
        /// </summary>
        /// <param name="state"></param>
        private void OnMsgReceive(SocketState state)
        {
            if (state.ErrorOccured)
                OnNetworkError(state.ErrorMessage);

            var items = MsgSplitPattern.Split(state.GetData());
            try
            {
                ParseJsonMsgs(items, state);
            }
            catch (Exception ex)
            {
                OnNetworkError("ERROR: Network Error !!  [Controller.OnMsgReceive]" +
                                "\n    " + ex.Message);
                return;
            }
            UpdateArrived();
            Networking.GetData(state);
        }


        /// <summary>
        /// Handles each json object contained in the passed <paramref name="msgs"/> string array.
        /// </summary>
        /// <param name="msgs"></param>
        /// <param name="state"></param>
        private void ParseJsonMsgs(string[] msgs, SocketState state)
        {
            int ii;
            for (ii = 0; ii < msgs.Length - 1; ii++)
            {
                if (msgs[ii].Length == 0)
                    continue;

                ParseJsonString(msgs[ii]);

                state.RemoveData(0, msgs[ii].Length);
            }

            // verify that the last item is a complete json object and handle if so
            if (msgs[++ii][msgs[ii].Length - 1] == '\n')
            {
                ParseJsonString(msgs[ii]);

                state.RemoveData(0, msgs[ii].Length);
            }
        }


        /// <summary>
        /// Parses passed json string into the appropriate type,
        ///   and deserializes recognized objects to update client model world.
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
                    case "tank":
                        var tank = jObj.ToObject<Tank>();
                        lock (World.Tanks)
                        {
                            if (World.Tanks.ContainsKey(tank.Id))
                                if (tank.IsDisconnected)
                                    World.Tanks.Remove(tank.Id);
                                else
                                    World.Tanks[tank.Id] = tank;
                            else if (!tank.IsDisconnected)
                                World.Tanks.Add(tank.Id, tank);
                        }
                        return;
                    case "proj":
                        var proj = jObj.ToObject<Projectile>();
                        lock (World.Projectiles)
                        {
                            if (World.Projectiles.ContainsKey(proj.Id))
                                if (proj.IsDead)
                                    World.Projectiles.Remove(proj.Id);
                                else
                                    World.Projectiles[proj.Id] = proj;
                            else if (!proj.IsDead)
                                World.Projectiles.Add(proj.Id, proj);
                            return;
                        }
                    case "beam":
                        var beam = jObj.ToObject<Beam>();
                        lock (World.Beams)
                        {
                            World.Beams.Add(beam.Id, beam);
                        }
                        return;
                    case "power":
                        var powerup = jObj.ToObject<Powerup>();
                        lock (World.Powerups)
                        {
                            if (World.Powerups.ContainsKey(powerup.Id))
                                if (powerup.IsDead)
                                    World.Powerups.Remove(powerup.Id);
                                else
                                    World.Powerups[powerup.Id] = powerup;
                            else if (!powerup.IsDead)
                                World.Powerups.Add(powerup.Id, powerup);
                            return;
                        }
                    case "wall":
                        var wall = jObj.ToObject<Wall>();
                        World.Walls.Add(wall.Id, wall);
                        return;
                    default:
                        continue;
                }
            }
            throw new JsonException("ERROR: JSON not of Recognized type !!  [Controller.ParseJsonString]");
        }
        #endregion


        #region NetworkingSend
        /// <summary>
        /// Checks which inputs are currently requested.
        /// Sends it as a command to the server.
        /// </summary>
        private void SendCommand()
        {
            Command command;
            lock (Player)
            {
                if (mousePressed && movingPressed)
                {
                    command = new Command(moveDir, fireType, fireDir);
                    Networking.Send(State.TheSocket, JsonConvert.SerializeObject(command) + '\n');
                }
                else if (movingPressed)
                {
                    command = new Command(moveDir, fireType, GetPlayerTurretDir());
                    Networking.Send(State.TheSocket, JsonConvert.SerializeObject(command) + '\n');
                }
            }
        }
        #endregion


        #region MovementControls
        /// <summary>
        /// Updates the controller to show that the given direction is being help down
        /// </summary>
        /// <param name="direction">"up", "down", "left", "right" according to how the player tank should move</param>
        public void HandleMoveRequest(string direction)
        {
            lock (Player)
            {
                movingPressed = true;
                moveDir = direction;
            }
        }

        /// <summary>
        /// Updates the controller to show that the given direction is no longer being help down
        /// </summary>
        /// <param name="direction">"up", "down", "left", or "right" according to how the player tank should move</param>
        public void CancelMoveRequest(string direction)
        {
            lock (Player)
            {
                if (moveDir != direction)
                    return;
                movingPressed = false;
                moveDir = "none";
            }
        }

        /// <summary>
        /// Updates controller to show the mouse is being held down
        /// </summary>
        /// <param name="direction">"main" or "alt" depending on regular fire or beam fire</param>
        public void HandleMouseRequest(string fire)
        {
            lock (Player)
            {
                mousePressed = true;
                fireType = fire;
                fireDir = GetPlayerTurretDir();
            }
        }

        /// <summary>
        /// Updates the controller to show that the mouse is no longer being help down
        /// </summary>
        /// <param name="direction">"main" or "alt" depending on regular fire or beam fire</param>
        public void CancelMouseRequest(string fire)
        {
            lock (Player)
            {
                if (fireType != fire)
                    return;
                mousePressed = false;
                fireType = "none";
            }
        }

        /// <summary>P
        /// Trigger event to get get the curser position in world coordinates from the view.
        /// Then calculate the vector between the player tank position and the target position.
        /// Normalize it and return it.
        /// </summary>
        /// <returns>NOrmalized vector representing the direction the player tank is aiming in</returns>
        private Vector2D GetPlayerTurretDir()
        {
            GetTargetPos(out Vector2D targetPos);
            var dir = targetPos - ((Tank)World.Tanks[Player.Id]).Location;
            // var dir = GetTargetPos() - ((Tank)world.Tanks[player.Id]).Location;
            dir.Normalize();
            return fireDir;
        }

        #endregion
    }
}
