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
 *   + v1.0 - Submittal - 2020/11/21
 * 
 * About:
 *   An object representing a player within the game. Primarily
 *   used to hold the players ID that links them to their tank.
 */


namespace TankWars.Client.Model
{
    /// <summary>
    /// Object representing the information associated with the player using the client.
    /// Basically an overglorified struct.
    /// </summary>
    public class Player
    {

        /// <summary>
        /// Object representing the information associated with the player using the client.
        /// Basically an overglorified struct.
        /// </summary>
        public Player(string name)
        {
            Name = name;
        }

        /// <summary>
        /// bool representing whether or not an id has been assigned to the player yet.
        /// </summary>
        public bool IdSet { get; private set; }

        /// <summary>
        /// String containing the name of the player.
        /// </summary>
        public string Name { get; private set; }

        private int _id;

        /// <summary>
        /// The Id of the player using the client and their controlled <see cref="Tank"/>.
        /// <para>!! Can only be set once !!</para>
        /// </summary>
        /// <value>The id of the player (!! Can only be set once !!)</value>
        public int Id
        {
            get
            {
                return _id;
            }
            set
            {
                if (!IdSet)
                {
                    _id = value;
                    IdSet = true;
                }
            }
        }
    }
}
