
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEditor.PackageManager;
using UnityEngine;


public class Spawner : MonoBehaviour
{


    /*  private Socket _clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
      private byte[] _recieveBuffer = new byte[8142];

      private void SetupServer()
      {
          try
          {
              _clientSocket.Connect(new IPEndPoint(IPAddress.Loopback, 39000));
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

          if (recieved <= 0)
              return;

          Debug.Log("Recieved data!" + recieved);

          //Copy the recieved data into new buffer , to avoid null bytes
          byte[] recData = new byte[recieved];
          Buffer.BlockCopy(_recieveBuffer, 0, recData, 0, recieved);

          //Process data here the way you want , all your bytes will be stored in recData

          //Start receiving again
          _clientSocket.BeginReceive(_recieveBuffer, 0, _recieveBuffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), null);
      }*/
    /*    private const int bufferSize = 1024;
        private const int port = 38000;

        private Socket listener;
        private byte[] buffer = new byte[bufferSize];

        public GameObject[] GameObjectPrefab;
        Socket client;
        private void Start()
        {
            // Start listening for data
            client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            client.Connect("localhost", 1234); // Change the IP address and port number as needed
            Debug.Log("Client connected");
            byte[] buffer = new byte[1024];
            client.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(OnReceive),null);
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


        public void Update()
        {
            client.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, OnReceive, new object[] { client, buffer });
        }


    */
    Socket SeverSocket = null;
    Thread Socket_Thread = null;
    bool Socket_Thread_Flag = false;

    //for received message
    //    private float mouse_delta_x;
    //    private float mouse_delta_y;
    //    private bool isTapped;
    //    private bool isDoubleTapped;
    //
    //    public float getMouseDeltaX(){return mouse_delta_x;    }
    //    public float getMouseDeltaY(){return mouse_delta_y;    }
    //    public bool getTapped(){return isTapped;}
    //    public bool getDoubleTapped(){return isDoubleTapped;}
    //
    //    public void setMouseDeltaX(float dx){mouse_delta_x = dx;}
    //    public void setMouseDeltaY(float dy){mouse_delta_y = dy;}
    //    public void setTapped(bool t){isTapped = t;}
    //    public void setDoubleTapped(bool t){isDoubleTapped = t;}
    //
    //    private int tick =0;
    //private string[] receivedMSG;
    //public string[] getMsg(){return receivedMSG;    }


    string[] stringSeparators = new string[] { "*TOUCHEND*", "*MOUSEDELTA*", "*Tapped*", "*DoubleTapped*" };

    void Awake()
    {
        Socket_Thread = new Thread(Dowrk);
        Socket_Thread_Flag = true;
        Socket_Thread.Start();
    }

    private void Dowrk()
    {
        //receivedMSG = new string[10];
        SeverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        IPEndPoint ipep = new IPEndPoint(IPAddress.Any, 9999);
        SeverSocket.Bind(ipep);
        SeverSocket.Listen(10);

        Debug.Log("Socket Standby....");
        Socket client = SeverSocket.Accept();//client?? ??? ???? ?????.
        Debug.Log("Socket Connected.");

        IPEndPoint clientep = (IPEndPoint)client.RemoteEndPoint;
        NetworkStream recvStm = new NetworkStream(client);
        //tick = 0;

        while (Socket_Thread_Flag)
        {
            byte[] receiveBuffer = new byte[1024 * 80];
            try
            {

                //print (recvStm.Read(receiveBuffer, 0, receiveBuffer.Length));
                if (recvStm.Read(receiveBuffer, 0, receiveBuffer.Length) == 0)
                {
                    // when disconnected , wait for new connection.
                    client.Close();
                    SeverSocket.Close();

                    SeverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    ipep = new IPEndPoint(IPAddress.Any, 10000);
                    SeverSocket.Bind(ipep);
                    SeverSocket.Listen(10);
                    Debug.Log("Socket Standby....");
                    client = SeverSocket.Accept();//client?? ??? ???? ?????.
                    Debug.Log("Socket Connected.");

                    clientep = (IPEndPoint)client.RemoteEndPoint;
                    recvStm = new NetworkStream(client);

                }
                else
                {


                    string Test = Encoding.Default.GetString(receiveBuffer);
                    //string Test = Convert.ToBase64String(receiveBuffer);
                    //Test = Test.Normalize();


                    print(Test);
                    //string[] splitMsg = Test.Split(stringSeparators,System.StringSplitOptions.RemoveEmptyEntries);
                    // parsing gogo

                    //                    string[] splitMsg = Test.Split('*');
                    ////                    print (splitMsg);
                    //                    if(splitMsg.Length>1)
                    //                    {
                    //                        if(splitMsg[1].CompareTo("Tapped")==0){
                    //                            print ("tap");
                    //                            isTapped = true;
                    //                        }else if(splitMsg[1].CompareTo("DoubleTapped")==0){
                    //                            print ("double tap");
                    //                            isDoubleTapped = true;
                    //                        }else if(splitMsg[1].CompareTo("MOUSEDELTA")==0){
                    //                            print ("move");
                    //                            //string[] lastMsg = splitMsg[splitMsg.Length-1].Split('*');
                    //                            mouse_delta_x = (float)Convert.ToDouble(splitMsg[2]);
                    //                            mouse_delta_y = (float)Convert.ToDouble(splitMsg[3]);
                    //                        }else{
                    //                            print ("F*** :"+splitMsg[1].Length);
                    //                          
                    //                        }
                    //                    }
                    //
                    //                    string singletap = "one";
                    //                    string doubletap = "two";
                    //                    if(splitMsg.Length>0){
                    //
                    //

                    //                          
                    //                        if(lastMsg.Length>1){
                    //                      

                    //
                    //                        }else{
                    //
                    //                            print ("split msg : "+splitMsg[0]);
                    //                            int tmp = (int)Convert.ToInt32(splitMsg[0]);
                    //                            if(tmp ==1){
                    //
                    //                                print ("Tapped!~");
                    //                          
                    //                                isTapped = true;
                    //
                    //                            }else if(tmp ==2){
                    //
                    //                                print ("Double Tapped!~");
                    //                          
                    //                                isDoubleTapped = true;
                    //                          
                    //                            }else{              
                    //
                    //                            }
                    //                        }
                    //                    }else{
                    //
                    //                    }

                    //print (receivedMSG);

                }


            }

            catch (Exception e)
            {
                Socket_Thread_Flag = false;
                client.Close();
                Debug.Log(e);
                SeverSocket.Close();
                continue;
            }

        }

    }

    void OnApplicationQuit()
    {
        try
        {
            Socket_Thread_Flag = false;
            Socket_Thread.Abort();
            SeverSocket.Close();
            Debug.Log("Bye~~");
        }

        catch
        {
            Debug.Log("Error when finished...");
        }
    }

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

