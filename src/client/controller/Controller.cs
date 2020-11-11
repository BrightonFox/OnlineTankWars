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

using TankWars.Client.Model;

namespace TankWars.Client.Control
{
    public class Controller
    {
        // World is a simple container for Players and Powerups
    private World theWorld;

    public const int worldSize = 500;

    private bool movingPressed = false;
    private bool mousePressed = false;

    // Although we aren't actually doing any networking,
    // this FakeServer object will simulate game updates coming 
    // from an asynchronous server.
    private FakeServer server;

    public delegate void ServerUpdateHandler();

    public event ServerUpdateHandler UpdateArrived;

    public Controller()
    {
      theWorld = new World(worldSize);
      server = new FakeServer(worldSize);
    }

    public World GetWorld()
    {
      return theWorld;
    }

    public void ProcessUpdatesFromServer()
    {
      // Start a new timer that will simulate updates coming from the server
      // every 15 millisecoonds
      // Do not do anything like this in PS8
      Timer serverTimer = new Timer();
      serverTimer.Interval = 15;
      serverTimer.Elapsed += UpdateCameFromServer;
      serverTimer.Start();
    }


    // Simulate a game update from a fake server
    // This method is invoked every time the "serverTimer", above, ticks
    private void UpdateCameFromServer(object sender, ElapsedEventArgs e)
    {
      List<Player> newPlayers;
      List<Powerup> newPowerups;

      // Get the update from the "server"
      server.GenerateFakeServerUpdate(out newPlayers, out newPowerups);

      //Random r = new Random(); // ignore this for now

      // The server is not required to send updates about every object,
      // so we update our local copy of the world only for the objects that
      // the server gave us an update for.

      foreach (Player play in newPlayers)
      {
        //while (r.Next() % 1000 != 0) ; // ignore this loop for now
        if (!play.GetActive())
          theWorld.Players.Remove(play.GetID());
        else
          theWorld.Players[play.GetID()] = play;
      }

      foreach (Powerup pow in newPowerups)
      {
        if (!pow.GetActive())
          theWorld.Powerups.Remove(pow.GetID());
        else
          theWorld.Powerups[pow.GetID()] = pow;
      }

      // Notify any listeners (the view) that a new game world has arrived from the server
      if (UpdateArrived != null)
        UpdateArrived();

      // For whatever user inputs happened during the last frame,
      // process them.
      ProcessInputs();

    }

    /// <summary>
    /// Checks which inputs are currently held down
    /// Normally this would send a message to the server
    /// </summary>
    private void ProcessInputs()
    {
      if (movingPressed)
        Console.WriteLine("moving");
      if (mousePressed)
        Console.WriteLine("mouse pressed");
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
