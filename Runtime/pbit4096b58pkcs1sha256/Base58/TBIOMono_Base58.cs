using System.Text;
using UnityEngine;


namespace Eloi.TBIO
{

    public class TBIOMono_Base58 : MonoBehaviour
    {

        public string m_from;
        public string m_base58Encoded;
        public string m_to;

        private void OnValidate()
        {
            m_base58Encoded = TBIO_Base58.Base58EncodeToUTF8(m_from);
            m_to = TBIO_Base58.Base58DecodeFromUTF8(m_base58Encoded);
        }
    }


}