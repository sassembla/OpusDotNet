﻿using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;
using System.Text;

public class OpusNetworked : NetworkBehaviour {
    /*[SyncVar]
    private string MySyncString; // This string will sync across the network. It's value on the server will overide the value on all clients. Therefore this variable can only be changed only by the server, but it'll be the same on all clients

    [SyncVar]
    private int MySyncInt; // This int will sync across the network. It's value on the server will overide the value on all clients. Therefore this variable can only be changed only by the server, but it'll be the same on all clients

    [SyncVar]
    private float MySyncFloat;// This float will sync across the network. It's value on the server will overide the value on all clients. Therefore this variable can only be changed only by the server, but it'll be the same on all clients

    [SyncVar]
    private bool MySyncBool;// This bool will sync across the network. It's value on the server will overide the value on all clients. Therefore this variable can only be changed only by the server, but it'll be the same on all clients

    [SyncVar(hook = "MyHookFunction")]
    private string MySyncStringWithHook; // This is a hook, and the function "MyHookFunction" will be called whenever the variable changes.


    //This function will be called when "MySyncStringWithHook" changes.
    void MyHookFunction(string hook) {
        Debug.Log("MySyncStringWithHook changed");
    }*/

    [ClientCallback]
    void Update () {
        if ((Time.frameCount % 30) == 0) { 
            Cmd_MyCommmand(System.Environment.MachineName + " - " + (Time.frameCount / 30).ToString());
        }
    }

    // how to set up a RPC
    [ClientRpc]
    void Rpc_MyRPC(string value) {
        Debug.Log(System.Environment.MachineName + " received " + value);
    }

    // How to set up a Command
    [Command]
    void Cmd_MyCommmand(string value)  {
        Rpc_MyRPC(value);
    }


    /*const short chatMsg = 1001;
    NetworkClient _client;

    [SerializeField]
    private Text chatline;
    [SerializeField]
    private SyncListStruct<byte> opusData = new SyncListStruct<byte>();
    [SerializeField]
    private InputField input;

    public void Awake () {
        string name = "";
        if (isLocalPlayer) {
            name = System.Environment.MachineName;
            Debug.Log("OpusNetworked.Awake: local name " + name);
        }
        else {
            Debug.Log("OpusNetworked.Awake: non local name " + name);
        }
    }

    public override void OnStartClient() {
        opusData.Callback = OnOpusUpdated;
    }

    public void Start() {
        _client = NetworkManager.singleton.client;
        NetworkServer.RegisterHandler(opusData, OnServerPostOpusMessage);
        input.onEndEdit.AddListener(delegate { PostChatMessage(input.text); });
    }

    [Client]
    public void PostChatMessage(string message) {
        if (message.Length == 0) return;
        var msg = new StringMessage(message);
        _client.Send(chatMsg, msg);

        input.text = "";
        input.ActivateInputField();
        input.Select();
    }

    [Server]
    void OnServerPostOpusMessage(NetworkMessage netMsg) {
        //string message = netMsg.ReadMessage<StringMessage>().value;
        netMessage = netMsg.ReadMessage<byte>()
        byte[] toBytes = Encoding.ASCII.GetBytes();
        foreach (byte b in toBytes) { 
            opusData.Add(b);
        }
    }

    private void OnOpusUpdated(SyncList<byte>.Operation op, int index) {
        //chatline.text += chatLog[chatLog.Count - 1] + "\n";
        byte b = opusData[opusData.Count - 1];
    }*/
}