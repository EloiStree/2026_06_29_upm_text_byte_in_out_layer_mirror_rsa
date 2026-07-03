using UnityEngine;

namespace Eloi.TBIO
{
   
        public class TBIOMono_DebugReceivedInFromServer : MonoBehaviour
        {
            public TBIOMono_PlayerInOut m_playerInOut;

            public ulong m_totalCharReceived = 0;
            public ulong m_totalByteReceived = 0;

            public int m_textReceivedSize = 0;
            public string m_textReceived = "";

            public int m_byteReceivedSize = 0;
            public byte[] m_byteReceived;

            public void OnEnable()
            {
                m_playerInOut.m_onByteReceivedFromServerToClient.AddListener(SentBytes);
                m_playerInOut.m_onTextReceivedFromServerToClient.AddListener(SentText);
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

                m_totalCharReceived += (ulong)arg0.Length;
                m_textReceivedSize = arg0.Length;
                m_textReceived = arg0;
            }

            private void SentBytes(byte[] arg0)
            {
                if (arg0 == null)
                    return;

                m_totalByteReceived += (ulong)arg0.Length;
                m_byteReceivedSize = arg0.Length;
                m_byteReceived = arg0;

            }
        }
    
}
