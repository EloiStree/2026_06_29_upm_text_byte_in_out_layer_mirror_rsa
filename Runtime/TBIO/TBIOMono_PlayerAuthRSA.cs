using UnityEngine;
using Mirror;
using UnityEngine.Events;

namespace Eloi.TBIO
{

    using Mirror;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Text;
    using UnityEngine;
    using UnityEngine.Events;

    


    public class TBIOMono_PlayerAuthRSA : NetworkBehaviour
    {
        #region RSA KEY MANAGEMENT

        [Header("pbit4096 b58 pkcs1 sha256")]
        [TextArea(3, 8)]
        [SyncVar(hook = nameof(HookValidatedServerPublicKeyChanged))]
        [SerializeField] string m_validatedServerPublicKeyAsTagB58;
        public UnityEvent<string> m_onValidatedPublicKeyOnServer;
        public UnityEvent<string> m_onValidatedPublicKeyAll;


        [Header("Public Private Local B58")]
        [TextArea(3,8)]
        [SerializeField] string m_clientPublicKeyRsa;
        [SerializeField] string m_clientPrivateKeyRsa;
        [TextArea(3, 8)]
        [SerializeField] string m_clientPublicKeyB58;
        [SerializeField] string m_clientPrivateKeyB58; 


        public void HookValidatedServerPublicKeyChanged(string oldValue, string newValue)
        {
            Debug.Log($"Validated server public key changed from\n\n {oldValue} \n\nto\n\n {newValue}\n\n");
            if (isServer)
                m_onValidatedPublicKeyOnServer?.Invoke(newValue);
            m_onValidatedPublicKeyAll?.Invoke(newValue);
        }


        public void GetRsaFilePath(out string path)
        {
            path = Application.persistentDataPath + "/player_private_rsa_key.txt";
        }

        [ContextMenu("Save RSA Key")]
        public void SaveInspectorInFile()
        {
            GetRsaFilePath(out string path);
            System.IO.File.WriteAllText(path, m_clientPrivateKeyRsa);
        }
        [ContextMenu("Load RSA Key")]
        public void LoadRsaKeyFromFileToInspector()
        {
            GetRsaFilePath(out string path);
            if (System.IO.File.Exists(path))
            {
                string privateKey = System.IO.File.ReadAllText(path);
                if (!string.IsNullOrEmpty(privateKey))
                {
                    SetPrivateRsaKeyInInspector(privateKey);
                    return;
                }
            }
            ClearRsaKeyToEmpty();
        }

        private void ClearRsaKeyToEmpty()
        {
            m_clientPrivateKeyRsa = "";
            m_clientPublicKeyRsa = "";
            m_clientPrivateKeyB58 = "";
            m_clientPublicKeyB58 = "";
        }

        public void SetPrivateRsaKeyInInspector(string privateRsaKey)
        {

            TBIO_Bit4096B58Pkcs1SHA256.CreateFromPrivateKeyXmlToBase58(
                privateRsaKey,
                out string publicKey,
                out string privateKeyBase58,
                out string publicKeyBase58
                );
            m_clientPrivateKeyRsa = privateRsaKey;
            m_clientPublicKeyRsa = publicKey;
            m_clientPrivateKeyB58 = privateKeyBase58;
            m_clientPublicKeyB58 = publicKeyBase58;
        }
        public void LoadOrGenerateRsaKeySaved()
        {
            if (string.IsNullOrEmpty(m_clientPrivateKeyRsa))
            {
                LoadRsaKeyFromFileToInspector();
            }

            if (string.IsNullOrEmpty(m_clientPrivateKeyRsa))
            {
                GenerateRsaKey();
                SaveInspectorInFile();
            }
        }


        public override void OnStartClient()
        {
            base.OnStartClient();
            LoadOrGenerateRsaKeySaved();
            m_onValidatedPublicKeyAll?.Invoke(m_clientPublicKeyB58);
            AddPlayerToDictonary();
        }

        public override void OnStopClient()
        {
            base.OnStopClient();
            RemovePlayerFromDictonary();
            RemovePlayerValidatedFromDictonary();
        }


        private void GenerateRsaKey()
        {
            TBIO_Bit4096B58Pkcs1SHA256.CreatePrivateKeyAsBase58(
                out string privateKey,
                out string publicKey,
                out string privateKeyBase58,
                out string publicKeyBase58
                );
            m_clientPrivateKeyRsa = privateKey;
            m_clientPublicKeyRsa = publicKey;
            m_clientPrivateKeyB58 = privateKeyBase58;
            m_clientPublicKeyB58 = publicKeyBase58;
        }

        #endregion




        #region SIGNATURE HANDSHAKE 

        public enum EnumMirrorRsaHankshakeServerSide : byte { None, UncheckStartConnection, SaidHello, SentHandshakeGUID, ReceivedHandshakeGUID, HandshakeReceivedIsWrong, HandshakeIsSignedAndValide }


        [Header("Network Handshake")]
        [SyncVar(hook =nameof(HookNotifyServerHostPlayer))]
        public bool m_isTheServerHost;

        [SyncVar(hook = nameof(HookPlayerHandshakeStateChanged))]
        public byte m_handshakeState;
        public EnumMirrorRsaHankshakeServerSide m_handShakeAsEnum;


        public event System.Action<EnumMirrorRsaHankshakeServerSide> m_onActionPlayerHandshakeStateChanged;

        public string m_messageToSign;

        [Header("Client")]
        public string m_clientMessageSignedB64;

        [Header("Server")]
        public string m_serverPublicKeyReceived;
        public string m_serverGuidSent;
        public byte[] m_serverGuidSentAsByte;
        public string m_serverB64SignedMessage;


        [SyncVar]
        public bool m_isHandshakeEstablished;

        public UnityEvent m_onRsaHandshakeValidatedClientEvent;
        public UnityEvent m_onRsaHandshakeValidatedServerEvent;


       

        void HookNotifyServerHostPlayer(bool previous, bool current)
        {
            if (current)
            {
                m_serverHostInstance = this;
            }
        }

        public bool IsPublicKeyValide() { return m_handShakeAsEnum == EnumMirrorRsaHankshakeServerSide.HandshakeIsSignedAndValide; }


        public override void OnStartServer()
        {
            m_handshakeState = (byte)EnumMirrorRsaHankshakeServerSide.UncheckStartConnection;

        }

        public override void OnStartLocalPlayer()
        {
            if (!isLocalPlayer) return;
            CmdSayHelloToServer(m_clientPublicKeyRsa);
            Debug.Log("OnStartLocalPlayer");
            m_localPlayerInstance = this;
        }


        [Command]
        public void CmdSayHelloToServer(string publicKeyRSA)
        {

            m_handshakeState = (byte)EnumMirrorRsaHankshakeServerSide.SaidHello;
            m_serverPublicKeyReceived = publicKeyRSA;
            m_serverGuidSent = Guid.NewGuid().ToString();
            m_serverGuidSentAsByte = Encoding.UTF8.GetBytes(m_serverGuidSent);
            RpcMessageToSign(m_serverGuidSent);
            m_handshakeState = (byte)EnumMirrorRsaHankshakeServerSide.SentHandshakeGUID;
            Debug.Log("CmdSayHelloToServer");
        }

        [ClientRpc]
        public void RpcMessageToSign(string messageToSign)
        {

            if (!isLocalPlayer) return;
            m_messageToSign = messageToSign;
            byte[] messageAsByte = Encoding.UTF8.GetBytes(messageToSign);
            byte[] signed = SignData(messageAsByte, m_clientPrivateKeyRsa);
            m_clientMessageSignedB64 = Convert.ToBase64String(signed);

            CmdPushSignedMessage(m_clientMessageSignedB64);
            Debug.Log("RpcMessageToSign:" + messageToSign);
        }

        [Command]
        public void CmdPushSignedMessage(string signMessageAsB64)
        {
            m_handshakeState = (byte)EnumMirrorRsaHankshakeServerSide.ReceivedHandshakeGUID;
            m_serverB64SignedMessage = signMessageAsB64;

            byte[] signedbyte = Convert.FromBase64String(signMessageAsB64);

            m_isHandshakeEstablished = VerifySignature(m_serverGuidSentAsByte, signedbyte, m_serverPublicKeyReceived);
            if (m_isHandshakeEstablished)
            {
                m_handshakeState = (byte)EnumMirrorRsaHankshakeServerSide.HandshakeIsSignedAndValide;
                AddPlayerValidatedToDictonary();
                TBIO_Bit4096B58Pkcs1SHA256.CreateTagPublicKeyAsBase58(m_serverPublicKeyReceived, out string publicKeyB58);
                m_validatedServerPublicKeyAsTagB58 = publicKeyB58;
                m_onRsaHandshakeValidatedServerEvent.Invoke();
            }
            else { 
                m_handshakeState = (byte)EnumMirrorRsaHankshakeServerSide.HandshakeReceivedIsWrong;
                RemovePlayerValidatedFromDictonary();
            }
            Debug.Log("CmdPushSignedMessage:" + signMessageAsB64);

        }


        void HookPlayerHandshakeStateChanged(byte previousHandShakeState, byte currentHandShakeState)
        {
            m_handShakeAsEnum = (EnumMirrorRsaHankshakeServerSide)currentHandShakeState;
            m_onActionPlayerHandshakeStateChanged?.Invoke(m_handShakeAsEnum);
            if (isLocalPlayer)
                m_onRsaHandshakeValidatedClientEvent?.Invoke();
        }

        public string GetPublicKeyRSA()
        {
            if (isLocalPlayer) return m_clientPublicKeyRsa;
            else return m_serverPublicKeyReceived;
        }

        #endregion

        #region STATIC PART

        public static Dictionary<NetworkIdentity, TBIOMono_PlayerAuthRSA> m_playersInGame = new Dictionary<NetworkIdentity, TBIOMono_PlayerAuthRSA>();
        public static Dictionary<NetworkIdentity, TBIOMono_PlayerAuthRSA> m_playersValidatedInGame = new Dictionary<NetworkIdentity, TBIOMono_PlayerAuthRSA>();
        static TBIOMono_PlayerAuthRSA m_serverHostInstance;
        static TBIOMono_PlayerAuthRSA m_localPlayerInstance;
        public static TBIOMono_PlayerAuthRSA GetServerHostInstance() { return m_serverHostInstance; }
        public static TBIOMono_PlayerAuthRSA GetLocalPlayerInstance() { return m_localPlayerInstance; }

        private void AddPlayerToDictonary()
        {
            if (!m_playersInGame.ContainsKey(this.netIdentity))
                m_playersInGame.Add(this.netIdentity, this);
        }
        private void RemovePlayerFromDictonary()
        {
            if (m_playersInGame.ContainsKey(this.netIdentity))
                m_playersInGame.Remove(this.netIdentity);
        }
        private void AddPlayerValidatedToDictonary()
        {
            if (!m_playersValidatedInGame.ContainsKey(this.netIdentity))
                m_playersValidatedInGame.Add(this.netIdentity, this);
        }
        private void RemovePlayerValidatedFromDictonary()
        {
            if (m_playersValidatedInGame.ContainsKey(this.netIdentity))
                m_playersValidatedInGame.Remove(this.netIdentity);
        }
        #endregion



        #region SIGNATURE
        private byte[] SignData(byte[] messageAsByte, string m_client_privateKey)
        {
            TBIO_Bit4096B58Pkcs1SHA256.SignDataWithRsaXmlPrivateKey(messageAsByte, m_client_privateKey, out byte[] signed);
            return signed;  
        }

        private bool VerifySignature(byte[] m_server_guidSentAsByte, byte[] signedbyte, string m_server_publicKeyReceived)
        {
            return TBIO_Bit4096B58Pkcs1SHA256.VerifySignatureFromRawBytes(m_server_guidSentAsByte, signedbyte, m_server_publicKeyReceived);
        }
        #endregion















    }
}
