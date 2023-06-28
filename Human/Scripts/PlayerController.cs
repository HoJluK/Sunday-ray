using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PlayerController : MonoBehaviour
{
    public Joystick joystick;
    public Button jumpButton;
    public Button shootButton;
    public float moveSpeed = 5f;
    public float jumpForce = 5f;

    private Animator animator;
    private Rigidbody rb;

    private bool isJumping = false;
    private bool isShooting = false;

    private void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();

        jumpButton.onClick.AddListener(Jump);

        EventTrigger shootTrigger = shootButton.gameObject.AddComponent<EventTrigger>();

        EventTrigger.Entry pointerDownEntry = new EventTrigger.Entry();
        pointerDownEntry.eventID = EventTriggerType.PointerDown;
        pointerDownEntry.callback.AddListener((data) => { StartShoot(); });

        EventTrigger.Entry pointerUpEntry = new EventTrigger.Entry();
        pointerUpEntry.eventID = EventTriggerType.PointerUp;
        pointerUpEntry.callback.AddListener((data) => { StopShoot(); });

        shootTrigger.triggers.Add(pointerDownEntry);
        shootTrigger.triggers.Add(pointerUpEntry);
    }

    private void Update()
    {
        float horizontalInput = joystick.Horizontal;
        float verticalInput = joystick.Vertical;

        Vector3 movement = new Vector3(horizontalInput, 0f, verticalInput) * moveSpeed * Time.deltaTime;
        animator.SetFloat("Speed", movement.magnitude);
        rb.MovePosition(rb.position + transform.TransformDirection(movement));

        if (movement != Vector3.zero)
        {
            Quaternion toRotation = Quaternion.LookRotation(movement, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, Time.deltaTime * 200f);
        }

        if (isJumping && !animator.GetCurrentAnimatorStateInfo(0).IsName("Jump"))
        {
            isJumping = false;
            animator.SetTrigger("Land");
        }

        if (isShooting)
        {
            animator.CrossFade("Shooting", 0f);
        }
    }

    private void Jump()
    {
        if (!isJumping)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isJumping = true;
            animator.SetTrigger("Jump");
        }
    }

    private void StartShoot()
    {
        isShooting = true;
    }

    private void StopShoot()
    {
        isShooting = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            if (isJumping)
            {
                isJumping = false;
                animator.SetTrigger("Land");
            }
        }
    }
}
