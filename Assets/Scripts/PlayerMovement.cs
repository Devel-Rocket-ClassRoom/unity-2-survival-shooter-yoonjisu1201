using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private PlayerInput playerInput;
    private Rigidbody playerRb;
    private Animator playerAnimator;

    public float movespeed = 5f;


    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        playerRb = GetComponent<Rigidbody>();
        playerAnimator = GetComponent<Animator>();
    }
    private void OnEnable()
    {
        playerAnimator.SetBool("IsMove", false);
    }

    private void FixedUpdate()
    {
        //캐릭터 이동량 계산
        Vector3 moveDir = ((Vector3.forward * playerInput.V_Move) + (Vector3.right * playerInput.H_Move)).normalized;
        Vector3 nextPosition = playerRb.position + (moveDir * movespeed * Time.deltaTime);

        playerRb.MovePosition(nextPosition);

        //캐릭터 마우스 위치에 따른 회전
        Vector3 targetDir = playerInput.MouseWorldPosition - playerRb.position;
        targetDir.y = 0f;

        if (targetDir != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(targetDir);
            playerRb.MoveRotation(targetRotation);
        }
    }
    private void Update()
    {
        if (playerInput.H_Move != 0 || playerInput.V_Move != 0)
        {
            playerAnimator.SetBool("IsMove", true);
        }
        else
        {
            playerAnimator.SetBool("IsMove", false);
        }
        
    }

    
}
