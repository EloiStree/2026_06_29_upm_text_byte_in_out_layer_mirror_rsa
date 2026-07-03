using UnityEngine;
using Mirror;
using UnityEngine.Events;
namespace Eloi.TBIO
{
    public class TBIOMono_ServerTimeOffsetAndPing : NetworkBehaviour
    {

        [SyncVar]
        public ulong m_sharedClientPingInMilliseconds;
        [SyncVar]
        public ulong m_sharedClientOffsetInMilliseconds;

        public UnityEvent<ulong> m_onTickOffset;
        public UnityEvent<float> m_onPingInMilliseconds;
        public UnityEvent<float> m_onPingInSeconds;
        public ulong m_playerSentLocalTimeUTC;
        public ulong m_serverAtReceivedLocalTimeUTC;
        public ulong m_playerCallbackLocalTimeUTC;
        
        [Header("Deducted Values")]
        public ulong m_tickPingSendToReceived;
        public ulong m_tickPingSendToReceivedHalf;
        public ulong m_tickPingSendToReceivedHalfInMilliseconds;
        public ulong m_clientOffsetOfServerTimeWithoutPing;
        public ulong m_clientOffsetOfServerTimeWithPing;

        public float m_offsetInSeconds;
        public float m_offsetInMinutes;
        public float m_offsetInHours;


        public float m_timeBetweenRefresh = 5f;

        public override void OnStartAuthority()
        {
            base.OnStartAuthority();
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
            CmdInformServerOfFullTiming(playerLocalTimeUTC, serverLocalTimeUTC, m_playerCallbackLocalTimeUTC);
            ComputerDeductionOfPing();
        }
        [Command]
        void CmdInformServerOfFullTiming(ulong playerLocalTimeUTC, ulong serverLocalTimeUTC,ulong playerCallbackLocalTimeUTC)
        {
            m_playerSentLocalTimeUTC = playerLocalTimeUTC;
            m_serverAtReceivedLocalTimeUTC = serverLocalTimeUTC;
            m_playerCallbackLocalTimeUTC = playerCallbackLocalTimeUTC;

            ComputerDeductionOfPing();

            m_sharedClientPingInMilliseconds = m_tickPingSendToReceivedHalfInMilliseconds;
            m_sharedClientOffsetInMilliseconds= m_clientOffsetOfServerTimeWithPing / 10000;
        }

        void ComputerDeductionOfPing() {

            if (m_playerSentLocalTimeUTC == 0 || m_serverAtReceivedLocalTimeUTC == 0 || m_playerCallbackLocalTimeUTC == 0)
            {
                m_tickPingSendToReceived = 0;
                m_tickPingSendToReceivedHalf = 0;
                m_playerCallbackLocalTimeUTC = 0;
                m_clientOffsetOfServerTimeWithoutPing= 0;
                m_clientOffsetOfServerTimeWithPing= 0;

                return;
            }

            m_tickPingSendToReceived = m_playerCallbackLocalTimeUTC - m_playerSentLocalTimeUTC;
            m_tickPingSendToReceivedHalf = m_tickPingSendToReceived / 2;
            m_tickPingSendToReceivedHalfInMilliseconds = m_tickPingSendToReceivedHalf / 10000;

            m_clientOffsetOfServerTimeWithoutPing = m_serverAtReceivedLocalTimeUTC - m_playerSentLocalTimeUTC;
            m_clientOffsetOfServerTimeWithPing = m_clientOffsetOfServerTimeWithoutPing + m_tickPingSendToReceivedHalf;


       m_offsetInSeconds = m_clientOffsetOfServerTimeWithPing / 10000000f;
       m_offsetInMinutes = m_clientOffsetOfServerTimeWithPing / 10000000f / 60f;
        m_offsetInHours = m_clientOffsetOfServerTimeWithPing / 10000000f / 3600f;
        m_onTickOffset?.Invoke(m_clientOffsetOfServerTimeWithPing);
            m_onPingInMilliseconds?.Invoke(m_tickPingSendToReceivedHalfInMilliseconds);
            m_onPingInSeconds?.Invoke(m_tickPingSendToReceivedHalfInMilliseconds / 1000f);
        }
       
    }
}
