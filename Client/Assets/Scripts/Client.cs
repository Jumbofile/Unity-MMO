using UnityEngine;
using System.Collections;
using System.Net.Sockets;
using System.IO;
using System;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Globalization;
using UnityEngine.AI;

public class Client : MonoBehaviour
{
    private string clientName;
    private int portToConnect = 6321;
    private string password;
    private bool socketReady;
    private TcpClient socket;
    private NetworkStream stream;
    private StreamWriter writer;
    private StreamReader reader;
    public InputField clientNameInputField;
    public InputField serverAddressInputField;
    public InputField passwordInputField;
    private List<Unit> unitsOnMap = new List<Unit>();
    private CultureInfo culture = (CultureInfo)CultureInfo.CurrentCulture.Clone();

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
        culture.NumberFormat.NumberDecimalSeparator = ".";
    }

    public bool ConnectToServer(string host, int port)
    {
        if (socketReady)
            return false;

        try
        {
            socket = new TcpClient(host, port);
            stream = socket.GetStream();
            writer = new StreamWriter(stream);
            reader = new StreamReader(stream);

            socketReady = true;
        }
        catch (Exception e)
        {
            Debug.Log("Socket error " + e.Message);
        }

        return socketReady;
    }

    private void Update()
    {
        if (socketReady)
        {
            if (stream.DataAvailable)
            {
                string data = reader.ReadLine();
                if (data != null)
                    OnIncomingData(data);
            }
        }
    }

    // Sending message to the server
    public void Send(string data)
    {
        if (!socketReady)
            return;

        writer.WriteLine(data);
        writer.Flush();
    }

    // Read messages from the server
    private void OnIncomingData(string data)
    {
        string[] aData = data.Split('|');
        Debug.Log("Received from server: " + data);

        switch (aData[0])
        {
            case "WhoAreYou":
                Send("Iam|" + clientName + "|" + password);
                break;
            case "Authenticated":
                SceneManager.LoadScene("ClientGameView");
                break;
            case "UnitSpawned":
                GameObject prefab = Resources.Load("Prefabs/Unit1") as GameObject;
                GameObject go = Instantiate(prefab);
                float parsedX = float.Parse(aData[3], culture);
                float parsedY = float.Parse(aData[4], culture);
                float parsedZ = float.Parse(aData[5], culture);
                go.GetComponent<NavMeshAgent>().Warp(new Vector3(parsedX, parsedY, parsedZ));
                Unit un = go.AddComponent<Unit>();
                unitsOnMap.Add(un);
                int parsed;
                Int32.TryParse(aData[2], out parsed);
                un.unitID = parsed;

                if (aData[1] == clientName)
                {
                    un.isPlayersUnit = true;
                }
                else
                {
                    un.isPlayersUnit = false;
                }
                break;
            case "UnitMoved":
                if (aData[1] == clientName)
                {
                    return;
                }
                else
                {
                    Int32.TryParse(aData[2], out parsed);
                    foreach (Unit unit in unitsOnMap)
                    {
                        if (unit.unitID == parsed) {
                            parsedX = float.Parse(aData[3], culture);
                            parsedY = float.Parse(aData[4], culture);
                            parsedZ = float.Parse(aData[5], culture);
                            unit.MoveTo(new Vector3(parsedX,parsedY,parsedZ));
                        }
                    }
                }
                break;
            case "Synchronizing":
                int numberOfUnitsOnServersMap;
                Int32.TryParse(aData[1], out numberOfUnitsOnServersMap);
                int serverUnitID;
                int[] serverUnitIDs = new int[numberOfUnitsOnServersMap];
                for (int i = 0; i < numberOfUnitsOnServersMap; i++)
                {
                    Int32.TryParse(aData[2+i*4], out serverUnitID);
                    serverUnitIDs[i] = serverUnitID;
                    bool didFind = false;
                    foreach (Unit unit in unitsOnMap) //synchronize existing units
                    {
                        if (unit.unitID == serverUnitID)
                        {
                            parsedX = float.Parse(aData[3+i*4], culture);
                            parsedY = float.Parse(aData[4+i*4], culture);
                            parsedZ = float.Parse(aData[5+i*4], culture);
                            unit.MoveTo(new Vector3(parsedX, parsedY, parsedZ));
                            didFind = true;
                        }
                    }
                    if (!didFind) //add non-existing (at client) units
                    {
                        prefab = Resources.Load("Prefabs/Unit1") as GameObject;
                        go = Instantiate(prefab);
                        un = go.AddComponent<Unit>();
                        unitsOnMap.Add(un);
                        un.unitID = serverUnitID;
                        parsedX = float.Parse(aData[3+i*4], culture);
                        parsedY = float.Parse(aData[4+i*4], culture);
                        parsedZ = float.Parse(aData[5+i*4], culture);
                        go.GetComponent<NavMeshAgent>().Warp(new Vector3(parsedX, parsedY, parsedZ));
                    }

                }
                //remove units which are not on server's list (like disconnected ones)
                foreach (Unit unit in unitsOnMap)
                {
                    bool exists = false;
                    for (int i = 0; i < serverUnitIDs.Length; i++)
                    {
                        if (unit.unitID == serverUnitIDs[i])
                        {
                            exists = true;
                        }
                    }
                    if (!exists)
                    {
                        Destroy(unit.gameObject);
                        unitsOnMap.Remove(unit);
                    }
                }
                break;
            default:
                Debug.Log("Unrecognizable command received");
                break;
        }
    }

 

    private void OnApplicationQuit()
    {
        CloseSocket();
    }
    private void OnDisable()
    {
        CloseSocket();
    }
    private void CloseSocket()
    {
        if (!socketReady)
            return;

        writer.Close();
        reader.Close();
        socket.Close();
        socketReady = false;
    }


    public void ConnectToServerButton()
    {
        password = passwordInputField.text;
        clientName = clientNameInputField.text;
        CloseSocket();
        try
        {
            ConnectToServer(serverAddressInputField.text, portToConnect);
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
        }
    }
}

