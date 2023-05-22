// Include the necessary headers
#include <iostream>
#include <WinSock2.h>
#include <WS2tcpip.h>
#include<string>

#pragma comment(lib, "ws2_32.lib")

int main()
{
    // Initialize Winsock
    WSADATA wsData;
    if (WSAStartup(MAKEWORD(2, 2), &wsData) != 0)
    {
        std::cerr << "Failed to initialize Winsock" << std::endl;
        return 1;
    }

    // Create a socket
    SOCKET clientSocket = socket(AF_INET, SOCK_STREAM, 0);
    if (clientSocket == INVALID_SOCKET)
    {
        std::cerr << "Failed to create socket" << std::endl;
        WSACleanup();
        return 1;
    }

    // Set up the server address
    sockaddr_in serverAddress{};
    serverAddress.sin_family = AF_INET;
    serverAddress.sin_port = htons(9999); // Use the same port number as in the Unity side

    // Convert the IP address string to binary format
    if (inet_pton(AF_INET, "127.0.0.1", &(serverAddress.sin_addr)) != 1)
    {
        std::cerr << "Failed to convert IP address" << std::endl;
        closesocket(clientSocket);
        WSACleanup();
        return 1;
    }

    // Connect to the server
    if (connect(clientSocket, (sockaddr*)&serverAddress, sizeof(serverAddress)) == SOCKET_ERROR)
    {
        std::cerr << "Failed to connect to the server" << std::endl;
        closesocket(clientSocket);
        WSACleanup();
        return 1;
    }

    // Send the number of objects to be spawned
    int numObjects = 10; // Example number of objects to send
    std::string data =std::to_string( numObjects);
    if (send(clientSocket, data.c_str(), data.size(), 0) == SOCKET_ERROR)
    {
        std::cerr << "Failed to send data" << std::endl;
        closesocket(clientSocket);
        WSACleanup();
        return 1;
    }
    std::cerr << "Sent data:" << numObjects << " , " << data.size();

    // Close the socket and clean up Winsock
    closesocket(clientSocket);
    WSACleanup();

    return 0;
}
