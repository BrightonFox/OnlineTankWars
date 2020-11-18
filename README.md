---
title: Game README document
project: Tank Wars Game
version: v0.0
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
We have a general model design that will get extended as appropriate for a client model and sever model.

### Schedule
#### Model
-[X] Make General json Models
-[X] Make client specific extension of json models
-[ ] Finish commenting the code
  -[ ] Comment Method XML-Doc Headers.
  -[ ] Comment inside Methods where necessary for clarity.
  -[ ] Comment Public Property & Field XML-Doc Headers.
  -[ ] Comment Class and Constructor XML-Doc Headers.

#### Controller
-[X] Set up Network Capabilities
  -[X] Code Json receive
  -[X] Code Json send
-[X] Create Events for the the View
-[X] Create Retrieve Methods for the View
-[ ] Finish commenting the code
  -[X] Comment Method XML-Doc Headers.
  -[ ] Comment inside Methods where necessary for clarity.
  -[ ] Comment Public Property, Field, & Event XML-Doc Headers.
  -[ ] Comment Class and Constructor XML-Doc Headers.

#### View 
-[X] Import Drawing Panel from Lab 12
  -[X] Create draw methods and add to `OnPaint` for all the objects
-[X] Design `MainForm`
  -[X] Handle connection interface
-[X] Handle center on player tank
-[X] Create methods to register to appropriate controller events.
-[ ] Finish commenting the code
  -[ ] Comment Method XML-Doc Headers.
  -[ ] Comment inside Methods where necessary for clarity.
  -[ ] Comment Public Property & Field XML-Doc Headers.
  -[ ] Comment Class and Constructor XML-Doc Headers.


#### 11-18 Debug TODO:
-[ ] Add Server disconnect event and separate `OnBtnDisconnect`
-[ ] Figure out init networking issue.
  -[ ] player id and world size not always being returned from network.
    - Try running server on slower speed.
  -[ ] figure out why we keep trying to add walls that already exist in dict
    - We should not be receiving any duplicate walls at all.
    - Check to see if when that occurs the data of the deserialized walls is correct.
      - Try making a method in the `IWord` that deserialized to the base json classes then converts to client versions.
      - Or try taking the advice of this [stack overflow page](https://stackoverflow.com/questions/22465868/deserialize-into-correct-child-objects) or [this one](https://stackoverflow.com/questions/5951043/how-to-inherit-the-attribute-from-interface-to-object-when-serializing-it-using)
  -[ ] Figure out double error reporting issue.
    - Check that exceptions are not thrown out of `OnNetworkAction` in `NetworkingUtils`
-[ ] Debug whatever is left...



## PS9: Tank Wars Server + Web Server
<!-- Useful link when we need to set up ASP.NET https://www.syncfusion.com/blogs/post/how-to-develop-an-asp-net-core-application-using-visual-studio-code.aspx -->



<!-- << End of Markdown Document >> -->
