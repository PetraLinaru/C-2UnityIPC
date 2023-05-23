// Include the necessary headers
#include <iostream>
#include <WinSock2.h>
#include <WS2tcpip.h>
#include<string>

#pragma comment(lib, "ws2_32.lib")

int main()
{
    // Initialize Winsock

    //Structure to hold info about WinSocket
    WSADATA wsData;

    //Initialize 0x0202 by using makeword(2,2)
    //Send a pointer to wsData's memory location
    //WSAStartup initializes the library with the version and the info about Winsocket^^
    //The WSAStartup function initiates use of the Winsock DLL by a process.

    if (WSAStartup(MAKEWORD(2, 2), &wsData) != 0)
    {
        std::cerr << "Failed to initialize Winsock" << std::endl;
        return 1;
    }

    // Create a socket with  s = socket(domain, type, protocol);
    //AF_INET stands for IPV4, indicates IPV4 domain
    //SOCK_STREAM provides sequenced, two-way byte streams with a transmission mechanism for stream data , TCP
    //protocol is unspecified (a value of 0) so the system selects a protocol that supports the requested socket type
    SOCKET clientSocket = socket(AF_INET, SOCK_STREAM, 0);
    if (clientSocket == INVALID_SOCKET)
    {
        std::cerr << "Failed to create socket" << std::endl;
        WSACleanup();
        return 1;
    }

    // Set up the server address
    // htons(38000) converts the decimal value 38000 to its equivalent in network byte order
    sockaddr_in serverAddress{};
    serverAddress.sin_family = AF_INET;
    serverAddress.sin_port = htons(9999); // Use the same port number as in the Unity side

    // inet_pton is used convert the IP address string to binary format
    if (inet_pton(AF_INET, "127.0.0.1", &(serverAddress.sin_addr)) != 1)
    {
        std::cerr << "Failed to convert IP address" << std::endl;
        closesocket(clientSocket);
        WSACleanup();
        return 1;
    }

    // Connect to the server
    //attempt to establish a connection to the server using the created socket and the server address
    if (connect(clientSocket, (sockaddr*)&serverAddress, sizeof(serverAddress)) == SOCKET_ERROR)
    {
        std::cerr << "Failed to connect to the server" << std::endl;
        closesocket(clientSocket);
        WSACleanup();
        return 1;
    }



    // Send number of objects to be spawn to the server until 'Q' is entered
    std::string input;
    while (true)
    {
        std::cout << "Enter a number (or 'Q' to quit): ";
        std::cin >> input;

        if (input == "Q" || input == "q")
        {
            std::cout << "Well that's all folks! Thanks, BYEEE!!!!!";
            break;
        }

        int numObjects = std::stoi(input);
        std::string data = std::to_string(numObjects);

        if (send(clientSocket, data.c_str(), data.size(), 0) == SOCKET_ERROR)
        {
            std::cerr << "Failed to send data" << std::endl;
            closesocket(clientSocket);
            WSACleanup();
            return 1;
        }

        std::cout << "Sent data: " << numObjects << ", " << data.size() << std::endl;
    }

    // Close the socket and clean up Winsock
    closesocket(clientSocket);
    WSACleanup();

    return 0;
}

