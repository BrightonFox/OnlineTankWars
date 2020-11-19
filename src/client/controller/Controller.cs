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

    public class Controller
    {
        private static readonly Regex MsgSplitPattern = new Regex(@"(?<=[\n])");

        private SocketState State;
        private bool movingPressed = false;
        private bool mousePressed = false;
        private string moveDir = "none";
        private string fireType = "none";
        private Vector2D targetDir;


        public event NetworkErrorOccuredHandler OnNetworkError;
        public event NetworkErrorOccuredHandler OnNetworkConnectionError;
        public event ServerUpdateHandler UpdateArrived;
        public event ServerUpdateHandler ServerConnectionMade;
        public event ServerUpdateHandler ServerDisconnected;


        public Player Player { get; private set; }   //! If there is a compile error this might be it!
        public World World { get; private set; }     //! If there is a compile error this might be it!


        public Controller()
        {
            this.UpdateArrived += this.ProcessInputs;
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
            World.ManageBeamLifeTimes(30);

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
        /// !! If we never draw anyting this is prob.s the culprit !!
        /// </summary>
        /// <param name="state"></param>
        private void OnInitMsgReceive(SocketState state)
        {
            if (state.ErrorOccured)
                OnNetworkConnectionError(state.ErrorMessage);

            var temp = state.GetData();
            var items = MsgSplitPattern.Split(temp);
            int ii;
            try
            {
                for (ii = 0; ii < items.Length - 1; ii++)
                {
                    if (items[ii].Length == 0) // is it empty?
                        continue;
                    else if (items[ii].Contains("{"))   // is it a json object that is not a wall?
                    {
                        state.OnNetworkAction = OnMsgReceive;
                        ParseJsonMsgs((new ArraySegment<string>(items, ii, items.Length - ii)).ToArray<string>(), state);
                        ServerConnectionMade();
                        Networking.GetData(state);
                        return;
                    }
                    else if (!Player.IdSet) // if not a json do we have the player id yet? (is it the player id?)
                        Player.Id = Int32.Parse(items[ii]);
                    else                    // we have player id it must be the world size !
                        World = new World(Int32.Parse(items[ii]), Player);

                    // Clear out the appropriate section of the data section
                    state.RemoveData(0, items[ii].Length);
                }

                // check last item to see if it is complete ----
                if (items[ii].Length > 0 && items[++ii][items[ii].Length - 1] == '\n')  // is it a wall?
                {
                    if (items[ii].Contains("{"))   // is it a json object that is not a wall?
                    {
                        World.ParseJsonString(items[ii]);
                    }
                    else if (!Player.IdSet)        // if not a json do we have the player id yet? (is it the player id?)
                        Player.Id = Int32.Parse(items[ii]/*.Substring(0, items[ii].Length - 2)*/);
                    else if (World == null)        // we have player id it must be the world size !
                        World = new World(Int32.Parse(items[ii]/*.Substring(0, items[ii].Length - 2)*/), Player);

                    // Clear out the appropriate section of the data section
                    state.RemoveData(0, items[ii].Length);
                }
                // check to see of we have received all walls and therefore finished init process
                if (World != null /*&& World.HasPlayerTank()*/)
                {
                    state.OnNetworkAction = OnMsgReceive;
                    ServerConnectionMade();
                }
            }
            catch (Exception ex)
            {
                OnNetworkConnectionError("ERROR: Failed to parse player ID or World size from Server message !!  [Controller.OnInitMsgReceive]\n" +
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
            lock (World)
            {
                for (ii = 0; ii < msgs.Length - 1; ii++)
                {
                    if (msgs[ii].Length == 0)
                        continue;

                    World.ParseJsonString(msgs[ii]);

                    state.RemoveData(0, msgs[ii].Length);
                }

                // verify that the last item is a complete json object and handle if so
                if (++ii < msgs.Length && msgs[ii].Length > 0 && msgs[ii][msgs[ii].Length - 1] == '\n')
                {
                    World.ParseJsonString(msgs[ii]);

                    state.RemoveData(0, msgs[ii].Length);
                }
            }
        }

        #endregion


        #region NetworkingSend
        /// <summary>
        /// Checks which inputs are currently requested.
        /// Sends it as a command to the server.
        /// </summary>
        private void ProcessInputs()
        {
            lock (Player)
            {
                if (movingPressed || mousePressed)
                {
                    var command = new Command(moveDir, fireType, targetDir);
                    Networking.Send(State.TheSocket, JsonConvert.SerializeObject(command) + '\n');
                }
                if (fireType == "alt")
                {
                    fireType = "none";
                    mousePressed = false;
                }
            }
        }

        /// <summary>
        /// Checks which inputs are currently requested.
        /// Sends it as a command to the server.
        /// </summary>
        public void DisconnectFromServer()
        {
            Command command;
            lock (Player)
            {
                command = new Command(moveDir, fireType, targetDir);
            }
            State.OnNetworkAction = (s) => { return; };
            Networking.SendAndClose(State.TheSocket, JsonConvert.SerializeObject(command) + '\n');
            ServerDisconnected();
            // Player = null;
            World = null;
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

        public void SetPlayerTurretDir(Vector2D targetPos)
        {
            targetPos.Normalize();
            lock (Player)
            {
                targetDir = targetPos;
            }
        }

        #endregion
    }
}
