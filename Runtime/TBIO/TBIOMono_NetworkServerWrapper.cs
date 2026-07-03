using Mirror;
using System.Collections;
using UnityEngine;


namespace Eloi.TBIO
{
    public class TBIOMono_NetworkServerWrapper : MonoBehaviour
    {
        public NetworkManager m_networkServer;



        [ContextMenu("Stop Network Manager")]
        public void StopNetworkManager()
        {
            if (m_networkServer == null)
                return;

            if (!m_networkServer.isNetworkActive)
                return;

            // StopHost includes stopping the client, so we don't need both
            if (NetworkServer.active)
            {
                m_networkServer.StopHost();
            }
            else if (NetworkClient.isConnected)
            {
                m_networkServer.StopClient();
            }
        }
        [ContextMenu("Start Server Manager")]
        public void StartServer()
        {
            //StopNetworkManager();
            m_networkServer.StartHost();
        }

        [ContextMenu("Start Client Manager")]
        public void StartAsClientTargetingServer(string ipAddress)
        {
            //StopNetworkManager();
            m_networkServer.networkAddress = ipAddress;
            m_networkServer.StartClient();
        }

    }
}