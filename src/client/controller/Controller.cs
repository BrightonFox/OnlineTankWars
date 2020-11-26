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
 *   + v1.0 - submittal - 2020/11/21
 *   
 * About:
 *   The controller for the TankWars game,
 *   It handles network communications with the server.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

using Newtonsoft.Json;

using TankWars.Client.Model;
using TankWars.NetworkUtil;
using TankWars.MathUtils;

namespace TankWars.Client.Control
{

    /// <summary>
    /// The delegate used for Network error events registered in the TankWars Controller
    /// </summary>
    /// <param name="msg">a string containing information about the error that occured</param>
    public delegate void NetworkErrorOccuredHandler(string msg);

    /// <summary>
    /// The delegate used for events registered in the TankWars Controller, 
    ///  that are triggered when various events having to do with the server occur.
    /// </summary>
    public delegate void ServerUpdateHandler();


    /// <summary>
    /// The Controller for the client version of the TankWars game 
    ///  (part of the MVC design scheme).
    /// </summary>
    public class Controller
    {
        /// <summary>
        /// A regular expression for segmenting the network messages,
        ///  which under teh Tank wars protocol are denoted with a `\n` character.
        /// </summary>
        private static readonly Regex MsgSplitPattern = new Regex(@"(?<=[\n])");

        private SocketState State;
        private bool mousePressed = false;
        private List<string> moveDir;
        private string fireType = "none";
        private Vector2D targetDir;

        /// <summary>
        /// Event triggered when network errors that DO NOT require you
        ///  to disconnect from the server / reconnect to the server to recover.
        /// <para>
        /// Responsible for communication with the TankWars Game server,
        ///  and for maintaining small amounts of client side logic for the game,
        ///  like managing how long beams should be displayed for, etc</para>
        /// </summary>
        public event NetworkErrorOccuredHandler OnNetworkError;

        /// <summary>
        /// Event triggered when network errors that DO require you
        ///  to disconnect from the server / reconnect to the server in order to recover.
        /// </summary>
        public event NetworkErrorOccuredHandler OnNetworkConnectionError;

        /// <summary>
        /// Event triggered after the client has received enough information from the server 
        ///  to constitute a frame.
        /// <para>!! The view should register any methods that need to be done once per frame to this event !!</para>
        /// Will not be triggered until after <see cref="ServerConnectionMade"/> is triggered.
        /// </summary>
        public event ServerUpdateHandler UpdateArrived;

        /// <summary>
        /// Event triggered after the client has successfully connected with the server,
        ///  and initial handshake info has been exchanged
        ///  (i.e. sent player name, and received player id, & world size).
        /// </summary>
        public event ServerUpdateHandler ServerConnectionMade;

        /// <summary>
        /// Event triggered after teh controller has started to send a final message to the server.
        /// After this no more messages will be received from the server,
        ///  and <see cref="UpdateArrived"/> will not be triggered again until,
        ///  <see cref="ServerConnectionMade"/> is triggered again.
        /// </summary>
        public event ServerUpdateHandler ServerDisconnected;


        /// <summary>
        /// Information representing the player.
        /// </summary>
        /// <value></value>
        public Player Player { get; private set; }

        /// <summary>
        /// The object that represents the world.
        /// </summary>
        public World World { get; private set; }



        /// <summary>
        /// The Controller for the client version of the TankWars game 
        ///  (part of the MVC design scheme).
        /// <para>
        /// Responsible for communication with the TankWars Game server,
        ///  and for maintaining small amounts of client side logic for the game,
        ///  like managing how long beams should be displayed for, etc</para>
        /// </summary>
        public Controller()
        {
            this.UpdateArrived += this.ProcessInputs;
            this.UpdateArrived += this.GameLoop;
            this.ServerConnectionMade += () => moveDir = new List<string>() {"none"};
        }


        #region GameLoop
        /// <summary>
        /// Logic that needs to be done every frame agnostic of the view inputs.
        /// </summary>
        private void GameLoop()
        {
            // Handle the life span of a beam
            World.ManageBeamLifeTimes(30);
        }

        #endregion


        #region NetworkConnect
        /// <summary>
        /// Connects to server.
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
        /// Retrieves player ID and world size from server initial messages,
        ///   will not be called after initial messages are received.
        /// <para>see <see cref="OnMsgReceive"/> for regular server communication processing.</para>
        /// </summary>
        /// <param name="state"></param>
        private void OnInitMsgReceive(SocketState state)
        {
            // - throw an error if the connection could not be setup
            if (state.ErrorOccured)
                OnNetworkConnectionError(state.ErrorMessage);

            var temp = state.GetData();
            var items = MsgSplitPattern.Split(temp);
            int ii;
            try
            {
                // - loop to differentiate the text received from the server
                for (ii = 0; ii < items.Length - 1; ii++)
                {
                    if (items[ii].Length == 0) // is it empty?
                        continue;
                    else if (items[ii].Contains("{"))   // is it a json object?
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
                if (items[ii].Length > 0 && items[++ii][items[ii].Length - 1] == '\n')
                {
                    if (items[ii].Contains("{"))   // is it a json object?
                    {
                        World.ParseJsonString(items[ii]);
                    }
                    else if (!Player.IdSet)        // if not a json do we have the player id yet? (is it the player id?)
                        Player.Id = Int32.Parse(items[ii]);
                    else if (World == null)        // we have player id it must be the world size !
                        World = new World(Int32.Parse(items[ii]), Player);

                    // Clear out the appropriate section of the data section
                    state.RemoveData(0, items[ii].Length);
                }
                // check to see of we have received all walls and therefore finished init process
                if (World != null)
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
        /// <para>see <see cref="OnInitMsgReceive"/> for the handling of the initial messages from the server.</para>
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
        /// Handles each json object contained in the passed <paramref name="msgs"/> string array and
        ///  removes them from the passed <paramref name="state"/> buffer.
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
        /// Checks which inputs are currently requested and
        ///  sends any as a command to the server.
        /// </summary>
        private void ProcessInputs()
        {
            lock (Player)
            {
                var command = new Command(moveDir[0], fireType, targetDir);
                Networking.Send(State.TheSocket, JsonConvert.SerializeObject(command) + '\n');
                
                if (fireType == "alt")
                {
                    fireType = "none";
                    mousePressed = false;
                }
            }
        }

        /// <summary>
        /// Sends a final command to the server to safely close the connection.
        /// </summary>
        public void DisconnectFromServer()
        {
            Command command;
            lock (Player)
            {
                command = new Command(moveDir[0], fireType, targetDir);
            }
            State.OnNetworkAction = (s) => { return; };
            Networking.SendAndClose(State.TheSocket, JsonConvert.SerializeObject(command) + '\n');

            // - do the things you need to after you disconnect from server
            //? i.e. make sure you're ready to connect to a server again without restarting the application.
            ServerDisconnected();
            World = null;
            mousePressed = false;
            moveDir = new List<string>() { "none" };
            fireType = "none";
        }
        #endregion


        #region MovementControls
        /// <summary>
        /// Updates the controller to show that the given direction is being help down.
        /// <para>
        /// !! DO NOT EVER GIVE "none" as <paramref name="direction"/> !!
        /// </para>
        /// </summary>
        /// <param name="direction">"up", "down", "left", "right" according to how the player tank should move</param>
        public void HandleMoveRequest(string direction)
        {
            lock (Player)
            {
                if (!moveDir.Contains(direction))
                    moveDir.Insert(0, direction);
            }
        }

        /// <summary>
        /// Updates the controller to show that the given direction is no longer being help down
        /// <para>
        /// !! DO NOT EVER GIVE "none" as <paramref name="direction"/> !!
        /// </para>
        /// </summary>
        /// <param name="direction">"up", "down", "left", or "right" according to how the player tank should move</param>
        public void CancelMoveRequest(string direction)
        {
            lock (Player)
            {
                if (moveDir.Contains(direction))
                    moveDir.Remove(direction);
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
                fireType = "none";
            }
        }

        /// <summary>
        /// Updates the controller to show that the mouse has been moved
        /// </summary>
        /// <param name="targetPos">The coordinates of the desired target for the turret</param>
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
