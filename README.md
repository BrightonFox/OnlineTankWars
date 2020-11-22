---
title: Game README document
project: Tank Wars Game
version: v1.0
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
<!-- << HTML Header for html stuff >> -->




<!-- << Begin Markdown Document >> -->

  Game Project: Tank Wars 
===========================
#### _(PS7-PS9)_

&lt;Clever thing here&gt;

## About
_&lt;Brief description of project&gt;_


## PS8: Tank Wars Client
<!-- 
  *   README Requirements 
  * =======================
  *   (From PS8 assignment brief)
  * Your project README should document all of your design decisions, 
  *   as well as detailing any features you wish the graders to be aware of.
  *
  * This file will be the "first stop" when your work is being evaluated. 
  * Set the tone by doing a good job describing what works and doesn't work, 
  *   as well as listing interesting things (i.e., features) 
  *   that you would like the graders to be aware of. 
  -->

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

### Schedule
-[X] Complete `README.md` documentation.

#### Model
-[X] Make General json Models
-[X] Make client specific extension of json models
-[X] Finish commenting the code
  -[X] Comment Method XML-Doc Headers.
  -[X] Comment inside Methods where necessary for clarity.
  -[X] Comment Public Property & Field XML-Doc Headers.
  -[X] Comment Class and Constructor XML-Doc Headers.

#### Controller
-[X] Set up Network Capabilities
  -[X] Code Json receive
  -[X] Code Json send
-[X] Create Events for the the View
-[X] Create Retrieve Methods for the View
-[X] Finish commenting the code
  -[X] Comment Method XML-Doc Headers.
  -[X] Comment inside Methods where necessary for clarity.
  -[X] Comment Public Property, Field, & Event XML-Doc Headers.
  -[X] Comment Class and Constructor XML-Doc Headers.

#### View 
-[X] Import Drawing Panel from Lab 12
  -[X] Create draw methods and add to `OnPaint` for all the objects
-[X] Design `MainForm`
  -[X] Handle connection interface
-[X] Handle center on player tank
-[X] Create methods to register to appropriate controller events.
-[X] Add at least 1 new small feature
-[X] Finish commenting the code
  -[X] Comment Method XML-Doc Headers.
  -[X] Comment inside Methods where necessary for clarity.
  -[X] Comment Public Property & Field XML-Doc Headers.
  -[X] Comment Class and Constructor XML-Doc Headers.



## PS9: Tank Wars Server + Web Server
<!-- Useful link when we need to set up ASP.NET https://www.syncfusion.com/blogs/post/how-to-develop-an-asp-net-core-application-using-visual-studio-code.aspx -->



<!-- << End of Markdown Document >> -->
