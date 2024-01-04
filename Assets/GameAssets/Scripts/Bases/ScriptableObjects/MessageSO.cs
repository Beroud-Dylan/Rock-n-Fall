using UnityEngine;

namespace RocknFall.Bases.SO
{
    [CreateAssetMenu(fileName = "Message Value", menuName = "RocknFall/Events/Message")]
    public class MessageSO : ScriptableObject
    {
        public delegate void onNewMessage(string message);
        public event onNewMessage OnNewMessage;

        public void SendMessage(string message)
        {
            OnNewMessage?.Invoke(message);
        }

        private void OnDestroy()
        {
            // Delete the events
            OnNewMessage = null;
        }
    }
}
