using UnityEngine;

namespace RocknFall.Bases.SO
{
    [CreateAssetMenu(fileName = "Float Value", menuName = "RocknFall/ValueData/Float")]
    public class FloatValue : ScriptableObject
    {
        [SerializeField] float value;
        public float Value
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
        public delegate void onValueChange(float value);
        public event onValueChange OnValueChange;

        private void OnDestroy()
        {
            // Delete the events
            OnValueChange = null;
        }
    }
}
