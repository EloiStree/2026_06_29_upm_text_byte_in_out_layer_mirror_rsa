using System;
using System.Collections.Generic;
using UnityEngine;

namespace Eloi.TBIO
{
    public class TBIOMono_ServerInOutReceivedDebug: MonoBehaviour
    {
        public TBIOMono_ServerInOut m_toListenTo;
      
        public ForDebug m_forDebug;
        [Serializable]
        public class ForDebug
        {
            public string m_lastPlayerTextReceived = "";
            public byte[] m_lastPlayerByteReceived = new byte[0];
            public TBIOMono_PlayerInOut m_lastPlayerReceivedEvent;
            public List<TBIOMono_PlayerInOut> m_playersValidated = new List<TBIOMono_PlayerInOut>();
            public int[] m_playersValidatedIndex = new int[0];
            public string[] m_playersValidatedPublicKeyB58 = new string[0];
        }

        void OnEnable()
        {

            InvokeRepeating(nameof(RefreshList), 0.1f, 1f);
            m_toListenTo.m_events.m_onPlayerTextWithSource?.AddListener(OnAnyPlayerTextForServer);
            m_toListenTo.m_events.m_onPlayerByteWithSource?.AddListener(OnAnyPlayerBytesForServer);

        }

        private void OnAnyPlayerBytesForServer(TBIOMono_PlayerInOut player, byte[] byteReceived)
        {
            m_forDebug.m_lastPlayerByteReceived = byteReceived;
            m_forDebug.m_lastPlayerReceivedEvent = player;
        }

        private void OnAnyPlayerTextForServer(TBIOMono_PlayerInOut player, string text)
        {
            m_forDebug.m_lastPlayerTextReceived = text;
            m_forDebug.m_lastPlayerReceivedEvent = player;
        }

        void RefreshList()
        {
            TBIOMono_PlayerInOut.GetAllPlayersInOut(out m_forDebug.m_playersValidated);

            if (m_forDebug.m_playersValidatedIndex.Length != m_forDebug.m_playersValidated.Count)
                m_forDebug.m_playersValidatedIndex = new int[m_forDebug.m_playersValidated.Count];
            if (m_forDebug.m_playersValidatedPublicKeyB58.Length != m_forDebug.m_playersValidated.Count)
                m_forDebug.m_playersValidatedPublicKeyB58 = new string[m_forDebug.m_playersValidated.Count];

            for (int i = 0; i < m_forDebug.m_playersValidated.Count; i++)
            {
                m_forDebug.m_playersValidated[i].GetPlayerIndex(out m_forDebug.m_playersValidatedIndex[i]);
                m_forDebug.m_playersValidated[i].GetAsymmetricPublicKey(out m_forDebug.m_playersValidatedPublicKeyB58[i]);
            }
        }
    }
}
