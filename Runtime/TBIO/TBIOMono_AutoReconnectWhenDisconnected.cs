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


        // State
        private Coroutine reconnectCoroutine;


        private void OnEnable()
        {
            if (networkManager == null)
                networkManager = NetworkManager.singleton;
            reconnectCoroutine= StartCoroutine(ReconnectLoop());
        }
      

      

        #region Reconnect Loop

        public float waitBeforeStartingReconnectLoop = 10f;
        private IEnumerator ReconnectLoop()
        {
            yield return new WaitForSeconds(waitBeforeStartingReconnectLoop);

            while (true)
            {
                if (NetworkClient.isConnected)
                {
                    yield return new WaitForSeconds(reconnectInterval);
                    yield break;
                }
                if (!NetworkClient.active)
                    networkManager.StartClient();
                yield return new WaitForSeconds(reconnectInterval);
                yield return new WaitForSeconds(0.1f);
            }
        }

        #endregion
    }
}