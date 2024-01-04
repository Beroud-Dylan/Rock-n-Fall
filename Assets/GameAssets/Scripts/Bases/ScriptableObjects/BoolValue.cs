using UnityEngine;

namespace RocknFall.Bases.SO
{
    [CreateAssetMenu(fileName = "Bool Value", menuName = "RocknFall/ValueData/Bool")]
    public class BoolValue : ScriptableObject
    {
        [SerializeField] bool value;
        public bool Value
        {
            get => value;
            set
            {
                // Modify the value
                this.value = value;
                // Trigger the event related to the value
                OnValueChange?.Invoke(this.value);
            }
        }
        public delegate void onValueChange(bool value);
        public event onValueChange OnValueChange;

        private void OnDestroy()
        {
            // Delete the events
            OnValueChange = null;
        }
    }
}
