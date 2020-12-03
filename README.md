---
title: Game README document
project: Tank Wars Game
version: 
  client: v1.0
  server: v1.0
team-name: JustBroken
authors: 
  -
    name: Andrew Osterhout
    uID: u1317172
  -
    name: Brighton Fox
    uid: u0981544
organization: University of Utah (UofU)
  semester: fall 2020
  course: 
    id: CS3500
    name: Software Practice
---

<!-- << Begin Markdown Document >> -->

  Game Project: Tank Wars 
===========================
#### _(PS7-PS9)_

&lt;Clever thing here&gt;

## About
A basic multiplayer 2D tank game.
With a Server and Client programs included.


## Table of Contents:
- [Game Project: Tank Wars](#game-project-tank-wars)
      - [_(PS7-PS9)_](#ps7-ps9)
  - [About](#about)
  - [Table of Contents:](#table-of-contents)
  - [PS8: Tank Wars Client](#ps8-tank-wars-client)
    - [Design Decisions](#design-decisions)
    - [Added Features:](#added-features)
  - [PS9: Tank Wars Server](#ps9-tank-wars-server)
    - [Design Decisions:](#design-decisions-1)
    - [Added Features:](#added-features-1)


## PS8: Tank Wars Client

### Design Decisions
- We have a general model design that will get extended as appropriate for a client model and sever model.
  - Made abstract classes for general objects that can be extended for either the client or the server.
    - Abstract Json classes to ensure consistent serializing and deserializing.
- Made our namespaces be more descriptive and organized.
- Made our solution structure and file structure far more organized and conform to more general project organization standards.
- `Controller` 
  - Split the way the incoming json messages are parsed before and after wall objects are received.
  - On receiving messages from server, only full objects are parsed and removed from the socket's buffer.
  - Only send a command Json when the user has actually interacted with the client.
- `drawingPanel`
  - Made all disposable graphics elements private class elements, created on object creation, 
     and made sure dispose was called `.Dispose()` on them in the `drawingPanel` deconstruction, and anywhere else reasonable.
  - Skip drawing tanks when their health is 0.
  - Created a custom ImageCollection class to concisely handle the images used to represent each tank body, turret, and projectiles, 
     grouped by color.
  - Made the tanks color be determined by modulus division of the id.
    - This should make the first 8 tanks to connect different colors with how the server works, 
       but that consistency logically does not keep all tanks different colors of more than 8 tanks exist or when players disconnect.
  - Draw each wall received from its "top-left" section despite order in which coordinates in P1 and P2 are given.
- `MainForm`
  - Made it agnostic to the world, and just pass Drawing panel a copy of the world in it's constructor. 
  - The player name entry text box max character count is set to 16 to prevent longer names than that from being entered.
  - All data from the text entry fields is striped of leading and trailing whitespace.
  - Closing or exiting the view will properly dispose all graphics used.

### Added Features:
- Made the health indicators be little hearts that change color to indicate health level.
- Made the text below the tank that has the player name and score always be roughly centered on the tank (horizontally).
  - This way player names with different lengths don't look wacky. 
- Set Custom Font for the menu system.
- Added an about button indicated with ℹ, to get credit information about the application.
- Added a controls information button indicated with ⌨, to get information on how our control scheme is set up.
- Added a way to disconnect from the server without shutting down the client.
  - Implemented with a disconnect button as well as the <kbd>esc</kbd> while connected.
  - The player name, server address, and connect button are disabled when connected to a server.
  - The disconnect button is disabled when the client is not connected to a server.
- Hitting <kbd>enter</kbd> in the server address text box will also connect to the server.
- Added an Icon to the form so it looks less in dev.
- Made the Menu system size be semi-dynamic.
  - The server address bar grows and shrinks according to the space available.
  - The position of elements in the menu are "linked"/derived from the other elements so they position properly as well.
- If the player name is less than 3 characters, the client will pop up a waring message and not connect to server.
- Similarly if the server address provided blank another message box will pop up and it will not attempt to connect to server.
- Made an extended control scheme for in game:
  - <kbd>space</kbd> & <kbd>left click</kbd> will fire the primary projectile.
  - <kbd>E</kbd> & <kbd>right click</kbd> will fire the alternate beam weapon.
  - You can use the arrow keys or <kbd>WASD</kbd> to move the tank.
  - <kbd>esc</kbd> will disconnect from the server.



## PS9: Tank Wars Server

### Design Decisions:
- Decided to Design the server program in MVC protocols, in case we wanted to make fancier Interface for it down the road.
  - The view does:
    - Launch the server, and stops it when the user types "stop" (case insensitive).
    - Prints messages w/ some formatting that the controller can send via an event.
    - Print Error messages with special formatting that the controller can pass via an event.
    - Things the view could do since the controller supports it, but we never implemented in the view:
      - Has the option to provide a file path for the settings and technically even change the port number when creating the controller.
      - Allow for the view to create multiple controllers and therefore multiple servers simultaneously.
  - The controller does:
    - Handles how long to wait between sending frames.
    - Handles all networking, and stores the states for each player.
    - Sends normal log messages to the view.
    - Sends error messages to the view separately.
      - should not ever allow an exception to be uncaught.
    - Only allows the first command json received in the `Networking.GetData()` to be sent to the model to be queued for processing.
      - Disregards any sent past the first full one received & clears all fully received json command objects out of the socket state data buffer before starting to listen for more data again.
    - The controller has a way to stop the server by calling a method.
      - This method is automatically called when the controller is deleted via the deconstructor.
  - The Model does:
    - All things that have to do with the rules and mechanics of the TankWars Game.
- Separated the World into multiple partial classes in separate files for better organization:
  - [`World.cs`](src/server/model/World.cs): contains the game logic that is **not** about calculating if two objects are colliding.
  - [`World.collisions.cs`](src/server/model/World.collisions.cs): contains the game logic that **is** about calculating if two objects are colliding.
  - [`World.settings.cs`](src/server/model/World.settings.cs): contains the user-editable settings for the game and code necessary to import them from an xml file.
    - Also contains a small extension class for adding elements to the object dictionaries, without having to specify the id as the parameter, in the `Dictionary<int,...>.Add()` method.
- The world contains all of the game logic that involves the various other model object interacting and stores all of the game objects.
- The World handles reading in the settings xml file.
- We have one method for general collision calculating, that is the same method regardless of what object you are checking the collision with (excluding beams and walls as inputs).
  - It is mildly optimized to only check collisions with objects that the object in question could collide with.
  - We calculate collisions with walls by checking if rectangles overlap (AABB).
    - We calculate an upper bound corner and lower bound corner based upon the provided locations in the Wall Constructor, to reduce the amount of work that needs to happen for every collision check.
      - Since we are still using the same shared general model that is shared between the server and the client for things like the networking, `Vector2D` object and actually relevant to this point basic Json representations of the objects that get sent between the server and client, we still save the provided points for the wall, and don't just modify them for out upper and lower bound corners of the wall.  
- Handled the "timers" of events that have an associated cool down in the properties holding the values so you don't have to care about decrementing or resetting them when appropriate.
- The Model (_aka_ the World) will only accept the first command that the controller sends to it, per frame, regardless if clients end up sending many many commands per frame.
- We process commands in the order that we received them.
- We spawn powerups, move/handle existing projectiles, and attempt to respawn dead tanks before we process any commands received from the clients.
- World return a long string of all the json objects in the world when the controller asks for a new frame.
  - this allows us to use a string builder to improve efficiency.
  - this also helps keep everything about interpreting json objects in the model rather than the controller.


### Added Features:
- All unnecessary _"Basic Data"_, except for Wall Size, can be specified in the XML, with basically the same names as specified in assignment brief.
- We created some alternative names that can be used in the settings xml file, to help make the naming of things more intuitive and consistent across our code and for "future users".
- Formatting in the console messaging for distinguishing errors and basic log messages.
- If an object cannot be spawned in due to no available room, the server will continue all of its other operations until a spot becomes available.
- Tanks will move up to the edge of a wall regardless of speed and initial position, if they would have travelled "inside" of the wall with their movement command.
- If the tank teleports to the other side of the world because it passes the boundary, it will teleport the tank to some point along either the x/y axis when you change directions to get unstuck.

<!-- << End of Markdown Document >> -->
