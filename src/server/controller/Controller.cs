using System.Threading;
using System.ComponentModel;
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
 *   + v1.0 - submittal - 2020/12/2
 *   
 * About:
 *   The controller for the TankWars server,
 *   It handles network communications with each client.
 */

using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Net.Sockets;

using TankWars.Server.Model;
using TankWars.NetworkUtil;


namespace TankWars.Server.Control
{

    /// <summary>
    /// The delegate used for information that should be displayed to the console, such as errors and networking events.
    /// </summary>
    /// <param name="msg">a string containing information about the event that occured</param>
    public delegate void LogEventHandler(string msg);

    /// <summary>
    /// The controller of a TankWars server. Main job
    /// is to handle network communications with all clients.
    /// </summary>
    public class Controller
    {

        private static readonly Regex MsgSplitPattern = new Regex(@"(?<=[\n])");
        private static readonly Regex InitMsgPattern = new Regex(@"^([^\n\r\t]{3,16})\n");
        private TcpListener tcpListener;
        private Dictionary<long, SocketState> Clients = new Dictionary<long, SocketState>();
        private World world;
        private bool running;
        private delegate bool SendHandler(Socket socket, string msg);

        private SendHandler Send;

        /// <summary>
        /// The port that this server uses.
        /// </summary>
        public int ServerPort {get; private set;}

        /// <summary>
        /// The file path to the settings file that this server uses.
        /// </summary>
        public string SettingsFilePath {get; private set;}

        /// <summary>
        /// This event is triggered on errors, 
        ///  with a message that the view should display how it sees fit to display error messages.
        /// </summary>
        public event LogEventHandler OnErrorLog;
        
        /// <summary>
        /// This event is triggered whenever the server wants to send a message 
        ///  that should be logged to the view.
        /// </summary>
        public event LogEventHandler OnMessageLog;

        /// <summary>
        /// Create the controller for a server application.
        /// Designed for the TankWars game.
        /// </summary>
        /// <param name="settingsFilePath">Optional file path to a settings xml file for congigring the server</param>
        /// <param name="port">Optional port number for the server to use. (note the v1 clients can't change their port so you shouldn't change it either.</param>
        public Controller(String settingsFilePath = "../../../../../../res/settings.xml", int port = 11000)
        {
            ServerPort = port;
            SettingsFilePath = settingsFilePath;
        }


        /// <summary>
        ///  Make sure everything that needs to be done to shut down the server is done.
        /// </summary>
        ~Controller()
        {
            StopServer();
        }


        /// <summary>
        /// Do all the things that should happen every frame.
        /// </summary>
        private void GameLoop()
        {
            Stopwatch watch = new Stopwatch();

            while (running)
            {
                watch.Start();
                while (watch.ElapsedMilliseconds < World.MSPerFrame)
                {/* DO NOTHING */}
                watch.Reset();

                if (Clients.Count > 0)
                    SendFrame();
            }
        }


        /// <summary>
        /// Sends a "frame" containing all objects in the world as
        /// JSON objects in strings to each client connected to the server. 
        /// </summary>
        private void SendFrame()
        {
            // long[] clientIds = new long[Clients.Count];
            // Clients.Keys.CopyTo(clientIds, 0);
            string frame;
            lock (world)
            {
                frame = world.NextFrame();
            }
            lock (Clients)
            {
                foreach (SocketState client in Clients.Values)
                    if (client.TheSocket.Connected)
                    {
                        if (!Networking.Send(client.TheSocket, frame))
                            ClientDisconnected(true, client, $"ERROR: Failed to Send frame to client (id: {client.ID}) !!  [Server.Controller.SendFrame]\n");
                    }
                    else
                        ClientDisconnected(false, client, $"Player (id: {client.ID}) has Disconencted");
            }
        }


        /// <summary>
        /// Do all the things that need to be done when a client disconnects from the server.
        /// !! Should always be called form within a lock on <see cref="Controller.Clients"/> !! 
        /// </summary>
        private void ClientDisconnected(bool error, SocketState client, string disconnectMsg)
        {
            Clients.Remove(client.ID);
            world.RemovePlayer((int)client.ID);
            if (error)
                OnErrorLog(disconnectMsg);
            else
                OnMessageLog(disconnectMsg);
        }


        /// <summary>
        /// Creates TCP listener for the server 
        ///  and starts the process of listening for connection attempts.
        /// On a connection attempt calls <see cref="OnClientConnection"/>
        ///   via <see cref="SocketState.OnNetworkAction"/> method.
        /// </summary>
        public void StartServer()
        {
            Send = Networking.Send;

            try
            {
                world = new World(SettingsFilePath);
            }
            catch (Exception ex)
            {
                OnErrorLog($"Error:    [Server.Controller.StartServer]\n {ex.Message}");
            }

            tcpListener = Networking.StartServer(OnClientConnection, ServerPort);

            running = true;
            OnMessageLog("Server started, awaiting clients...");
            new Thread(GameLoop).Start();
        }

        /// <summary>
        /// Receives socket state created when client connects,
        ///  and starts the Network handshake process. 
        /// </summary>
        private void OnClientConnection(SocketState state)
        {
            if (state.ErrorOccured)
            {
                OnErrorLog(state.ErrorMessage);
                if (state.TheSocket != null)
                    Networking.SendAndClose(state.TheSocket, state.ErrorMessage + "  [Server.Controller.OnClientConnection]");
            }
            else
            {
                state.OnNetworkAction = ClientSetup;
                Networking.GetData(state);
            }
        }

        /// <summary>
        /// Receives the clients player name and creates a new Tank in the world
        /// with the clients information. Changes the callback to listen for commands 
        /// from the client. Sends the world size and initial walls to the client.
        /// Adds socket to list of sockets to update on frame. Begin asking for commands.
        /// </summary>
        private void ClientSetup(SocketState state)
        {
            var temp = state.GetData();
            var match = InitMsgPattern.Match(temp);

            if (!match.Success)
            {
                // failed to parse palyer name from client !!
                Networking.SendAndClose(state.TheSocket, "ERROR: Failed Read Player Name !! \n");
                OnErrorLog($"ERROR: Failed Read Player Name '{temp.Replace('\n', ' ')}'  !!  [Server.Controller.ClientSetup]");
                return;
            }

            var playerName = match.Groups[1].Value;
            state.ClearData();

            if (!world.CreateNewPlayer((int)state.ID, playerName))
            {
                // failed to place new players tank disconencting from client !!
                Networking.SendAndClose(state.TheSocket, "ERROR: Failed to create tank !! \n");
                OnErrorLog($"ERROR: Failed to create tank for '{playerName}':{state.ID}  !!  [Server.Controller.ClientSetup]");
                return;
            }


            // - Send Player ID and WorldSize to Client ----
            Networking.Send(state.TheSocket, $"{state.ID}\n{world.Size}\n");

            // - Send Player the wall configuration ----
            foreach (string wall in world.GetWalls())
            {
                Networking.Send(state.TheSocket, wall);
            }

            // - Add client to list of active clients ----
            lock (Clients)
            {
                Clients.Add(state.ID, state);
            }

            OnMessageLog($"Player {state.ID}: '{playerName}' Connected successfully");  // log the successfull connection 

            // - Continue to regular networking receive loop ----
            state.OnNetworkAction = ReceiveClientCommand;
            Networking.GetData(state);
        }

        /// <summary>
        /// Closes server by stoping the TCPListener from accepting requests,
        ///  and sending final messages to the clients.
        /// </summary>
        public void StopServer()
        {
            if (tcpListener == null)
                return;

            Networking.StopServer(tcpListener);

            running = false;
            Send = Networking.SendAndClose;
            SendFrame();
        }

        /// <summary>
        /// Receives command objects from the player to interpret
        /// their desired action on a frame.
        /// </summary>
        private void ReceiveClientCommand(SocketState state)
        {
            if (state.ErrorOccured && state.TheSocket.Connected)
                OnErrorLog("ERROR:    [Server.Controller.ReceiveClientCommand]\n    " + state.ErrorMessage);
            var temp = state.GetData();
            var msgs = MsgSplitPattern.Split(temp);
            try
            {
                if (msgs.Length >= 1 && msgs[0].Length > 1 && msgs[0][msgs[0].Length - 1] == '\n')      // verify that a complete command has been received
                {
                    world.RegisterCommand((int)state.ID, msgs[0]);
                    for (int ii = 0; ii < msgs.Length; ii++)
                        if (msgs[ii].Length >= 1 && msgs[ii][msgs[ii].Length - 1] == '\n')      // clear out received command and any other
                            state.RemoveData(0, msgs[ii].Length);                               // complete commands received on this same frame
                }
            }
            catch (Exception ex)
            {
                OnErrorLog("ERROR: " + ex.Message + "  [Server.Controller.ReceiveClientCommand]");
            }
            Networking.GetData(state);
        }
    }
}

