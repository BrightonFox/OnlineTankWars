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
using System.Linq;
using System.Collections.Generic;


namespace TankWars.Server.Model
{
    public partial class World : IWorld
    {       
        private Queue<CommandEntry> FrameCommands = new Queue<CommandEntry>();
        private HashSet<int> CommandIds = new HashSet<int>();

        public World(string fileDir) : base(2000)
        {
            MSPerFrame = 17;
        }


        public string GetFrame()
        {
            while (FrameCommands.Count > 0)
            {
                var entry = FrameCommands.Dequeue();
                //TODO: Process commands...
            }
            CommandIds.Clear();
            return "\n";
        }


        public void CreateNewPlayer(int id, string playerName)
        {
            Tanks.Add(id, new Tank());
        }

        public void RegisterCommand(int id, Command command)
        {
            if (CommandIds.Contains(id))
                return;
            lock (this)
            {
                CommandIds.Add(id);
                FrameCommands.Enqueue(new CommandEntry(id, command));
            }
        }

        public IEnumerable<Wall> GetWalls()
        {
            return Walls.Values.AsEnumerable() as IEnumerable<Wall>;
        }


        private struct CommandEntry
        {
            Command Command;
            int Id;

            public CommandEntry(int id, Command command)
            {
                Command = command;
                Id = id;
            }
        }
    }
}
