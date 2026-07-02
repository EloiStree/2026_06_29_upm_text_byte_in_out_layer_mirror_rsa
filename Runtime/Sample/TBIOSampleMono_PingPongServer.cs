using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Eloi.TBIO
{
    public class TBIOSampleMono_PingPongServer : MonoBehaviour
    {

        public UnityEvent<string> m_onTextForClients;
        public UnityEvent<byte[]> m_onByteForClients;
        public UnityEvent<string,string> m_onTextForRsaClient;
        public UnityEvent<string, byte[]> m_onByteForRsaClient;



        public IEnumerator Start()
        {
            while (true)
            {
                m_onTextForClients.Invoke("START");
                for (int i = 0; i < 10; i++) { 
                    m_onTextForClients.Invoke("TICK "+ ( (int) (Time.realtimeSinceStartup) ));
                    yield return new WaitForSeconds(1f);
                }
                yield return new WaitForSeconds(0.05f);
            }
        }


        public void ReceivedFromClientText(string publicKey,string text)
        {
            if (text == "PING")
            {
                m_onTextForRsaClient.Invoke(publicKey,"PONG "+publicKey);
                Debug.Log("PONG...", this);
            }
        }
        public void ReceivedFromClientByte(string publicKey, byte[] data) {         
            if (data != null) {
                m_lastDataReceivedFromClient = data;
                if (data.Length == 4) {
                    m_lastIntegerReceivedFromClient = BitConverter.ToInt32(data, 0);
                }
            }
        }

        public int m_lastIntegerReceivedFromClient = 0;
        public byte[] m_lastDataReceivedFromClient;
    }
}
