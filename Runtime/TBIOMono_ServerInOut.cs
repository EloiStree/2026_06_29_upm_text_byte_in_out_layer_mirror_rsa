using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace Eloi.TBIO
{
    public class TBIOMono_ServerInOut : MonoBehaviour
    {


        void OnEnable()
        {
            TBIOMono_PlayerInOut.AddPlayersInOutTextListener(OnAnyPlayerTextForServer);
            TBIOMono_PlayerInOut.AddPlayerInOutByteListener(OnAnyPlayerBytesForServer);
            InvokeRepeating(nameof(RefreshList),0.1f,1f);
        }

        void OnDisable()
        {
            TBIOMono_PlayerInOut.RemovePlayersInOutTextListener(OnAnyPlayerTextForServer);
            TBIOMono_PlayerInOut.RemovePlayerInOutByteListener(OnAnyPlayerBytesForServer);
        }


        void RefreshList() {
            Debug.Log("Refreshing player list");
            TBIOMono_PlayerInOut.GetAllPlayersInOut(out m_playersValidated);

            if (m_playersValidatedIndex.Length != m_playersValidated.Count)
                m_playersValidatedIndex = new int[m_playersValidated.Count];
            if (m_playersValidatedPublicKeyB58.Length != m_playersValidated.Count)
                m_playersValidatedPublicKeyB58 = new string[m_playersValidated.Count];

            for(int i = 0; i < m_playersValidated.Count; i++)
            {
                 m_playersValidated[i].GetPlayerIndex(out m_playersValidatedIndex[i]);
                 m_playersValidated[i].GetAsymmetricPublicKey(out m_playersValidatedPublicKeyB58[i]);
            }
        }


        public Events m_events;
        [Serializable]
        public class Events
        {
            public UnityEvent<TBIOMono_PlayerInOut, string> m_onPlayerText;
            public UnityEvent<TBIOMono_PlayerInOut, byte[]> m_onPlayerByte;
            public UnityEvent<string> m_onPlayerTextWithoutSource;
            public UnityEvent<byte[]> m_onPlayerByteWithoutSource;
        }

        [SerializeField] string m_lastPlayerTextReceived = "";
        [SerializeField] byte[] m_lastPlayerByteReceived = new byte[0];
        [SerializeField] TBIOMono_PlayerInOut m_lastPlayerReceivedEvent;
        [SerializeField] bool m_useDebugLog = true;

        [SerializeField] List<TBIOMono_PlayerInOut> m_playersValidated = new List<TBIOMono_PlayerInOut>();
        [SerializeField] int[] m_playersValidatedIndex = new int[0];
        [SerializeField] string[] m_playersValidatedPublicKeyB58 = new string[0];






        void OnAnyPlayerTextForServer(TBIOMono_PlayerInOut player, string text)
        {
            m_events.m_onPlayerText?.Invoke(player, text);
            m_events.m_onPlayerTextWithoutSource?.Invoke(text);
            m_lastPlayerTextReceived = text;
            m_lastPlayerReceivedEvent= player;
            if (m_useDebugLog)
                Debug.Log($"Server received text from player {player.name}: {text}");
        }

        void OnAnyPlayerBytesForServer(TBIOMono_PlayerInOut player, byte[] data)
        {
            m_events.m_onPlayerByte?.Invoke(player, data);
            m_events.m_onPlayerByteWithoutSource?.Invoke(data);
            m_lastPlayerByteReceived = data;
            m_lastPlayerReceivedEvent = player;
            if (m_useDebugLog)
                Debug.Log($"Server received bytes from player {player.name}: {BitConverter.ToString(data)}");
        }


        public void ServerSendTexToClients(string text)
        {
            TBIOMono_PlayerInOut.ServerOnlySendTextToAllPlayer(text);
        }
        public void ServerSendBytesToClients(byte[] data)
        {
            TBIOMono_PlayerInOut.ServerOnlySendBytesToAllPlayer(data);
        }

        public void ServerSendTexToClientByIndex(int playerIndex, string text)
        {
            TBIOMono_PlayerInOut.ServerOnlySendTextToPlayerIndex(playerIndex, text);
        }
        public void ServerSendBytesToClientByIndex(int playerIndex, byte[] data)
        {
            TBIOMono_PlayerInOut.ServerOnlySendBytesToPlayerIndex(playerIndex, data);
        }

        public void ServerSendTextToClientByPublicKeyB58(string publicKeyB58, string text)
        {
            TBIOMono_PlayerInOut.ServerOnlySendTextToPlayerPublicKey(publicKeyB58, text);
        }

        public void ServerSendBytesToClientByPublicKeyB58(string publicKeyB58, byte[] data)
        {
            TBIOMono_PlayerInOut.ServerOnlySendBytesToPlayerPublicKey(publicKeyB58, data);
        }
    }
}
