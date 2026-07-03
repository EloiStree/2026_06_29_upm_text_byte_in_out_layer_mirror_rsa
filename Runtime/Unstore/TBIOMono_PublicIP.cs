using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
namespace Eloi.TBIO
{
    public class TBIOMono_PublicIP : MonoBehaviour
    {
        public string m_publicIp;
        public UnityEvent<string> m_onPublicIpFetch;
        public UnityEvent m_onPublicIpFetchFailed;

        [ContextMenu("Refresh")]
        public void Refresh()
        {

            StartCoroutine(FetchPublicIP());
        }
        private void Start()
        {
            StartCoroutine(FetchPublicIP());
        }

        private IEnumerator FetchPublicIP()
        {
            UnityWebRequest request = UnityWebRequest.Get("https://api.ipify.org");
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                m_onPublicIpFetchFailed.Invoke();
                m_publicIp = "";
                m_onPublicIpFetch.Invoke("");
            }
            else
            {
                string publicIP = request.downloadHandler.text;

                m_publicIp = publicIP;
                m_onPublicIpFetch.Invoke(publicIP);
            }
        }
    }
}
