using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private PlayerInput playerInput;
    private Rigidbody playerRb;
    private Animator playerAnimator;

    public Transform cameraTransform;

    public float movespeed = 5;
    public float rotateSpeed = 250f;
    public float followSpeed = 5;
    public Vector3 camaraOfset = new Vector3(0f, 5f, -4f);

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
        var angle = playerInput.MouseRotatiton * rotateSpeed * Time.deltaTime;
        playerRb.MoveRotation(transform.rotation * Quaternion.Euler(0, angle, 0));

        //캐릭터 이동량 계산
        var new_vPosition = playerInput.transform.forward * playerInput.V_Move * movespeed * Time.deltaTime;
        var new_hPosition = playerInput.transform.right * playerInput.H_Move * movespeed * Time.deltaTime;

        playerRb.MovePosition(transform.position + new_vPosition + new_hPosition);
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
        FollowCamera();
    }

    public void FollowCamera()
    {
        Vector3 targetPos = transform.TransformPoint(camaraOfset);
        cameraTransform.position = Vector3.Lerp(cameraTransform.position, targetPos, 5f * Time.deltaTime);
        cameraTransform.LookAt(transform.position + Vector3.up * 3f);
    }
}
