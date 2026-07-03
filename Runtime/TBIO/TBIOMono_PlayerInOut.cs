using UnityEngine;
using Mirror;
using UnityEngine.Events;
using System;
using System.Collections.Generic;

namespace Eloi.TBIO
{

    public class TBIOMono_PlayerInOut : NetworkBehaviour
    {
        #region USE LOCAL PLAYER TO SERVER
        static TBIOMono_PlayerInOut m_localPlayer;
        public static void GetLocalPlayer(out TBIOMono_PlayerInOut localPlayer)
        {
            localPlayer = m_localPlayer;
        }
        public static TBIOMono_PlayerInOut GetLocalPlayer()
        {
            return m_localPlayer;
        }
        public static void SendTextToServerFromLocalPlayer(string text)
        {
            //if (m_localPlayer == null) return;
            m_localPlayer.SendTextToServer(text);
        }
        public static void SendBytesToServerFromLocalPlayer(byte[] data)
        {
            //if (m_localPlayer == null) return;
            m_localPlayer.SendByteToServer(data);
        }
        #endregion



        #region HOOK ACTION FOR SERVER TO HOOK AT
        static Action<TBIOMono_PlayerInOut, byte[]> m_onAnyPlayerBytesForServer;
        static Action<TBIOMono_PlayerInOut, string> m_onAnyPlayerTextForServer;

        public static void AddPlayersInOutTextListener(Action<TBIOMono_PlayerInOut, string> onAnyPlayerTextForServer)
        {
            m_onAnyPlayerTextForServer += onAnyPlayerTextForServer;
        }
        public static void AddPlayerInOutByteListener(Action<TBIOMono_PlayerInOut, byte[]> onAnyPlayerBytesForServer)
        {
            m_onAnyPlayerBytesForServer += onAnyPlayerBytesForServer;
        }

        public static void RemovePlayersInOutTextListener(Action<TBIOMono_PlayerInOut, string> onAnyPlayerTextForServer)
        {
            m_onAnyPlayerTextForServer -= onAnyPlayerTextForServer;
        }
        public static void RemovePlayerInOutByteListener(Action<TBIOMono_PlayerInOut, byte[]> onAnyPlayerBytesForServer)
        {
            m_onAnyPlayerBytesForServer -= onAnyPlayerBytesForServer;
        }

        #endregion

        #region PLAYER LIST IN GAME
        static List<TBIOMono_PlayerInOut> m_players = new List<TBIOMono_PlayerInOut>();
        public static void GetAllPlayersInOut(out List<TBIOMono_PlayerInOut> players)
        {
            players = m_players;
        }
        public static void GetPlayersByIndex(int playerIndex, out TBIOMono_PlayerInOut[] player)
        {
            player = m_players.FindAll(p => p.m_playerIndex == playerIndex).ToArray();
        }

        public static void GetPlayersByPublicKey(string asymmetricPublicKey, out TBIOMono_PlayerInOut[] player)
        {
            player = m_players.FindAll(p => p.m_asymmetricPublicKey == asymmetricPublicKey).ToArray();
        }   

        public static void GetPlayerIndexList(out List<int> playerIndexList)
        {
            List<int> list = new List<int>();
            foreach (var player in m_players)
            {
                list.Add(player.m_playerIndex);
            }
            playerIndexList = list;
        }
        public static void GetPlayerPublicKeyList(out List<string> playerPublicKeyList)
        {
            List<string> list = new List<string>();
            foreach (var player in m_players)
            {
                list.Add(player.m_asymmetricPublicKey);
            }
            playerPublicKeyList = list;
        }
        #endregion


        #region SEND DATA FROM SERVER TO CLIENT
        public static void ServerOnlySendTextToAllPlayer(string text)
        {
            foreach (var player in m_players)
            {
                if (player == null) continue;
                player.SendTextToThisPlayer(text);
            }
        }
        public static void ServerOnlySendByteToAllPlayer(byte[] data)
        {
            foreach (var player in m_players)
            {
                if (player == null) continue;
                player.SendByteToThisPlayer(data);
            }
        }

        public static void ServerOnlySendTextToPlayerByIndex(int playerIndex, string text)
        {       
            GetPlayersByIndex(playerIndex, out TBIOMono_PlayerInOut[] players);
            if (players == null || players.Length == 0) return;
            foreach (var player in players)
            {
                player.SendTextToThisPlayer(text);
            }
        }

        public static void ServerOnlySendByteToPlayerByIndex(int playerIndex, byte[] data)
        {
            GetPlayersByIndex(playerIndex, out TBIOMono_PlayerInOut[] players);
            if (players == null || players.Length == 0) return;
            foreach (var player in players)
            {
                player.SendByteToThisPlayer(data);
            }
        }

        public static void ServerOnlySendTextToPlayerByPublicKey(string publicKey, string text)
        {
            GetPlayersByPublicKey(publicKey, out TBIOMono_PlayerInOut[] players);
            if (players == null || players.Length == 0) return;
            foreach (var player in players)
            {
                player.SendTextToThisPlayer(text);
            }
        }

        public static void ServerOnlySendByteToPlayerByPublicKey(string publicKey, byte[] data)
        {
            GetPlayersByPublicKey(publicKey, out TBIOMono_PlayerInOut[] players);
            if (players == null || players.Length == 0) return;
            foreach (var player in players) 
                {
                player.SendByteToThisPlayer(data);
            }
        }
        #endregion




        public static void IsOnServer(out bool isOnServer)
        {
            isOnServer = NetworkServer.active;
        }
        public static bool IsOnServer()
        {
            return NetworkServer.active;
        }



        [SerializeField]
        [SyncVar]
        [Tooltip("UInt ID Given by the Mirror Network at creation")]
        uint m_playerNetworkIndex = 0;

        [SerializeField][SyncVar(hook = nameof(HookPlayerIndexChanged))]
        [Tooltip("Int ID to be set by developer based on the public key")]
        int m_playerIndex = 0;

        [SerializeField][SyncVar(hook = nameof(HookAsymmetricPublicKeyChanged))]
        [Tooltip("RSA Public Key set by the developer if public key is signed and validated")]
        string m_asymmetricPublicKey = "";


        [Tooltip("Give a random negative number to player to be override by developer")]
        public bool m_giveRandomPlayerIndexOnStart = true;


        [Header("For Server")]
        public UnityEvent<byte[]> m_onByteReceivedFromClientToServer;
        public UnityEvent<string> m_onTextReceivedFromClientToServer;

        [Header("For Client")]
        public UnityEvent<byte[]> m_onByteReceivedFromServerToClient;
        public UnityEvent<string> m_onTextReceivedFromServerToClient;


        public void Awake()
        {
            m_onByteReceivedFromClientToServer.AddListener((data) =>
            {
                m_onAnyPlayerBytesForServer?.Invoke(this, data);
            });
            m_onTextReceivedFromClientToServer.AddListener((text) =>
            {
                m_onAnyPlayerTextForServer?.Invoke(this, text);
            });

        }

        private void OnEnable()
        {
            m_players.Add(this);
        }

        public void OnDisable()
        {
            m_players.Remove(this);
        }

        public void GetPlayerIndex(out int playerIndexGivenByServer)
        {
            playerIndexGivenByServer = m_playerIndex;
        }
        public int GetPlayerIndex()
        {
            return m_playerIndex;
        }
        public void GetAsymmetricPublicKey(out string asymmetricPublicKey)
        {
            asymmetricPublicKey = m_asymmetricPublicKey;
        }
        public string GetAsymmetricPublicKey()
        {
            return m_asymmetricPublicKey;
        }

        public void HookPlayerIndexChanged(int oldValue, int newValue)
        {
            if (m_useDebugLog)
                Debug.Log($"Player index changed from {oldValue} to {newValue}", this);
        }

        public void HookAsymmetricPublicKeyChanged(string oldValue, string newValue)
        {
            if (m_useDebugLog)
                Debug.Log($"AsymmetricPublicKey public key changed from\n\n {oldValue} \n\nto\n\n {newValue}\n\n",this);
        }

        [Server]
        public void SetServerOnlyPlayerIndex(int index)
        {
            m_playerIndex = index;
        }
        [Server]
        public void SetServerOnlyAsymmetricPublicKey(string asymmetricPublicKey)
        {
            m_asymmetricPublicKey = asymmetricPublicKey ;
        }

        [ClientRpc][Server]
        public void RpcSendBytesServerToAllPlayer(byte[] data)
        {
            m_onByteReceivedFromServerToClient?.Invoke(data);
        }

        [ClientRpc][Server]
        public void RpcSendTextServerToAllPlayer(string text)
        {
            m_onTextReceivedFromServerToClient?.Invoke(text);
        }


        [Server]
        public void SendByteToThisPlayer(byte[] data)
        {
            RpcSendBytesServerToPlayer(connectionToClient, data);
        }
        [Server]
        public void SendTextToThisPlayer(string text)
        {
            RpcSendTextServerToPlayer(connectionToClient, text);
        }



        [TargetRpc][Server]
        public void RpcSendBytesServerToPlayer(NetworkConnectionToClient target ,byte[] data)
        {
            m_onByteReceivedFromServerToClient?.Invoke(data);
        }

        [TargetRpc][Server]
        public void RpcSendTextServerToPlayer(NetworkConnectionToClient target, string text)
        {
            m_onTextReceivedFromServerToClient?.Invoke(text);
        }




        #region CLIENT TO SERVER
        [Command] 
        void CmdSendBytesClientToServer(byte[] data)
        {
            if (data == null) return;
            m_onByteReceivedFromClientToServer?.Invoke(data);
        }

        [Command] 
        void CmdSendTextClientToServer(string text)
        {
            if (text == null) return;
            m_onTextReceivedFromClientToServer?.Invoke(text);
        }
        public bool IsOwnedByThisClient()
        {
            return netIdentity != null && netIdentity.isLocalPlayer;
        }

        public void SendByteToServer(byte[] data)
        {
            if (IsOwnedByThisClient()) return;
            CmdSendBytesClientToServer(data);
        }
        public void SendTextToServer(string text)
        {
            if (IsOwnedByThisClient()) return;
            CmdSendTextClientToServer(text);
        }
        #endregion






        #region MIRROR CALLBACKS

        public bool m_useDebugLog = true;
        public override void OnStartServer()
        {
            base.OnStartServer();
            if (m_useDebugLog)
                Debug.Log("Server started", this);
            if (m_giveRandomPlayerIndexOnStart)
            {
                m_playerIndex = UnityEngine.Random.Range(-int.MaxValue, 0);
                if (m_useDebugLog)
                    Debug.Log($"Giving random player index: {m_playerIndex}", this);
            }
            m_playerNetworkIndex = this.netIdentity.netId;
        }

        public override void OnStopServer()
        {
            base.OnStopServer();
            if (m_useDebugLog)
                Debug.Log($"Server stopped player previous index: {m_playerIndex}", this);
        }

        public override void OnStartClient()
        {
            base.OnStartClient();
            if (m_useDebugLog)
                Debug.Log("A new player has joined the game!", this);

            if (netIdentity.isLocalPlayer)
                m_localPlayer = this;

        }
        public override void OnStopClient()
        {
            base.OnStopClient();
            if (m_useDebugLog)
                Debug.Log("A player has left the game.", this);
        }
        #endregion

    }
}
