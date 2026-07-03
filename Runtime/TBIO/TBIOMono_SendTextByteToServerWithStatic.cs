using UnityEngine;
using UnityEngine.Events;

namespace Eloi.TBIO
{

    public class  TBIOMono_SendTextByteToServerWithStatic : MonoBehaviour
    {
        public void SendCharAsTextToServer(char character)
        {
            TBIOMono_PlayerInOut.SendTextToServerFromLocalPlayer(character.ToString());
        }
        public void SendCharAsByteUtf8ToServer(char character)
        {
            byte[] data = System.Text.Encoding.UTF8.GetBytes(character.ToString());
            TBIOMono_PlayerInOut.SendBytesToServerFromLocalPlayer(data);
        }

        public void SendTextToServer(string text)
        {
            TBIOMono_PlayerInOut.SendTextToServerFromLocalPlayer(text);
        }

        public void SendByteToServer(byte value)
        {
            byte[] data = new byte[1] { value };
            TBIOMono_PlayerInOut.SendBytesToServerFromLocalPlayer(data);
        }

        public void SendBytesToServer(params byte[] data)
        {
            TBIOMono_PlayerInOut.SendBytesToServerFromLocalPlayer(data);
        }
        public void SendTextAsBytesUtf8ToServer(string text)
        {
            byte[] data = System.Text.Encoding.UTF8.GetBytes(text);
            TBIOMono_PlayerInOut.SendBytesToServerFromLocalPlayer(data);
        }

        public void SendIntegerToServer(int value)
        {
            byte[] data = System.BitConverter.GetBytes(value);
            TBIOMono_PlayerInOut.SendBytesToServerFromLocalPlayer(data);
        }
        public void SendIndexIntegerToServer(int index, int value)
        {
            byte[] data = new byte[4 + 4];
            System.BitConverter.GetBytes(index).CopyTo(data, 0);
            System.BitConverter.GetBytes(value).CopyTo(data, 4);
            TBIOMono_PlayerInOut.SendBytesToServerFromLocalPlayer(data);
        }
        public void SendIntegersToServer(params int[] value) { 
        
            byte[] data = new byte[4 * value.Length];
            for (int i = 0; i < value.Length; i++)
            {
                System.BitConverter.GetBytes(value[i]).CopyTo(data, i * 4);
            }
            TBIOMono_PlayerInOut.SendBytesToServerFromLocalPlayer(data);
        }

        public void SendRandomInteger() => SendIntegerToServer(UnityEngine.Random.Range(-int.MaxValue, int.MaxValue));
        public void SendRandomPositiveInteger() => SendIntegerToServer(UnityEngine.Random.Range(0, int.MaxValue));
        public void SendRandomGamepadInteger() => SendIntegerToServer(UnityEngine.Random.Range(1800000000,1900000000));
        
    }

}
