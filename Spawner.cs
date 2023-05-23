
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEditor.PackageManager;
using UnityEngine;


public class Spawner : MonoBehaviour
{
    public GameObject gameObjectPrefab;
    Socket SeverSocket = null;
    Thread Socket_Thread = null;
    bool Socket_Thread_Flag = false;


   
    int currentNumber = 0;
    int sentData = 0;
    
    //Create a thread to do work *function is Dowrk, get it ? heehe*
    //sets the thread flag to true, so that we know while the flag is true, we'll do work
    //start the thread
    void Awake()
    {
        Socket_Thread = new Thread(Dowrk);
        Socket_Thread_Flag = true;
        Socket_Thread.Start();
    }

    private void Dowrk()
    {
        //Create a socket
        SeverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        //Pin an endpoint on port 9999 (same as on C++ side)
        IPEndPoint ipep = new IPEndPoint(IPAddress.Any, 9999);
        //bind that endpoint to the socket
        SeverSocket.Bind(ipep);

        //Listening for max 10 pending requests, then start accepting
        SeverSocket.Listen(10);
       
        Debug.Log("Socket Standby....");
        Socket client = SeverSocket.Accept();//client
        Debug.Log("Socket Connected.");

        //create a client end point to recieve info from c++
        IPEndPoint clientep = (IPEndPoint)client.RemoteEndPoint;
        //create a network stream to recieve data
        NetworkStream recvStm = new NetworkStream(client);

        while (Socket_Thread_Flag)
        {
            byte[] receiveBuffer = new byte[1024 * 80];
            try
            {

                //print (recvStm.Read(receiveBuffer, 0, receiveBuffer.Length));
                //read data
                //if unavailable data/ no data sent, we wait
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
                    client = SeverSocket.Accept();//client
                    Debug.Log("Socket Connected.");

                    clientep = (IPEndPoint)client.RemoteEndPoint;
                    recvStm = new NetworkStream(client);

                }//else, we read data and update number of objs to be spawned
                else
                {


                    string Test = Encoding.Default.GetString(receiveBuffer);
                    sentData += int.Parse(Test);
                    
                    print(Test);

                }


            }

            catch (Exception e)
            {
                Socket_Thread_Flag = false;
                client.Close();
                Debug.Log(e);
                Debug.Log("Closed due to exception!!");
                SeverSocket.Close();
                continue;
            }

        }

    }


    //update is called in each frame
    //also Instantiate could be used in unity in main thread functions
    //we keep a currentNumber of objs in the scene so whenever we want more, 
    //we'll create inside the while loop
    private void Update()
    {
        while(currentNumber<sentData)
        {
            Debug.Log("here we gotta instantiate " + (sentData - currentNumber) + " more objs");
            Vector3 spawnLocation = new Vector3(UnityEngine.Random.Range(-30, 31), 5, UnityEngine.Random.Range(-22, 23));
            Instantiate(gameObjectPrefab, spawnLocation, Quaternion.identity);
            currentNumber++;
        }
    }

    //closing function
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


}
