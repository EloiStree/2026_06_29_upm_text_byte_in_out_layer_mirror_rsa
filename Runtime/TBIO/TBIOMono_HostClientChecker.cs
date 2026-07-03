using Mirror;
using UnityEngine.Events;

namespace Eloi.TBIO
{

    public class TBIOMono_HostClientChecker : NetworkBehaviour
    {
        [System.Serializable]
        public class BoolEvent : UnityEvent<bool> { }

        public BoolEvent m_onHostBool;
        public BoolEvent m_onClientBool;
        public UnityEvent m_onHost;
        public UnityEvent m_onClient;
        public bool m_isHost;
        public bool m_checkDone;

        public override void OnStartClient()
        {
            base.OnStartClient();
            m_isHost = isServer;
            m_onHostBool?.Invoke(m_isHost);
            m_onClientBool?.Invoke(!m_isHost);
            if (m_isHost)
                m_onHost?.Invoke();
            else
                m_onClient?.Invoke();
            m_checkDone = true;
        }
    }
}
