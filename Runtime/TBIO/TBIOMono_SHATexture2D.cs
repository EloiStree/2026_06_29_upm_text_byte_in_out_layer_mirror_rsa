using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using UnityEngine.Events;

namespace Eloi.TBIO
{
    public class TBIOMono_SHATexture2D : MonoBehaviour
    {
        public byte[] m_hash512;
        public Texture2D m_textureOfHash512;
        public UnityEvent<Texture2D> m_onTextureGenerated;

        private const int Size = 8;

        public void SetTexture2DWithHashOf(string text)
        {
            using (SHA512 sha = SHA512.Create())
            {
                m_hash512 = sha.ComputeHash(Encoding.UTF8.GetBytes(text));
            }

            Color[] colors = new Color[Size * Size];

            for (int i = 0; i < colors.Length; i++)
            {
                byte b = m_hash512[i];

                float hue = b / 255f;
                colors[i] = Color.HSVToRGB(hue, 0.8f, 1f);
            }

            m_textureOfHash512 = new Texture2D(Size, Size, TextureFormat.RGBA32, false);
            m_textureOfHash512.filterMode = FilterMode.Point;
            m_textureOfHash512.wrapMode = TextureWrapMode.Clamp;

            m_textureOfHash512.SetPixels(colors);
            m_textureOfHash512.Apply();

            m_onTextureGenerated?.Invoke(m_textureOfHash512);
        }
    }
}