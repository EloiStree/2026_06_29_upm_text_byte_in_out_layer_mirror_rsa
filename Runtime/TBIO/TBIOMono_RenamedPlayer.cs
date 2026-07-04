using Mirror;
using UnityEngine;

namespace Eloi.TBIO
{
    public class TBIOMono_RenamedPlayer : NetworkBehaviour
    {

        private void Reset()
        {
            m_whatToRename= gameObject;
        }

        public GameObject m_whatToRename;


        private void Awake()
        {
            Refresh();
        }

        public void Refresh()
        {
            
            m_isLocalPlayer = isLocalPlayer;
            m_isServer= isServer;
            m_isOwned= isOwned;

            string nameBuild = @"Player_";
            if (m_isLocalPlayer)
                nameBuild += "Local_";
            if (m_isServer)
                nameBuild += "Server_";
            if (m_isOwned)
                nameBuild += "Owned_";
            if (m_onStartAuthorityCalled)
                nameBuild += "Authority_";
            if (m_onStartClientCalled)
                nameBuild += "ClientC_";
            if (m_onStartServerCalled)
                nameBuild += "ServerC_";
            if (m_onStartLocalPlayerCalled)
                nameBuild += "LocalC_";
            gameObject.name = nameBuild;
        }

        public bool m_isLocalPlayer = false;
        public bool m_isServer = false;
        public bool m_isOwned = false;

        public bool m_onStartClientCalled = false;
        public bool m_onStartServerCalled = false;
        public bool m_onStartLocalPlayerCalled = false;
        public bool m_onStartAuthorityCalled = false;

        public override void OnStartClient()
        {
            base.OnStartClient();
            Refresh();
        }
        public override void OnStartServer()
        {
            base.OnStartServer();
            Refresh();
        }
      

        public override void OnStartLocalPlayer()
        {
            base.OnStartLocalPlayer();
            Refresh();
        }
        public override void OnStartAuthority()
        {
            base.OnStartAuthority();
            Refresh();
        }
    }
}
