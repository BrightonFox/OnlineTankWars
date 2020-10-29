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
 *   + Template/Starter-Code:
 *       Author: Daniel Kopta
 *       Date: May 2019
 *       About: Unit testing examples for CS 3500 networking library (part of final project)
 *   + ...
 */


using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace NetworkUtil
{

  [TestClass]
  public class NetworkTests
  {
    // When testing network code, we have some necessary global state,
    // since open sockets are system-wide (managed by the OS)
    // Therefore, we need some per-test setup and cleanup
    private TcpListener testListener;
    private SocketState testLocalSocketState, testRemoteSocketState;


    [TestInitialize]
    public void Init()
    {
      testListener = null;
      testLocalSocketState = null;
      testRemoteSocketState = null;
    }


    [TestCleanup]
    public void Cleanup()
    {
      StopTestServer(testListener, testLocalSocketState, testRemoteSocketState);
    }


    private void StopTestServer(TcpListener listener, SocketState socket1, SocketState socket2)
    {
      try
      {
        // '?.' is just shorthand for null checks
        listener?.Stop();
        socket1?.TheSocket?.Shutdown(SocketShutdown.Both);
        socket1?.TheSocket?.Close();
        socket2?.TheSocket?.Shutdown(SocketShutdown.Both);
        socket2?.TheSocket?.Close();
      }
      // Do nothing with the exception, since shutting down the server will likely result in 
      // a prematurely closed socket
      // If the timeout is long enough, the shutdown should succeed
      catch (Exception) { }
    }



    public void SetupTestConnections(bool clientSide,
      out TcpListener listener, out SocketState local, out SocketState remote)
    {
      if (clientSide)
      {
        NetworkTestHelper.SetupSingleConnectionTest(
          out listener,
          out local,    // local becomes client
          out remote);  // remote becomes server
      }
      else
      {
        NetworkTestHelper.SetupSingleConnectionTest(
          out listener,
          out remote,   // remote becomes client
          out local);   // local becomes server
      }

      Assert.IsNotNull(local);
      Assert.IsNotNull(remote);
    }


    /*** Begin Basic Connectivity Tests ***/
    [TestMethod]
    public void TestConnect()
    {
      NetworkTestHelper.SetupSingleConnectionTest(out testListener, out testLocalSocketState, out testRemoteSocketState);

      Assert.IsTrue(testRemoteSocketState.TheSocket.Connected);
      Assert.IsTrue(testLocalSocketState.TheSocket.Connected);

      Assert.AreEqual("127.0.0.1:2112", testLocalSocketState.TheSocket.RemoteEndPoint.ToString());
    }


    [TestMethod]
    public void TestConnectNoServer()
    {
      bool isCalled = false;

      void saveClientState(SocketState x)
      {
        isCalled = true;
        testLocalSocketState = x;
      }

      // Try to connect without setting up a server first.
      Networking.ConnectToServer(saveClientState, "localhost", 2112);
      NetworkTestHelper.WaitForOrTimeout(() => isCalled, NetworkTestHelper.timeout);

      Assert.IsTrue(isCalled);
      Assert.IsTrue(testLocalSocketState.ErrorOccured);
    }


    [TestMethod]
    public void TestConnectTimeout()
    {
      bool isCalled = false;

      void saveClientState(SocketState x)
      {
        isCalled = true;
        testLocalSocketState = x;
      }

      Networking.ConnectToServer(saveClientState, "google.com", 2112);

      // The connection should timeout after 3 seconds. NetworkTestHelper.timeout is 5 seconds.
      NetworkTestHelper.WaitForOrTimeout(() => isCalled, NetworkTestHelper.timeout);

      Assert.IsTrue(isCalled);
      Assert.IsTrue(testLocalSocketState.ErrorOccured);
    }


    [TestMethod]
    public void TestConnectCallsDelegate()
    {
      bool serverActionCalled = false;
      bool clientActionCalled = false;

      void saveServerState(SocketState x)
      {
        testLocalSocketState = x;
        serverActionCalled = true;
      }

      void saveClientState(SocketState x)
      {
        testRemoteSocketState = x;
        clientActionCalled = true;
      }

      testListener = Networking.StartServer(saveServerState, 2112);
      Networking.ConnectToServer(saveClientState, "localhost", 2112);
      NetworkTestHelper.WaitForOrTimeout(() => serverActionCalled, NetworkTestHelper.timeout);
      NetworkTestHelper.WaitForOrTimeout(() => clientActionCalled, NetworkTestHelper.timeout);

      Assert.IsTrue(serverActionCalled);
      Assert.IsTrue(clientActionCalled);
    }


    /// <summary>
    /// This is an example of a parameterized test. 
    /// DataRow(true) and DataRow(false) means this test will be 
    /// invoked once with an argument of true, and once with false.
    /// This way we can test your Send method from both
    /// client and server sockets. In theory, there should be no 
    /// difference, but odd things can happen if you save static
    /// state (such as sockets) in your networking library.
    /// </summary>
    /// <param name="clientSide"></param>
    [DataRow(true)]
    [DataRow(false)]
    [DataTestMethod]
    public void TestDisconnectLocalThenSend(bool clientSide)
    {
      SetupTestConnections(clientSide, out testListener, out testLocalSocketState, out testRemoteSocketState);

      testLocalSocketState.TheSocket.Shutdown(SocketShutdown.Both);

      // No assertions, but the following should not result in an unhandled exception
      Networking.Send(testLocalSocketState.TheSocket, "a");
    }

    /*** End Basic Connectivity Tests ***/


    /*** Begin Send/Receive Tests ***/

    // In these tests, "local" means the SocketState doing the sending,
    // and "remote" is the one doing the receiving.
    // Each test will run twice, swapping the sender and receiver between
    // client and server, in order to defeat statically-saved SocketStates
    [DataRow(true)]
    [DataRow(false)]
    [DataTestMethod]
    public void TestSendTinyMessage(bool clientSide)
    {
      SetupTestConnections(clientSide, out testListener, out testLocalSocketState, out testRemoteSocketState);

      // Set the action to do nothing
      testLocalSocketState.OnNetworkAction = x => { };
      testRemoteSocketState.OnNetworkAction = x => { };

      Networking.Send(testLocalSocketState.TheSocket, "a");

      Networking.GetData(testRemoteSocketState);

      // Note that waiting for data like this is *NOT* how the networking library is 
      // intended to be used. This is only for testing purposes.
      // Normally, you would provide an OnNetworkAction that handles the data.
      NetworkTestHelper.WaitForOrTimeout(() => testRemoteSocketState.GetData().Length > 0, NetworkTestHelper.timeout);

      Assert.AreEqual("a", testRemoteSocketState.GetData());
    }

    
    [DataRow(true)]
    [DataRow(false)]
    [DataTestMethod]
    public void TestSendMessage(bool clientSide)
    {
      SetupTestConnections(clientSide, out testListener, out testLocalSocketState, out testRemoteSocketState);

      // Set the action to do nothing
      testLocalSocketState.OnNetworkAction = x => { };
      testRemoteSocketState.OnNetworkAction = x => { };

      var testMessage = "I feel like there is a convention in unit testing to make all variabel names as long as possible !!";

      Networking.Send(testLocalSocketState.TheSocket, testMessage);

      Networking.GetData(testRemoteSocketState);

      // Note that waiting for data like this is *NOT* how the networking library is 
      // intended to be used. This is only for testing purposes.
      // Normally, you would provide an OnNetworkAction that handles the data.
      NetworkTestHelper.WaitForOrTimeout(() => testRemoteSocketState.GetData().Length > 0, NetworkTestHelper.timeout);

      Assert.AreEqual(testMessage, testRemoteSocketState.GetData());
    }


    [DataRow(true)]
    [DataRow(false)]
    [DataTestMethod]
    public void TestSendLargeMessage(bool clientSide)
    {
      SetupTestConnections(clientSide, out testListener, out testLocalSocketState, out testRemoteSocketState);

      // Set the action to do nothing
      testLocalSocketState.OnNetworkAction = x => { };
      testRemoteSocketState.OnNetworkAction = x => { };

      var testMessage = @"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
lDlPQ
duo9QzXpqbK2erk3HwHRBjU6b1czt245l5M6hsWUTDaOavwl5gDpV2uaWeRDlyMBHaxAsjVIROoHGuOtyh2BMz87b0u1O33XFR2wBaN0UujDKCosQK4b4Ea4zylErj4ver9yAAY3D3uAvYSH7Mpmmnv7DUKwuplfT88i05Jr2lBWoQf5YoJ7jLel1
B2ElzeDX8190lywKRxtLQhBCW08YlNxWrhZkg2M96Wx20lHnDU4UrPWT600Me1ujEzs4QPKbcNP7gFXxTWepD2ZuYezT9jljMzfQJULgfi6zBRVqW7xwh3qcsIwKh1yQKduDwxCERO1TbIGTjfa5bSIJYttCqzKZT7VRYP9sPFVM7oa0KNGvFFeofNkhsI45Yi4P67sfOTaEkJWeZxUmsCK5rwVMFoyVNrfAIP
elTyr7Sq64hlWqeONglPhZNX2rJ3Wg1uJpIf6wNYFJdVV6oHwp01zzmoqjRI79ByVcbeL9wfi962MJ20KtB34Dwri1M7EjSwZrtFOD5W1ECp6yI5gf834hr7b5ZiYgtB8aUzHCMkEr5hbt98B7Rv2uwMB4Rx3rq6iaQjbyOnA6njZzENZJngVduk9zYYzyMMCeuooY46HLrBOVIHPS4byLewa3rkgkG4Ve1vPtqqQUncUJLSQc
nDWWykH5wHwfoHrgiEtmSOoyC9JvmPnel858CjSDNsUULDMaB67RLBG0vFbJNEw3T4z1cPKuGyxE4BmkXxdcjofHLoGvjr2tDqmyfUymrUChd3D5zXF0yGrpRjp2kf4pDtq2MfzrfKmMWJTbJkeepEuWH5JW4EcAw351HhXTUM9CzU3VSebWUjqZnbzWwPH2BAkFUc9DZyDluPEExXt8io6fHjU3SXjCNh5LCw2fZBkwnwOHImwswNIObjGZjHWHavbOR7G7S6WypiDPorBWL8lUtHYOzhsqUffsTeoN6u
LvAi4Eag4m9XgT8tzPOXhljPKe1QinjoQt93lNZqZVDxk5GQPFTDQuyHmfFgbzZeZYTjaWH0Us97Xgmeful1gBMDYOdvi7kq9HmBGtaGa6aZsAMizDMMcFMG9T62j6qNBe32mv8xfd0qCKjQjiwKskWQ9eDoTQEsx2eq02jmxn2U47laT6haovy3xTUZqLwvLGvrLPq1lqqwIieaIi3HbGDVQy0g8Za8zHjtKzwo3EKCldXmalV4SnhQImRdlndW9OdW9xhDQ1cVXz2qDoirhvJfYHucXVat94y4XUGW14OJWQenjq9lHJcgR7dxTXtDqB8hRud4smGcR9oMejcDMY3X8akhLcReG1CWyr2zOAx1boCcPnZtN8GxeyHSZptqgIF83qapcJSJLOPDpItwPx5tESU5Ew7FMzKhtViB3K5EPRatyHvwPuQuYt4ldh1EYn6fniOgWo8Du9mF1GGQiAEs31ZjMfGr19l01vtzQeYMJcffHP9Rer9nZyl83u9ERxBoAe7ewAwv5gpSuL88qKL51mh16tqITwDCmVtAhyyb3dum0hRT6CKqF7VR8CTSxWzoxFPemdQHHZB6raIVU5MNsurXRba5dRZUouPifSNPixaEFvybKDWNnpAjetdK3kQaJnjxlAI31L018Hj54LPQJXhK1Lgi0kl5r5gkCew6kiOgGaZ0Tqzd0CoUJUwM5cbXr3UENoGhaMxsSPkFcnSBx8VVK2x5f3yJk3XcRyG7w7";

      Networking.Send(testLocalSocketState.TheSocket, testMessage);

      Networking.GetData(testRemoteSocketState);

      // Note that waiting for data like this is *NOT* how the networking library is 
      // intended to be used. This is only for testing purposes.
      // Normally, you would provide an OnNetworkAction that handles the data.
      NetworkTestHelper.WaitForOrTimeout(() => testRemoteSocketState.GetData().Length > 0, NetworkTestHelper.timeout);

      Assert.AreEqual(testMessage, testRemoteSocketState.GetData());
    }

    [DataRow(true)]
    [DataRow(false)]
    [DataTestMethod]
    public void TestNoEventLoop(bool clientSide)
    {
      SetupTestConnections(clientSide, out testListener, out testLocalSocketState, out testRemoteSocketState);

      int calledCount = 0;

      // This OnNetworkAction will not ask for more data after receiving one message,
      // so it should only ever receive one message
      testLocalSocketState.OnNetworkAction = (x) => calledCount++;

      Networking.Send(testRemoteSocketState.TheSocket, "a");
      Networking.GetData(testLocalSocketState);
      // Note that waiting for data like this is *NOT* how the networking library is 
      // intended to be used. This is only for testing purposes.
      // Normally, you would provide an OnNetworkAction that handles the data.
      NetworkTestHelper.WaitForOrTimeout(() => testLocalSocketState.GetData().Length > 0, NetworkTestHelper.timeout);

      // Send a second message (which should not increment calledCount)
      Networking.Send(testRemoteSocketState.TheSocket, "a");
      NetworkTestHelper.WaitForOrTimeout(() => false, NetworkTestHelper.timeout);

      Assert.AreEqual(1, calledCount);
    }


    [DataRow(true)]
    [DataRow(false)]
    [DataTestMethod]
    public void TestDelayedSends(bool clientSide)
    {
      SetupTestConnections(clientSide, out testListener, out testLocalSocketState, out testRemoteSocketState);

      // Set the action to do nothing
      testLocalSocketState.OnNetworkAction = x => { };
      testRemoteSocketState.OnNetworkAction = x => { };

      Networking.Send(testLocalSocketState.TheSocket, "a");
      Networking.GetData(testRemoteSocketState);
      // Note that waiting for data like this is *NOT* how the networking library is 
      // intended to be used. This is only for testing purposes.
      // Normally, you would provide an OnNetworkAction that handles the data.
      NetworkTestHelper.WaitForOrTimeout(() => testRemoteSocketState.GetData().Length > 0, NetworkTestHelper.timeout);

      Networking.Send(testLocalSocketState.TheSocket, "b");
      Networking.GetData(testRemoteSocketState);
      // Note that waiting for data like this is *NOT* how the networking library is 
      // intended to be used. This is only for testing purposes.
      // Normally, you would provide an OnNetworkAction that handles the data.
      NetworkTestHelper.WaitForOrTimeout(() => testRemoteSocketState.GetData().Length > 1, NetworkTestHelper.timeout);

      Assert.AreEqual("ab", testRemoteSocketState.GetData());
    }


    [DataRow(true)]
    [DataRow(false)]
    [DataTestMethod]
    public void TestEventLoop(bool clientSide)
    {
      SetupTestConnections(clientSide, out testListener, out testLocalSocketState, out testRemoteSocketState);

      int calledCount = 0;

      // This OnNetworkAction asks for more data, creating an event loop
      testLocalSocketState.OnNetworkAction = (x) =>
      {
        if (x.ErrorOccured)
          return;
        calledCount++;
        Networking.GetData(x);
      };

      Networking.Send(testRemoteSocketState.TheSocket, "a");
      Networking.GetData(testLocalSocketState);
      NetworkTestHelper.WaitForOrTimeout(() => calledCount == 1, NetworkTestHelper.timeout);

      Networking.Send(testRemoteSocketState.TheSocket, "a");
      NetworkTestHelper.WaitForOrTimeout(() => calledCount == 2, NetworkTestHelper.timeout);

      Assert.AreEqual(2, calledCount);
    }


    [DataRow(true)]
    [DataRow(false)]
    [DataTestMethod]
    public void TestChangeOnNetworkAction(bool clientSide)
    {
      SetupTestConnections(clientSide, out testListener, out testLocalSocketState, out testRemoteSocketState);

      int firstCalledCount = 0;
      int secondCalledCount = 0;

      // This is an example of a nested method (just another way to make a quick delegate)
      void firstOnNetworkAction(SocketState state)
      {
        if (state.ErrorOccured)
          return;
        firstCalledCount++;
        state.OnNetworkAction = secondOnNetworkAction;
        Networking.GetData(testLocalSocketState);
      }

      void secondOnNetworkAction(SocketState state)
      {
        secondCalledCount++;
      }

      // Change the OnNetworkAction after the first invocation
      testLocalSocketState.OnNetworkAction = firstOnNetworkAction;

      Networking.Send(testRemoteSocketState.TheSocket, "a");
      Networking.GetData(testLocalSocketState);
      NetworkTestHelper.WaitForOrTimeout(() => firstCalledCount == 1, NetworkTestHelper.timeout);

      Networking.Send(testRemoteSocketState.TheSocket, "a");
      NetworkTestHelper.WaitForOrTimeout(() => secondCalledCount == 1, NetworkTestHelper.timeout);

      Assert.AreEqual(1, firstCalledCount);
      Assert.AreEqual(1, secondCalledCount);
    }



    [DataRow(true)]
    [DataRow(false)]
    [DataTestMethod]
    public void TestReceiveRemovesAll(bool clientSide)
    {
      SetupTestConnections(clientSide, out testListener, out testLocalSocketState, out testRemoteSocketState);

      StringBuilder localCopy = new StringBuilder();

      void removeMessage(SocketState state)
      {
        if (state.ErrorOccured)
          return;
        localCopy.Append(state.GetData());
        state.RemoveData(0, state.GetData().Length);
        Networking.GetData(state);
      }

      testLocalSocketState.OnNetworkAction = removeMessage;

      // Start a receive loop
      Networking.GetData(testLocalSocketState);

      for (int i = 0; i < 10000; i++)
      {
        char c = (char)('a' + (i % 26));
        Networking.Send(testRemoteSocketState.TheSocket, "" + c);
      }

      NetworkTestHelper.WaitForOrTimeout(() => localCopy.Length == 10000, NetworkTestHelper.timeout);

      // Reconstruct the original message outside the send loop
      // to (in theory) make the send operations happen more rapidly.
      StringBuilder message = new StringBuilder();
      for (int i = 0; i < 10000; i++)
      {
        char c = (char)('a' + (i % 26));
        message.Append(c);
      }

      Assert.AreEqual(message.ToString(), localCopy.ToString());
    }


    [DataRow(true)]
    [DataRow(false)]
    [DataTestMethod]
    public void TestReceiveRemovesPartial(bool clientSide)
    {
      SetupTestConnections(clientSide, out testListener, out testLocalSocketState, out testRemoteSocketState);

      const string toSend = "abcdefghijklmnopqrstuvwxyz";

      // Use a static seed for reproducibility
      Random rand = new Random(0);

      StringBuilder localCopy = new StringBuilder();

      void removeMessage(SocketState state)
      {
        if (state.ErrorOccured)
          return;
        int numToRemove = rand.Next(state.GetData().Length);
        localCopy.Append(state.GetData().Substring(0, numToRemove));
        state.RemoveData(0, numToRemove);
        Networking.GetData(state);
      }

      testLocalSocketState.OnNetworkAction = removeMessage;

      // Start a receive loop
      Networking.GetData(testLocalSocketState);

      for (int i = 0; i < 1000; i++)
      {
        Networking.Send(testRemoteSocketState.TheSocket, toSend);
      }

      // Wait a while
      NetworkTestHelper.WaitForOrTimeout(() => false, NetworkTestHelper.timeout);

      localCopy.Append(testLocalSocketState.GetData());

      // Reconstruct the original message outside the send loop
      // to (in theory) make the send operations happen more rapidly.
      StringBuilder message = new StringBuilder();
      for (int i = 0; i < 1000; i++)
      {
        message.Append(toSend);
      }

      Assert.AreEqual(message.ToString(), localCopy.ToString());
    }



    [DataRow(true)]
    [DataRow(false)]
    [DataTestMethod]
    public void TestReceiveHugeMessage(bool clientSide)
    {
      SetupTestConnections(clientSide, out testListener, out testLocalSocketState, out testRemoteSocketState);

      testLocalSocketState.OnNetworkAction = (x) =>
      {
        if (x.ErrorOccured)
          return;
        Networking.GetData(x);
      };

      Networking.GetData(testLocalSocketState);

      StringBuilder message = new StringBuilder();
      message.Append('a', (int)(SocketState.BufferSize * 7.5));

      Networking.Send(testRemoteSocketState.TheSocket, message.ToString());

      NetworkTestHelper.WaitForOrTimeout(() => testLocalSocketState.GetData().Length == message.Length, NetworkTestHelper.timeout);

      Assert.AreEqual(message.ToString(), testLocalSocketState.GetData());
    }

    /*** End Send/Receive Tests ***/


    //TODO: Add more of your own tests here

  }
}
