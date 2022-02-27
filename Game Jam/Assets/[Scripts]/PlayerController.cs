using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public Rigidbody rigidBody;
    public GameObject followTransform;

    [SerializeField] float walkSpeed = 5;
    [SerializeField] float jumpForce = 5;

    Vector2 inputVector = Vector2.zero;
    Vector3 moveDirection = Vector3.zero;
    Vector2 lookInput = Vector2.zero;

    public float aimSensitivity = 0.2f;

    bool isJumping = false;
    bool isRunning = false;

    public AudioSource audioSource;
    public AudioClip footstepSounds;

    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        followTransform.transform.rotation *= Quaternion.AngleAxis(lookInput.x * aimSensitivity, Vector3.up);
        var angles = followTransform.transform.localEulerAngles;
        angles.z = 0;

        //rotate the player to face where we are looking
        transform.rotation = Quaternion.Euler(0, followTransform.transform.rotation.eulerAngles.y, 0);
        followTransform.transform.localEulerAngles = new Vector3(angles.x, 0, 0);

        if (isJumping) return;
        if (!(inputVector.magnitude > 0)) moveDirection = Vector3.zero;

        moveDirection = transform.forward * inputVector.y + transform.right * inputVector.x;

        Vector3 movementDirection = moveDirection * (walkSpeed * Time.deltaTime);

        transform.position += movementDirection;

        if (movementDirection.magnitude > 0) PlayFootstepSounds();
    }

    public void OnMovement(InputValue value)
    {
        inputVector = value.Get<Vector2>();
    }

    public void OnJump(InputValue value)
    {
        if (isJumping)
        {
            return;
        }
        isJumping = true;
        GetComponent<Rigidbody>().AddForce((transform.up + moveDirection) * jumpForce, ForceMode.Impulse);
    }

    public void OnLook(InputValue value)
    {
        lookInput = value.Get<Vector2>();
    }

    float time = 0;
    public void PlayFootstepSounds()
    {
        audioSource.volume = 0.5f;
        if (time < 0.5f)
        {
            time += Time.deltaTime;
        }
        else
        {

            audioSource.Play();
            time = 0;
        }
    }

}
