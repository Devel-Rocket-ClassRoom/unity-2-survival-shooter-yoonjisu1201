using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    private readonly string hMoveAxis = "Horizontal";
    private readonly string vMoveAxis = "Vertical";
    private readonly string mRotationAxis = "Mouse X";
    //private readonly string fireButton = "Fire1";


    public float H_Move {  get; private set; } 
    public float V_Move { get; private set; }
    public float MouseRotatiton { get; private set; }
    //public bool FireButton { get; private set; }

    private void Update()
    {
        H_Move = Input.GetAxisRaw(hMoveAxis);
        V_Move = Input.GetAxisRaw(vMoveAxis);
        MouseRotatiton = Input.GetAxis(mRotationAxis);
        //FireButton = Input.GetButton(fireButton);
    }
}
