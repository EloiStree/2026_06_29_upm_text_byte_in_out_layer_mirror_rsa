using Mirror;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Eloi.TBIO
{
    public class TBIOMono_NetworkStateObserver : NetworkBehaviour
    {
        [Header("Check Interval")]
        [SerializeField] private float interval = 1f;

        [Header("Server")]
        public UnityEvent m_onServerActive;
        public UnityEvent m_onServerInactive;
        public UnityEvent<bool> m_onServerActiveStateChanged;

        [Header("Client")]
        public UnityEvent m_onClientConnected;
        public UnityEvent m_onClientDisconnected;
        public UnityEvent<bool> m_onClientConnectedStateChanged;

        [Header("Client Active")]
        public UnityEvent m_onClientActive;
        public UnityEvent m_onClientInactive;
        public UnityEvent<bool> m_onClientActiveStateChanged;

        [Header("Client Ready")]
        public UnityEvent m_onClientReady;
        public UnityEvent m_onClientNotReady;
        public UnityEvent<bool> m_onClientReadyStateChanged;


        [Header("Is Server")]
        public UnityEvent m_onReadyClientIsHost;
        public UnityEvent m_onReadyClientIsNotHost;
        public UnityEvent<bool> m_onReadyClientIsHostStateChanged;

        [SerializeField] bool m_lastServerActive;
        [SerializeField] bool m_lastClientConnected;
        [SerializeField] bool m_lastClientActive;
        [SerializeField] bool m_lastClientReady;

        private void Start()
        {
            m_lastServerActive = NetworkServer.active;
            m_lastClientConnected = NetworkClient.isConnected;
            m_lastClientActive = NetworkClient.active;
            m_lastClientReady = NetworkClient.ready;

            StartCoroutine(Observe());
        }

        private IEnumerator Observe()
        {
            while (true)
            {
                CheckState(
                    NetworkServer.active,
                    ref m_lastServerActive,
                    m_onServerActive,
                    m_onServerInactive,
                    m_onServerActiveStateChanged);

                CheckState(
                    NetworkClient.isConnected,
                    ref m_lastClientConnected,
                    m_onClientConnected,
                    m_onClientDisconnected,
                    m_onClientConnectedStateChanged);

                CheckState(
                    NetworkClient.active,
                    ref m_lastClientActive,
                    m_onClientActive,
                    m_onClientInactive,
                    m_onClientActiveStateChanged    );

                CheckState(
                    NetworkClient.ready,
                    ref m_lastClientReady,
                    m_onClientReady,
                    m_onClientNotReady,
                    m_onClientReadyStateChanged);

               

                yield return new WaitForSeconds(interval);
            }
        }

        public override void OnStartClient()
        {
            base.OnStartClient();
            bool isHost = isServer;
            CheckState(
                isHost,
                ref m_lastClientReady,
                m_onReadyClientIsHost,
                m_onReadyClientIsNotHost,
                m_onReadyClientIsHostStateChanged);

        }

        private void CheckState(
            bool current,
            ref bool previous,
            UnityEvent onTrue,
            UnityEvent onFalse,
            UnityEvent<bool> onStateChanged)
        {
            if (current == previous)
                return;

            previous = current;

            if (current)
                onTrue?.Invoke();
            else
                onFalse?.Invoke();

            onStateChanged?.Invoke(current);
        }
    }
}
