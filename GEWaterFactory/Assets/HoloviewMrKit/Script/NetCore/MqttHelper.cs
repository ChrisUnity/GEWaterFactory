using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using uPLibrary.Networking.M2Mqtt;
using UnityEngine;
namespace ShowNowMrKit
{
    public enum ConnectState { Connected, Disconnected }
    public enum MessageType { System, Group }

    public delegate void MqttReceiveMsgDelegate(MqttHelper mqtt, String msg);
    public delegate void MqttReceiveByteMsgDelegate(MqttHelper mqtt, byte[] msg);

    public delegate void MqttSendMsgDelegate(bool result);
    public delegate void MqttConnectDelegate(MqttHelper mqtt, bool result);
    public delegate void ResultDelegate<T>(T t);
    public delegate void ResultDelegate<T, M>(T t, M m);

    public class MqttHelper
    {


        private static readonly object InstanceLock = new object();
        private static readonly object MessageQueueLock = new object();
        private static readonly object MessageSendingQueueLock = new object();
        private static readonly object SubscribeingQueueLock = new object();

        private static MqttHelper _instance;

        private MqttClient mqttClient = null;


        

        public event MqttReceiveMsgDelegate OnReceiveMsg;
        public event ResultDelegate<MqttHelper, ConnectState> onConnectStateChange;
        public event MqttReceiveByteMsgDelegate OnReceiveByteMsg;

        private Queue<MessageBean> messageQueue = new Queue<MessageBean>();
        private List<MessageBean> messageSendingQueue = new List<MessageBean>();
        private List<SubscribeBean> subscribeingQueue = new List<SubscribeBean>();

        /// <summary>
        ///  The single instance of the Conductor class.
        /// </summary>
        public static MqttHelper Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (InstanceLock)
                    {
                        if (_instance == null)
                        {
                            _instance = new MqttHelper();
                        }
                    }
                }
                return _instance;
            }
        }
        private bool loop = false;

        public ClientType clientType { set; get; }
        public MqttHelper()
        {
            uPLibrary.Networking.M2Mqtt.Utility.Trace.TraceLevel = uPLibrary.Networking.M2Mqtt.Utility.TraceLevel.Verbose;
            uPLibrary.Networking.M2Mqtt.Utility.Trace.TraceListener = new uPLibrary.Networking.M2Mqtt.Utility.WriteTrace(WriteTrace);
        }

        public static void WriteTrace(string format, params object[] args)
        {
            //Debug.Log(String.Format(format, args));
        }

        private void startTask()
        {
            Task sendTask = new Task(async () =>
            {
                while (loop)
                {
                    MessageBean m = null;
                    lock (MessageQueueLock)
                    {
                        if (messageQueue.Count > 0)
                        {
                            m = messageQueue.Dequeue();
                        }
                    }

                    if (m != null)
                    {
                        PublishMsgInternal(m);
                    }
                    else
                    {
                        await Task.Delay(10);
                    }
                }

            });
            sendTask.Start();
        }

        private string clientId;
        public void Connect(string serverIp, int serverPort, string userName, string PassWord, string clientId, MqttConnectDelegate onConnect)
        {
            loop = true;
            //startTask();

            this.clientId = clientId;

            new Task(() =>
            {
                try {
#if !UNITY_EDITOR && UNITY_WSA
                    mqttClient = new MqttClient(serverIp, serverPort, false, MqttSslProtocols.None);
#else
                    mqttClient = new MqttClient(serverIp, serverPort, false, null, null, MqttSslProtocols.None);
#endif
                    mqttClient.MqttMsgPublishReceived += MqttClient_MqttMsgPublishReceived;
                    mqttClient.MqttMsgPublished += MqttClient_MqttMsgPublished;
                    mqttClient.ConnectionClosed += MqttClient_ConnectionClosed;
                    mqttClient.MqttMsgSubscribed += MqttClient_MqttMsgSubscribed;
                    byte ret = mqttClient.Connect(clientId, userName, PassWord, true, 30);
                    onConnect?.Invoke(this,true);
                    onConnectStateChange?.Invoke(this, ConnectState.Connected);
                    Debug.Log("Mqtt Connect:" + ret);
                }
                catch (Exception e){
                    onConnect?.Invoke(this, false);
                    onConnectStateChange?.Invoke(this, ConnectState.Disconnected);

                    Debug.Log(e);
                }

            }).Start();
        }

        private void MqttClient_MqttMsgSubscribed(object sender, uPLibrary.Networking.M2Mqtt.Messages.MqttMsgSubscribedEventArgs e)
        {
            lock (SubscribeingQueueLock)
            {
                foreach(SubscribeBean bean in subscribeingQueue)
                {
                    if(bean.msgId == e.MessageId)
                    {
                        bean.resultDelegate?.Invoke(true);
                        subscribeingQueue.Remove(bean);
                        break;
                    }
                }
            }
        }

        private void MqttClient_ConnectionClosed(object sender, EventArgs e)
        {
            Debug.Log("MqttClient_ConnectionClosed");

            onConnectStateChange?.Invoke(this, ConnectState.Disconnected);
        }

        private void MqttClient_MqttMsgPublished(object sender, uPLibrary.Networking.M2Mqtt.Messages.MqttMsgPublishedEventArgs e)
        {
            ushort messageId = e.MessageId;
            lock (MessageSendingQueueLock)
            {
                foreach(MessageBean m in messageSendingQueue)
                {
                    if(m.msgId == messageId)
                    {
                        m.onSend?.Invoke(e.IsPublished);
                        messageSendingQueue.Remove(m);
                        break;
                    }
                }
            }
        }

        public void Disconnect()
        {
            this.loop = false;
            this.clientId = null;

            try
            {
                if(mqttClient != null)
                {
                    if (mqttClient.IsConnected)
                    {
                        mqttClient.Disconnect();
                    }
                    mqttClient = null;
                }
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }
        }

        private void MqttClient_MqttMsgPublishReceived(object sender, uPLibrary.Networking.M2Mqtt.Messages.MqttMsgPublishEventArgs e)
        {
           // Debug.Log(Encoding.UTF8.GetString(e.Message));
            if (e.Topic.Equals(this.clientId))
            {
                OnReceiveMsg?.Invoke(this, Encoding.UTF8.GetString(e.Message));
            }
            else
            {
                //Debug.Log(e.Topic + "  "+Encoding.UTF8.GetString(e.Message));
                OnReceiveByteMsg?.Invoke(this, e.Message);
            }
        }
        public void SubscribeMsg(string topic,ResultDelegate<bool> resultDelegate)
        {
            string[] topics = { topic };
            byte[] qosLevels = { 0 }; 
            ushort messageId = mqttClient.Subscribe(topics, qosLevels);

            SubscribeBean bean = new SubscribeBean();
            bean.topic = topic;
            bean.msgId = messageId;
            bean.resultDelegate = resultDelegate;
            lock (SubscribeingQueueLock)
            {
                subscribeingQueue.Add(bean);
            }
        }

        public void UnSubscribeMsg(string topic)
        {
            string[] topics = { topic };
            if(mqttClient != null && mqttClient.IsConnected)
                mqttClient.Unsubscribe(topics);
        }
        public void PublishMsg(MessageType type,String targetId, String msg, MqttSendMsgDelegate onSend)
        {
			Debug.Log ("PublishMsg:" + msg);
            MessageBean m = new MessageBean();
            m.type = type;
            m.targetId = targetId;
            m.msg = Encoding.UTF8.GetBytes(msg);
            m.onSend = onSend;
            //lock (MessageQueueLock)
            //{
            //    messageQueue.Enqueue(m);
            //}
            PublishMsgInternal(m);
        }

        public void PublishMsg(MessageType type, String targetId, byte[] msg, MqttSendMsgDelegate onSend)
        {
            MessageBean m = new MessageBean();
            m.type = type;
            m.targetId = targetId;
            m.msg = msg;
            m.onSend = onSend;

            //lock (MessageQueueLock)
            //{
            //    messageQueue.Enqueue(m);
            //}
            PublishMsgInternal(m);

        }
        private void PublishMsgInternal(MessageBean m)
        {
            String topic = "";
            int qos = 0;
            if (m.type == MessageType.System)
            {
                // topic += "1000/";
                topic = "EnterRoomholoview";
                qos = 1;
            }
            else if (m.type == MessageType.Group)
            {
                //topic += "2000/";
                topic = m.targetId;
            }
            else
            {
                return;
            }
            //topic += m.targetId;
            if (mqttClient == null) { return; }
            ushort  msgid = mqttClient.Publish(topic, m.msg, (byte)qos, false);
            m.msgId = msgid;

            lock (MessageSendingQueueLock)
            {
                messageSendingQueue.Add(m);
            }
        }

        private class MessageBean
        {
            public MessageType type { set; get; }
            public string targetId { set; get; }
            public byte[] msg { set; get; }

            public MqttSendMsgDelegate onSend { set; get; }
            public ushort msgId { set; get; }
        }

        private class SubscribeBean
        {
            public string topic { set; get; }
            public ResultDelegate<bool> resultDelegate { set; get; }
            public ushort msgId { set; get; }
        }
    }
}
