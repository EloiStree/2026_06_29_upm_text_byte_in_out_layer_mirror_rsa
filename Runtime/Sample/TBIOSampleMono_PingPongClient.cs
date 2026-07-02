using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Eloi.TBIO
{
    

    public class TBIOSampleMono_PingPongClient : MonoBehaviour
    {


        public UnityEvent<string > m_textForServer;
        public UnityEvent<byte[]> m_byteForServer;

        public AudioSource m_startSound;

        public string m_startReceivedDate;
        public string m_pongReceivedDate;
        public int m_integerReceived;


        public float m_timeBetweenPing = 1f;


        public int m_lastSendInteger;

        private IEnumerator Start()
        {
            while (true)
            {
                m_textForServer.Invoke("PING");
                int randomInteger = UnityEngine.Random.Range(1800000000, 1900000000);
                m_lastSendInteger= randomInteger;
                m_byteForServer.Invoke(BitConverter.GetBytes(randomInteger));
                yield return new WaitForSeconds(m_timeBetweenPing);
                yield return new WaitForSeconds(0.05f);
            }
        }

        public void ReceivedFromServerBytes(byte[] bytes)
        {
            if(m_integerReceived==4)
            { 
                m_integerReceived = BitConverter.ToInt32(bytes, 0);
                Debug.Log("INT:"+m_integerReceived, this);
            }

        }
        public void ReceivedFromServerText(string text)
        {
            if (text == "START")
            {

                m_startReceivedDate = "" + DateTime.Now.ToString("HH:mm:ss");
                Debug.Log("START...", this);
                if (m_startSound)
                    m_startSound.Play();
            }
            if (text.StartsWith("PONG "))
            {
                m_pongReceivedDate = "" + DateTime.Now.ToString("HH:mm:ss");
                Debug.Log("PONG...", this);
            }
        }
    }
}
