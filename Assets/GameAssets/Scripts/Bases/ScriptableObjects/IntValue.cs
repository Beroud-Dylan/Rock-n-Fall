using UnityEngine;

namespace RocknFall.Bases.SO
{
    [CreateAssetMenu(fileName = "Int Value", menuName = "RocknFall/ValueData/Int")]
    public class IntValue : ScriptableObject
    {
        [SerializeField] int value;
        public int Value
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
        public delegate void onValueChange(int value);
        public event onValueChange OnValueChange;

        private void OnDestroy()
        {
            // Delete the events
            OnValueChange = null;
        }
    }
}
