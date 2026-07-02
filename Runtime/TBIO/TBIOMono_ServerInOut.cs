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
        }

        void OnDisable()
        {
            TBIOMono_PlayerInOut.RemovePlayersInOutTextListener(OnAnyPlayerTextForServer);
            TBIOMono_PlayerInOut.RemovePlayerInOutByteListener(OnAnyPlayerBytesForServer);
        }

        public Events m_events;
        [Serializable]
        public class Events
        {
            public UnityEvent<string> m_onPlayerTextWithoutSource;
            public UnityEvent<byte[]> m_onPlayerByteWithoutSource;

            [Header("With Player Info")]
            public UnityEvent<TBIOMono_PlayerInOut, string> m_onPlayerText;
            public UnityEvent<TBIOMono_PlayerInOut, byte[]> m_onPlayerByte;
            [Header("With Player ID")]
            public UnityEvent<string, string> m_onPlayerTextWithPublicKey;
            public UnityEvent<string, byte[]> m_onPlayerByteWithPublicKey;
            public UnityEvent<int, string> m_onPlayerTextWithIndex;
            public UnityEvent<int, byte[]> m_onPlayerByteWithIndex;
        }


        void OnAnyPlayerTextForServer(TBIOMono_PlayerInOut player, string text)
        {
            m_events.m_onPlayerText?.Invoke(player, text);
            m_events.m_onPlayerTextWithoutSource?.Invoke(text);
            m_events.m_onPlayerTextWithPublicKey?.Invoke(player.GetAsymmetricPublicKey(), text);
            m_events.m_onPlayerTextWithIndex?.Invoke(player.GetPlayerIndex(), text);
        }

        void OnAnyPlayerBytesForServer(TBIOMono_PlayerInOut player, byte[] data)
        {
            m_events.m_onPlayerByte?.Invoke(player, data);
            m_events.m_onPlayerByteWithoutSource?.Invoke(data);
            m_events.m_onPlayerByteWithPublicKey?.Invoke(player.GetAsymmetricPublicKey(), data);
            m_events.m_onPlayerByteWithIndex?.Invoke(player.GetPlayerIndex(), data);

        }

        public void ServerSendTextToClients(string text)
        {
            TBIOMono_PlayerInOut.ServerOnlySendTextToAllPlayer(text);
        }
        public void ServerSendBytesToClients(byte[] data)
        {
            TBIOMono_PlayerInOut.ServerOnlySendBytesToAllPlayer(data);
        }

        public void ServerSendTexToClientByIndex(int playerIndex, string text)
        {
            TBIOMono_PlayerInOut.ServerOnlySendTextToPlayerByIndex(playerIndex, text);
        }
        public void ServerSendBytesToClientByIndex(int playerIndex, byte[] data)
        {
            TBIOMono_PlayerInOut.ServerOnlySendBytesToPlayerByIndex(playerIndex, data);
        }

        public void ServerSendTextToClientByPublicKey(string publicKey, string text)
        {
            TBIOMono_PlayerInOut.ServerOnlySendTextToPlayerByPublicKey(publicKey, text);
        }

        public void ServerSendBytesToClientByPublicKey(string publicKey, byte[] data)
        {
            TBIOMono_PlayerInOut.ServerOnlySendBytesToPlayerByPublicKey(publicKey, data);
        }


        #region SEND INTEGER VALUE TO CLIENTS WITH 4 BYTES
        public void ServerSendIntegerToClients(int integerValue)
        {
            byte[] data = BitConverter.GetBytes(integerValue);
            TBIOMono_PlayerInOut.ServerOnlySendBytesToAllPlayer(data);
        }
        public void ServerSendIntegerToClientByIndex(int playerIndex, int integerValue)
        {
            byte[] data = BitConverter.GetBytes(integerValue);
            TBIOMono_PlayerInOut.ServerOnlySendBytesToPlayerByIndex(playerIndex, data);
        }
        public void ServerSendIntegerToClientByPublicKey(string publicKey, int integerValue)
        {
            byte[] data = BitConverter.GetBytes(integerValue);
            TBIOMono_PlayerInOut.ServerOnlySendBytesToPlayerByPublicKey(publicKey, data);
        }
        #endregion
    }
}
