using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Eloi.TBIO
{
    public class TBIOUIMono_InputFieldToUnityEvent : MonoBehaviour
    {
        public UnityEvent<string> m_onSubmitText;
        public UnityEvent<int> m_onSubmitInteger;
        public InputField m_observedInputField;
        public Button[] m_submitButton; 

        public bool m_useEndEditSubmit = false;

        public void OnEnable()
        {
            if (m_observedInputField != null)
            {
                if (m_useEndEditSubmit)
                    m_observedInputField.onEndEdit.AddListener(SubmitTextFromInputField);
            }
            foreach (var button in m_submitButton)
            {
                if (button)
                    button.onClick.AddListener(SubmitTextFromInputField);
            }
        }

        public void OnDisable()
        {
            if (m_observedInputField != null)
            {
                m_observedInputField.onEndEdit.RemoveListener(SubmitTextFromInputField);
            }
            foreach (var button in m_submitButton)
            {
                if (button)
                    button.onClick.RemoveListener(SubmitTextFromInputField);
            }
        }

        void SubmitTextFromInputField(string text)
        {
            SubmitTextFromInputField();
        }

        public void SubmitTextFromInputField()
        {
            if (m_observedInputField != null)
            {
                m_onSubmitText.Invoke(m_observedInputField.text);
                if (int.TryParse(m_observedInputField.text, out int value))
                {
                    m_onSubmitInteger.Invoke(value);
                }
            }
        }



    }
}
