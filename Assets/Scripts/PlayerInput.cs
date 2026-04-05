using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    private readonly string hMoveAxis = "Horizontal";
    private readonly string vMoveAxis = "Vertical";

    //다른 스크립트에서 읽을 수 있도록 프로퍼티 설정
    public float H_Move {  get; private set; } 
    public float V_Move { get; private set; }
    public Vector3 MouseWorldPosition { get; private set; }

    public Camera mainCamera;
    

    private void start()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        H_Move = Input.GetAxisRaw(hMoveAxis);
        V_Move = Input.GetAxisRaw(vMoveAxis);
        
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            MouseWorldPosition = hit.point;
        }

    }
}
