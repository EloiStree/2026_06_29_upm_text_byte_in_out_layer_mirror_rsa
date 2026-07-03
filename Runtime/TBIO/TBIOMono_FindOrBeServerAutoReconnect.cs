using UnityEngine;
using Mirror;
using System.Collections;
using Mirror.Discovery;

public class TBIOMono_FindOrBeServerAutoReconnect : MonoBehaviour
{
    public float m_searchTimeout = 5.0f;
    public float m_reconnectInterval = 5f;

    public NetworkManager m_networkManager;
    public NetworkDiscovery m_discovery;

    [SerializeField] bool m_serverFound = false;
    [SerializeField] bool m_isHost = false;



    private void Reset()
    {
        if (transform.parent) { 
            m_networkManager = this.transform.parent.GetComponent<NetworkManager>();
            m_discovery = this.transform.parent.GetComponent<NetworkDiscovery>();
        }

    }

    void OnEnable()
    {
        m_discovery.OnServerFound.AddListener(OnServerFound);
        StartCoroutine(MainLoop());
    }

    private IEnumerator MainLoop()
    {
        while (true)
        {
            // =========================
            // 1. If NOT connected → try to connect / host
            // =========================
            if (!NetworkClient.isConnected && !m_isHost)
            {
                yield return StartCoroutine(ConnectOrHost());
            }

            // =========================
            // 2. If connected → just monitor
            // =========================
            while (NetworkClient.isConnected || m_isHost)
            {
                yield return new WaitForSeconds(1f);
            }

            // =========================
            // 3. If we reach here → disconnected → wait then retry
            // =========================
            // Debug.Log("Disconnected detected (polling). Reconnecting in loop...");
            yield return new WaitForSeconds(m_reconnectInterval);
        }
    }

    private IEnumerator ConnectOrHost()
    {
        m_serverFound = false;

        //Debug.Log("Searching for server...");

        m_discovery.StartDiscovery();

        float timer = 0f;

        while (timer < m_searchTimeout && !m_serverFound)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        m_discovery.StopDiscovery();

        if (!m_serverFound)
        {
            Debug.Log("No server found → becoming host");

            m_networkManager.StartHost();
            m_discovery.AdvertiseServer();
            m_isHost = true;
        }
        else
        {
            Debug.Log("Server found → connecting handled via callback");
        }
    }

    private void OnServerFound(ServerResponse info)
    {
        if (m_serverFound) return;

        m_serverFound = true;

        Debug.Log($"Server found at {info.uri} → connecting");

        m_discovery.StopDiscovery();

        m_networkManager.StartClient(info.uri);
    }

    private void OnDestroy()
    {
        if (m_discovery != null)
            m_discovery.OnServerFound.RemoveListener(OnServerFound);
    }
}