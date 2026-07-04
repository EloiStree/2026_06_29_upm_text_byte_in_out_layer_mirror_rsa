using Mirror.BouncyCastle.Asn1.Mozilla;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Eloi.TBIO
{
    public class TBIOMono_ServerStatePusher : MonoBehaviour
    {
        public UnityEvent<byte[]> m_onGameStateAsByteForClients;
        public UnityEvent<string> m_onGameStateAsTextForClients;
        [Header("Game State")]
        public byte[] m_gameStateAsBytes;
        [TextArea(3, 10)]
        public string m_gameStateAsString;

        [Header("Auto Push Timer")]
        public bool m_useAutoPush = true;
        public float m_timeBetweenPush = 1f;

        private void OnEnable()
        {
                StartCoroutine(AutoPush());
        }

         IEnumerator AutoPush()
        {
            while (true)
            {
                if (m_useAutoPush)
                {
                    yield return new WaitForSeconds(m_timeBetweenPush);
                    if (m_gameStateAsBytes != null && m_gameStateAsBytes.Length > 0)
                        m_onGameStateAsByteForClients.Invoke(m_gameStateAsBytes);

                    if (m_gameStateAsString != null && m_gameStateAsString.Length > 0)
                        m_onGameStateAsTextForClients.Invoke(m_gameStateAsString);
                }
                else { 
                    yield return new WaitForSeconds(0.1f);
                }
            }
        }

    }
}
