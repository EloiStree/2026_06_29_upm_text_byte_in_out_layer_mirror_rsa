using System;
using UnityEngine;

namespace Eloi.TBIO
{
    public class TBIOMono_DebugSentOutToServer : MonoBehaviour
    {
        public TBIOMono_PlayerInOut m_playerInOut;

        public ulong m_totalCharSent = 0;
        public ulong m_totalByteSent = 0;

        public int m_textSentSize = 0;
        public string m_textSent = "";

        public int m_byteSentSize = 0;
        public byte [] m_byteSent ;

        public void OnEnable()
        {
            m_playerInOut.m_onByteReceivedFromClientToServer.AddListener(SentBytes);
            m_playerInOut.m_onTextReceivedFromClientToServer.AddListener(SentText);
        }

        public void OnDisable()
        {
            m_playerInOut.m_onByteReceivedFromClientToServer.RemoveListener(SentBytes);
            m_playerInOut.m_onTextReceivedFromClientToServer.RemoveListener(SentText);
        }

        private void SentText(string arg0)
        {
            if (arg0 == null)
                return;

            m_totalCharSent += (ulong)arg0.Length;
            m_textSentSize = arg0.Length;
            m_textSent = arg0;
        }

        private void SentBytes(byte[] arg0)
        {
            if (arg0 == null)
                return;

            m_totalByteSent += (ulong)arg0.Length;
            m_byteSentSize = arg0.Length;
            m_byteSent = arg0;

        }
    }
}
