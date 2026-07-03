using System;
using UnityEngine;
using UnityEngine.Events;

namespace Eloi.TBIO
{
    public class TBIOMono_PrimitiveToBytes : MonoBehaviour
    {
        public UnityEvent<byte[]> m_onEmittedByte;

        public void PushInCharAsByte(char c)
        {
            byte[] bytes = System.BitConverter.GetBytes(c);
            m_onEmittedByte.Invoke(bytes);
        }
        public void PushInCharAsUTF8(char c)
        {
            PushInTextAsUTF8("" + c);
        }
        public void PushInTextAsUTF8(string text)
        {
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(text);
            m_onEmittedByte.Invoke(bytes);
        }
        public void PushInBytes(byte[] bytes)
        {
            m_onEmittedByte.Invoke(bytes);
        }

        [ContextMenu("Push In Random Integer")]
        public void PushInRandomInteger()
        {
            PushInInteger(UnityEngine.Random.Range(-int.MaxValue, int.MaxValue));
        }
        public void PushInInteger(int i)
        {
            byte[] bytes = System.BitConverter.GetBytes(i);
            m_onEmittedByte.Invoke(bytes);
        }
        public void PushInFloat(float f)
        {
            byte[] bytes = System.BitConverter.GetBytes(f);
            m_onEmittedByte.Invoke(bytes);
        }
        public void PushInDouble(double d)
        {
            byte[] bytes = System.BitConverter.GetBytes(d);
            m_onEmittedByte.Invoke(bytes);
        }


        public void PushInLong(long l)
        {
            byte[] bytes = System.BitConverter.GetBytes(l);
            m_onEmittedByte.Invoke(bytes);
        }

        [ContextMenu("Push In Random Gamepad Value")]
        public void PushInRandomGamepadValue()
        {
            PushInInteger(UnityEngine.Random.Range(1800000000,1900000000));
        }
        [ContextMenu("Push In Random Index Integer")]
        public void PushInRandomIndexInteger()
        {
            PushInIndexInteger(UnityEngine.Random.Range(-int.MaxValue, int.MaxValue), UnityEngine.Random.Range(-int.MaxValue, int.MaxValue));

        }
        public void PushInIndexInteger(int index, int integer)
        {
            byte[] bytes = new byte[8];
            BitConverter.GetBytes(index).CopyTo(bytes, 0);
            BitConverter.GetBytes(integer).CopyTo(bytes, 4);
            m_onEmittedByte.Invoke(bytes);
        }
        public void PushInIndexIntegerUlongDate(int index, int integer, ulong date)
        {
            byte[] bytes = new byte[16];
            BitConverter.GetBytes(index).CopyTo(bytes, 0);
            BitConverter.GetBytes(integer).CopyTo(bytes, 4);
            BitConverter.GetBytes(date).CopyTo(bytes, 8);
            m_onEmittedByte.Invoke(bytes);

        }
        public void PushInIntegerUlongDate(int integer, ulong date)
        {
            byte[] bytes = new byte[12];
            BitConverter.GetBytes(integer).CopyTo(bytes, 0);
            BitConverter.GetBytes(date).CopyTo(bytes, 4);
            m_onEmittedByte.Invoke(bytes);
        }

        [ContextMenu("Push In Random Short")]
        public void PushInRandomShort()
        {
            PushInShort((short)UnityEngine.Random.Range(short.MinValue, short.MaxValue));
        }
        [ContextMenu("Push In Random UShort")]
        public void PushInRandomUShort()
        {
            PushInUShort((ushort)UnityEngine.Random.Range(ushort.MinValue, ushort.MaxValue));
        }
        public void PushInUShort(ushort s)
        {
            byte[] bytes = System.BitConverter.GetBytes(s);
            m_onEmittedByte.Invoke(bytes);
        }
        public void PushInShort(short s)
        {
            byte[] bytes = System.BitConverter.GetBytes(s);
            m_onEmittedByte.Invoke(bytes);
        }


        [ContextMenu("Push In Random Byte")]    
        public void PushInRandomByte()
        {
            PushInByte((byte)UnityEngine.Random.Range(0, 255));
        }
        public void PushInByte(byte b)
        {
            byte[] bytes = new byte[1] { b };
            m_onEmittedByte.Invoke(bytes);
        }
        public void PushInBoolean(bool b)
        {
            byte[] bytes = System.BitConverter.GetBytes(b);
            m_onEmittedByte.Invoke(bytes);
        }

        public void PushInFloatArray(params float[] value)
        {
            byte[] bytes = new byte[value.Length * 4];
            for (int i = 0; i < value.Length; i++)
            {
                BitConverter.GetBytes(value[i]).CopyTo(bytes, i * 4);
            }
            m_onEmittedByte.Invoke(bytes);
        }
  
        public void PushInVector3(Vector3 vector3)
        {
            PushInFloatArray(vector3.x, vector3.y, vector3.z);
        }
        public void PushInQuaternion(Quaternion q)
        {
            PushInFloatArray(q.x, q.y, q.z, q.w);
        }
    }
}
