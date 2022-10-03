using qASIC.InputManagement;
using UnityEngine;

namespace Game.Player
{
    public class PlayerFlightController : MonoBehaviour
    {
        [SerializeField] float speed = 20f;
        [SerializeField] InputMapItemReference inputHorizontal;
        [SerializeField] InputMapItemReference inputVertical;

        private void FixedUpdate()
        {
            Vector3 move = new Vector2(InputManager.GetFloatInput(inputHorizontal.GetGroupName(), inputHorizontal.GetItemName()),
                InputManager.GetFloatInput(inputVertical.GetGroupName(), inputVertical.GetItemName()));
            transform.position += move * Time.fixedDeltaTime * speed;
        }
    }
}
