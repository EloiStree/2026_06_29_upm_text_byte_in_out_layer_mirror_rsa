
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Eloi.TBIO
{
    public class TBIOUIMono_StringClampDebugLog : MonoBehaviour
    {

        public UnityEvent<string> m_onTextChanged;
        [TextArea(3, 10)]
        public string m_betweenLine ="\n";
        public List<string> m_log = new List<string>();
        public int m_maxMessage = 10;

        public bool m_useTextClamp= true;
        public int m_maxCharacter = 1000;

        public void AppendAsText(string s)
        {
            if ( m_useTextClamp && s.Length > m_maxCharacter)
            {
                s = s.Substring(0, m_maxCharacter);
            }

            m_log.Insert(0, s);
            if (m_log.Count > m_maxMessage)
                m_log.RemoveAt(m_log.Count - 1);

            m_onTextChanged?.Invoke(string.Join(m_betweenLine, m_log));
        }

        public void AppendAsUtf8(byte[] bytes)
        {
            string utf8String = System.Text.Encoding.UTF8.GetString(bytes);
            AppendAsText(utf8String);
        }

    }
}
