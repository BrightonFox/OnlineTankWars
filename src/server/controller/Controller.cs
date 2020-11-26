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
 *   + <>
 * 
 * About:
 *   <>
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
    /// The delegate used for Network error events registered in the TankWars Controller
    /// </summary>
    /// <param name="msg">a string containing information about the error that occured</param>
    public delegate void NetworkErrorOccuredHandler(string msg);


    public delegate void MessageLogHandler(string msg);


    public class Controller
    {

        private static readonly Regex MsgSplitPattern = new Regex(@"(?<=[\n])");
        private TcpListener tcpListener;
        private Dictionary<long, SocketState> Clients = new Dictionary<long, SocketState>();
        private World world;
        private bool running;
        private delegate bool SendHandler(Socket socket, string msg);

        private SendHandler Send;


        public event NetworkErrorOccuredHandler OnNetworkError;
        public event MessageLogHandler OnMessageLog;


        public Controller()
        {
        }


        /// <summary>
        ///  Make sure everything that needs to be done to shut down the server is done.
        /// </summary>
        ~Controller()
        {
            StopServer();
        }


        /// <summary>
        /// 
        /// </summary>
        private void GameLoop()
        {
            Stopwatch watch = new Stopwatch();
            while (running)
            {
                while (watch.ElapsedMilliseconds < world.MSPerFrame)
                {/* DO NOTHING */}
                watch.Reset();

                if (Clients.Count > 0)
                    SendFrame();
            }
        }


        /// <summary>
        /// 
        /// </summary>
        private void SendFrame()
        {
            long[] clientIds = new long[Clients.Count];
            Clients.Keys.CopyTo(clientIds, 0);
            string frame;
            lock (world)
            {
                frame = world.NextFrame();
            }
            foreach (long id in clientIds)
            {
                if (Clients[id].TheSocket.Connected)
                    Networking.Send(Clients[id].TheSocket, frame);
                else
                    lock (Clients)
                    {
                        Clients.Remove(id);
                    }
            }
        }


        /// <summary>
        /// Creates TCP listener for the server 
        ///  and starts the process of listening for connection attempts.
        /// On a connection attempt calls <see cref="OnClientConnection"/>
        ///   via <see cref="SocketState.OnNetworkAction"/> method.
        /// </summary>
        public void StartServer(String settingsFileDir="TODO: Figure out where the settings file will be...")
        {
            Send = Networking.Send;

            world = new World(settingsFileDir);

            tcpListener = Networking.StartServer(OnClientConnection, 11000);

            running = true;
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
                OnNetworkError(state.ErrorMessage);
                if (state.TheSocket != null)
                    Networking.SendAndClose(state.TheSocket, " \n");
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
            if (!world.CreateNewPlayer((int)state.ID, state.GetData()))
                // failed to place new players tank disconencting from client !!
                Networking.SendAndClose(state.TheSocket, "ERROR: Failed to create tank !! \n");

            Networking.Send(state.TheSocket, $"{state.ID}\n{world.Size}\n");

            foreach (Wall wall in world.GetWalls())
            {
                Networking.Send(state.TheSocket, wall.ToString());
            }

            lock (Clients)
            {
                Clients.Add(state.ID, state);
            }

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
            if (state.ErrorOccured)
                OnNetworkError(state.ErrorMessage);
            var temp = state.GetData();
            var msgs = MsgSplitPattern.Split(temp);
            try
            {
                if (msgs.Length >= 1 && msgs[0][msgs[0].Length - 1] == '\n')
                {
                    world.RegisterCommand((int)state.ID, msgs[0]);
                    state.RemoveData(0, msgs[0].Length);
                    for (int ii = 1; ii < msgs.Length; ii++)
                        if (msgs.Length > 1 && msgs[0][msgs[0].Length - 1] != '\n')
                            state.RemoveData(0, msgs[ii].Length);
                }
            }
            catch (Exception ex)
            {
                OnNetworkError("ERROR: " + ex.Message);
            }
            Networking.GetData(state);
        }
    }

    
}
