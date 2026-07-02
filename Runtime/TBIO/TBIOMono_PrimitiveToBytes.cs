using UnityEngine;
using UnityEngine.Events;

namespace Eloi.TBIO
{
    public class TBIOMono_PrimitiveToBytes : MonoBehaviour
    {
        public UnityEvent<byte[]> m_onEmittedByte;
    }
}
