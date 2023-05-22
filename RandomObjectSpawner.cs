using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;


public class RandomObjectSpawner : MonoBehaviour
{


    private Socket _clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
    private byte[] _recieveBuffer = new byte[8142];

    private void SetupServer()
    {
        try

        {
            Debug.Log("Conectat");
            _clientSocket.Connect(new IPEndPoint(IPAddress.Loopback, 6670));
        }
        catch (SocketException ex)
        {
            Debug.Log(ex.Message);
        }

        _clientSocket.BeginReceive(_recieveBuffer, 0, _recieveBuffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), null);

    }

    private void ReceiveCallback(IAsyncResult AR)
    {
        //Check how much bytes are recieved and call EndRecieve to finalize handshake
        int recieved = _clientSocket.EndReceive(AR);
        Debug.Log("Primesc ceva");
        if (recieved <= 0)
            return;

        //Copy the recieved data into new buffer , to avoid null bytes
        byte[] recData = new byte[recieved];
        Buffer.BlockCopy(_recieveBuffer, 0, recData, 0, recieved);

        //Process data here the way you want , all your bytes will be stored in recData

        //Start receiving again
        _clientSocket.BeginReceive(_recieveBuffer, 0, _recieveBuffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), null);
    }
    /* private const int bufferSize = 1024;
     private const int port = 38000;

     private Socket listener;
     private byte[] buffer = new byte[bufferSize];

     public GameObject[] GameObjectPrefab;
     Socket client;
     private void Start()
     {
         // Start listening for data
         client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
         client.Connect("127.0.0.1", 1234); // Change the IP address and port number as needed
         Debug.Log("Client connected");
         byte[] buffer = new byte[1024];
         client.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, OnReceive, new object[] { client, buffer });
         Debug.Log("Recieved from ");
     }

     private void ReceiveData()
     {
         try
         {
             int bytesRead = client.Receive(buffer);
             Debug.Log("Received data length: " + bytesRead);

             if (bytesRead > 0)
             {
                 string data = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                 Debug.Log("Received data: " + data);

                 if (int.TryParse(data, out int numObjects))
                 {
                     Debug.Log("Parsed number of objects: " + numObjects);

                     // Spawn the desired number of objects in Unity
                     for (int i = 0; i < numObjects; i++)
                     {
                         Vector3 randomSpawnLocation = new Vector3(1, 5, 1);
                         Instantiate(GameObjectPrefab[0], randomSpawnLocation, Quaternion.identity);
                     }
                 }
                 else
                 {
                     Debug.Log("Failed to parse received data as integer");
                 }
             }

             // Continue receiving data
             ReceiveData();
         }
         catch (SocketException ex)
         {
             Debug.LogError("SocketException: " + ex.Message);
             client.Close();
         }
     }



     private void OnReceive(IAsyncResult ar)
     {
         Debug.Log("Beginning to recieve data");
         object[] state = (object[])ar.AsyncState;



         int bytesRead = client.EndReceive(ar);
         Debug.Log("Reading data");
         Debug.Log(bytesRead);

         if (bytesRead > 0)
         {
             string data = Encoding.ASCII.GetString(buffer, 0, bytesRead);
             int numObjects = int.Parse(data);

             Debug.Log("Received number of objects: " + numObjects);

             // Spawn the desired number of objects in Unity
             for (int i = 0; i < numObjects; i++)
             {
                 Vector3 randomSpawnLocation = new Vector3(1, 5, 1);
                 Instantiate(GameObjectPrefab[0], randomSpawnLocation, Quaternion.identity);
             }
         }
         else
         {
             // Connection closed
             client.Close(); 
         }
     }
 */




    /*    private WebSocket wsServer;

        private void Start()
        {
            // Create and start the WebSocket server
            wsServer = new WebSocket(IPAddress.Any, 8080);
            wsServer.Start();

            // Hook up the event for when a message is received
            wsServer.OnMessage += OnMessageReceived;
        }

        private void OnMessageReceived(object sender, MessageEventArgs e)
        {
            // Handle the received message
            string message = e.Data;
            Debug.Log("Received message: " + message);

            // Implement your logic to spawn objects or perform actions based on the message
            // ...
        }

        private void OnDestroy()
        {
            // Stop the WebSocket server when the script is destroyed
            if (wsServer != null && wsServer.IsListening)
                wsServer.Stop();
        }*/
}






/*
public class WebSocketServer : MonoBehaviour
{
    private WebSocket websocket;

    private void Start()
    {
        string serverUrl = "ws://localhost:34999"; // Change to the appropriate server URL

        websocket = new WebSocket(serverUrl);
        websocket.Opened += OnWebSocketOpened;
        websocket.Error += OnWebSocketError;
        websocket.Closed += OnWebSocketClosed;
        websocket.MessageReceived += OnWebSocketMessageReceived;

        websocket.Open();
    }

    private void OnWebSocketOpened(object sender, EventArgs e)
    {
        Debug.Log("WebSocket connection opened");
    }

    private void OnWebSocketError(object sender, ErrorEventArgs e)
    {
        Debug.LogError("WebSocket error: " + e.Exception.Message);
    }

    private void OnWebSocketClosed(object sender, EventArgs e)
    {
        Debug.Log("WebSocket connection closed");
    }

    private void OnWebSocketMessageReceived(object sender, MessageReceivedEventArgs e)
    {
        // Handle the received message
        string message = e.Message;
        Debug.Log("Received message: " + message);

        // Implement your logic to spawn objects or perform actions based on the message
        // ...
    }

    private void OnDestroy()
    {
        if (websocket != null && websocket.State == WebSocketState.Open)
            websocket.Close();
    }}*/

