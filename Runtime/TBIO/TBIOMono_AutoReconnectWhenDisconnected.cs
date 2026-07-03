using Mirror;
using System.Collections;
using UnityEngine;

namespace Eloi.TBIO
{
    public class TBIOMono_AutoReconnectWhenDisconnected : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private NetworkManager networkManager;

        [Header("Reconnect Settings")]
        [SerializeField] private float reconnectInterval = 3f;
        [SerializeField] private float initialDelay = 10f;

        // State
        private Coroutine reconnectCoroutine;

        private void OnEnable()
        {
            if (networkManager == null)
                networkManager = NetworkManager.singleton;

            // Stop any existing coroutine
            if (reconnectCoroutine != null)
                StopCoroutine(reconnectCoroutine);

            reconnectCoroutine = StartCoroutine(ReconnectLoop());
        }

        private void OnDisable()
        {
            if (reconnectCoroutine != null)
            {
                StopCoroutine(reconnectCoroutine);
                reconnectCoroutine = null;
            }
        }

        private IEnumerator ReconnectLoop()
        {
            // Initial delay
            if (initialDelay > 0)
                yield return new WaitForSeconds(initialDelay);

            while (true)
            {
                // If we're properly connected, just wait and check again later
                if (NetworkClient.isConnected && NetworkClient.active)
                {
                    yield return new WaitForSeconds(reconnectInterval);
                    continue;
                }

                // We're not connected → try to reconnect
                if (!NetworkClient.active)
                {
                    Debug.Log("[AutoReconnect] Client is disconnected. Attempting to reconnect...");
                    networkManager.StartClient();
                }

                // Wait before next check
                yield return new WaitForSeconds(reconnectInterval);
                yield return new WaitForSeconds(0.1f);
            }
        }
    }
}