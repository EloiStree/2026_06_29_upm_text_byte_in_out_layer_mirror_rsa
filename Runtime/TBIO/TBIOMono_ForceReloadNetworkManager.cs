using Mirror;
using UnityEngine;


namespace Eloi.TBIO
{
    public class TBIOMono_ForceReloadNetworkManager : MonoBehaviour
    {
        public NetworkManager m_networkManager;

        public void ForceReloadAsSeverHost()
        {

            if (m_networkManager != null)
            {
                //If server is already running, stop it first
                if (m_networkManager.isNetworkActive)
                {
                    m_networkManager.StopHost();
                }
                m_networkManager.StartHost();
            }
        }
    }
}