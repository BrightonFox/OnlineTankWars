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

using TankWars.Server.Control;


namespace TankWars.Server
{
    class Program
    {
        static void Main(string[] args)
        {
            var ctrl = new Controller();
            ctrl.OnMessageLog += LogMessage;
            ctrl.OnNetworkError += LogErrorMessage;

            ctrl.StartServer();

            var running = true;
            while (running)
            {
                if (Console.ReadLine().ToLower() == "stop")
                {
                    ctrl.StopServer();
                    running = false;
                }
            }
        }

        private static void LogMessage(string msg)
        {
            Console.WriteLine(msg);
        }

        private static void LogErrorMessage(string msg)
        {
            Console.WriteLine("\n>>>>>>>>>>  !! ERROR !!\n" + msg);
        }
    }
}
