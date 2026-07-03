using UnityEngine;
using Mirror;
using UnityEngine.Events;
namespace Eloi.TBIO
{
    public class TBIOMono_ServerTimeOffsetAndPing : NetworkBehaviour
    {

        public ulong m_sharedClientPingInMilliseconds;
        public UnityEvent<ulong> m_onTickOffset;
        public UnityEvent<float> m_onPingInMilliseconds;
        public UnityEvent<float> m_onPingInSeconds;
        public ulong m_playerSentLocalTimeUTC;
        public ulong m_serverAtReceivedLocalTimeUTC;
        public ulong m_playerCallbackLocalTimeUTC;


        public float m_timeBetweenRefresh = 5f;

        public override void OnStartClient()
        {
            base.OnStartClient();
            if (!isLocalPlayer)
                return;
            RequestOffsetAndPingUpdate();
        }

        public void GetComputerTicks(out ulong ticks)
        {
            ticks = (ulong)System.DateTime.UtcNow.Ticks;
        }

        [ContextMenu("Request Offset And Ping Update")]
        public void RequestOffsetAndPingUpdate() { 
        
            GetComputerTicks(out ulong ticks);
            CmdAskServerTimeOffset(ticks);  
        }

        [Command]
        void CmdAskServerTimeOffset(ulong playerLocalTimeUTC) {
            GetComputerTicks(out ulong serverTicks);
            RpcCallbackPlayerTimeOffset(connectionToClient, playerLocalTimeUTC, serverTicks);

        }

        [TargetRpc]
        void RpcCallbackPlayerTimeOffset(NetworkConnection target, ulong playerLocalTimeUTC, ulong serverLocalTimeUTC) {
            m_playerSentLocalTimeUTC = playerLocalTimeUTC;
            m_serverAtReceivedLocalTimeUTC = serverLocalTimeUTC;
            GetComputerTicks(out ulong ticks);
            m_playerCallbackLocalTimeUTC = ticks;
            CmdInformServerOfCallbackLocalTime(m_playerCallbackLocalTimeUTC);
            ComputerDeductionOfPing();
        }
        [Command]
        void CmdInformServerOfCallbackLocalTime(ulong playerCallbackLocalTimeUTC)
        {
            m_playerCallbackLocalTimeUTC = playerCallbackLocalTimeUTC;
            ComputerDeductionOfPing();

            m_sharedClientPingInMilliseconds = m_tickPingSendToReceivedHalfInMilliseconds;
        }

        void ComputerDeductionOfPing() {


            m_tickPingSendToReceived = m_playerCallbackLocalTimeUTC - m_playerSentLocalTimeUTC;
            m_tickPingSendToReceivedHalf = m_tickPingSendToReceived / 2;
            m_tickPingSendToReceivedHalfInMilliseconds = m_tickPingSendToReceivedHalf / 10000;

            m_clientOffsetOfServerTimeWithoutPing = m_serverAtReceivedLocalTimeUTC - m_playerSentLocalTimeUTC;
            m_clientOffsetOfServerTimeWithPing = m_clientOffsetOfServerTimeWithoutPing + m_tickPingSendToReceivedHalf;

            m_onTickOffset?.Invoke(m_clientOffsetOfServerTimeWithPing);
            m_onPingInMilliseconds?.Invoke(m_tickPingSendToReceivedHalfInMilliseconds);
            m_onPingInSeconds?.Invoke(m_tickPingSendToReceivedHalfInMilliseconds / 1000f);
        }
        public ulong m_tickPingSendToReceived;
        public ulong m_tickPingSendToReceivedHalf;
        public ulong m_tickPingSendToReceivedHalfInMilliseconds;
        public ulong m_clientOffsetOfServerTimeWithoutPing;
        public ulong m_clientOffsetOfServerTimeWithPing;
    }
}
