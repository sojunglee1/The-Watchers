using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public Rigidbody rigidBody;
    public GameObject followTransform;

    [SerializeField] float walkSpeed = 5;

    Vector2 inputVector = Vector2.zero;
    Vector3 moveDirection = Vector3.zero;
    Vector2 lookInput = Vector2.zero;

    public float aimSensitivity = 0.2f;

    bool isJumping = false;

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

        transform.rotation = Quaternion.Euler(0, followTransform.transform.rotation.eulerAngles.y, 0);
        followTransform.transform.localEulerAngles = new Vector3(angles.x, 0, 0);

        if (isJumping) return;
        if (!(inputVector.magnitude > 0)) moveDirection = Vector3.zero;

        moveDirection = transform.forward * inputVector.y + transform.right * inputVector.x;

        Vector3 movementDirection = moveDirection * (walkSpeed * Time.deltaTime);

        transform.position += movementDirection;

        if (movementDirection.magnitude > 0) PlayFootstepSounds();

        //if (isSquished()) GameManager.instance.PlayerDied = true;
    }

    public void OnMovement(InputValue value)
    {
        if (!GameManager.instance.isLetterShowing() && !GameManager.isGamePaused() && !GameManager.instance.PlayerDied)
            inputVector = value.Get<Vector2>();
        else 
            inputVector = Vector3.zero;
    }

    public void OnLook(InputValue value)
    {
        if (!GameManager.instance.isLetterShowing() && !GameManager.isGamePaused() && !GameManager.instance.PlayerDied)
            lookInput = value.Get<Vector2>();
        else
            lookInput = Vector3.zero;
    }
    public void OnCloseLetter()
    {
        GameManager.instance.CloseLetter();
    }

    public void OnPause()
    {
        GameManager.instance.Pause();
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

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Finish"))
        {
            GameManager.instance.EndingLetter.SetActive(true);
            GameManager.instance.EndingLetter.GetComponent<Animator>().Play("OpenLetter");
        }
    }

    private bool isSquished()
    {
        Ray frontRay = new Ray(transform.position, Vector3.forward);
        Ray backRay = new Ray(transform.position, -Vector3.forward);
        Ray leftRay = new Ray(transform.position, -Vector3.right);
        Ray rightRay = new Ray(transform.position, Vector3.right);

        //Debug.DrawRay(frontRay.origin, frontRay.direction * 0.5f);
        //Debug.DrawRay(backRay.origin, backRay.direction * 0.5f);
        //Debug.DrawRay(leftRay.origin, leftRay.direction * 0.5f);
        //Debug.DrawRay(rightRay.origin, rightRay.direction * 0.5f);

        RaycastHit hitData;

        bool forward = false, backwards = false, left = false, right = false;

        forward = Physics.Raycast(frontRay, out hitData, 0.5f) && hitData.collider.CompareTag("Wall");
        backwards = Physics.Raycast(backRay, out hitData, 0.5f) && hitData.collider.CompareTag("Wall");
        left = Physics.Raycast(leftRay, out hitData, 0.5f) && hitData.collider.CompareTag("Wall");
        right = Physics.Raycast(rightRay, out hitData, 0.5f) && hitData.collider.CompareTag("Wall");

        if (forward && backwards || left && right) return true;
        else return false;

    }
}
