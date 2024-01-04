using UnityEngine;
using RocknFall.Bases.SO;

namespace RocknFall.UIHandlers
{
    public class MenuScreenSetter : MonoBehaviour
    {
        [SerializeField] MainCamera mainCamera;
        [SerializeField] Transform rightPart;
        [SerializeField] Transform leftPart;

        void Start()
        {
            // Get the screen positions of the walls
            Vector2 rightScreenPos = new Vector2(Screen.width, 0f);
            Vector2 leftScreenPos = new Vector2(0f, 0f);

            // Set the part at their right positions
            rightPart.position = new Vector2(mainCamera.Cam.ScreenToWorldPoint(rightScreenPos).x, 0f);
            leftPart.position = new Vector2(mainCamera.Cam.ScreenToWorldPoint(leftScreenPos).x, 0f);
        }
    }
}
