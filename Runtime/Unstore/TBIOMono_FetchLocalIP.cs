
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;
using UnityEngine.Events;
namespace Eloi.TBIO
{
 
public class TBIOMono_FetchLocalIP : MonoBehaviour
    {
        public List<string> m_localIpFound;
        public string m_stringViewSpliter = ", ";
        public UnityEvent<string[]> m_onLocalIpFound;
        public UnityEvent<string> m_onLocalIpFoundAsString;
        private void Start()
        {
            RefreshLocalIp();
        }


        [ContextMenu("Refresh Local IP")]
        public void RefreshLocalIp()
        {
            m_localIpFound = GetLocalIPv4Addresses();
            m_onLocalIpFound.Invoke(m_localIpFound.ToArray());
            m_onLocalIpFoundAsString.Invoke(string.Join(m_stringViewSpliter, m_localIpFound.ToArray()));
        }

        private List<string> GetLocalIPv4Addresses()
        {
            List<string> ipAddresses = new List<string>();

            // Get all network interfaces
            NetworkInterface[] networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();

            foreach (NetworkInterface networkInterface in networkInterfaces)
            {
                // Filter out loopback and non-operational interfaces
                if (networkInterface.NetworkInterfaceType == NetworkInterfaceType.Loopback ||
                    networkInterface.OperationalStatus != OperationalStatus.Up)
                {
                    continue;
                }

                IPInterfaceProperties ipProperties = networkInterface.GetIPProperties();
                foreach (UnicastIPAddressInformation ipAddressInfo in ipProperties.UnicastAddresses)
                {

                    if (ipAddressInfo.Address.AddressFamily != System.Net.Sockets.AddressFamily.InterNetwork)
                    {
                        continue;
                    }
                    ipAddresses.Add(ipAddressInfo.Address.ToString());
                }
            }

            return ipAddresses;
        }
    }
}
