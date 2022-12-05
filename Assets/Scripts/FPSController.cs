using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSController : MonoBehaviour {

    public float walkingSpeed = 7.5f;
    public float rotationSpeed = 2.0f;
    public float rotationXLimit = 45.0f;
    public Vector3SO playerCurrentPosition;
    public Vector3SO playerCurrentRotation;

    private Camera playerCamera;
    private CharacterController characterController;

    private Vector3 moveDirection;
    private float rotationX;
    private float rotationY;

    void Start() {
        playerCamera = Camera.main;
        characterController = playerCamera.transform.parent.GetComponent<CharacterController>();
        rotationX = playerCurrentRotation.value.x;
        rotationY = playerCurrentRotation.value.y;
    }

    void Update() {
        //Update the player position and rotation as soon as the scriptable objects are updated from elsewhere (for instance when resetting the game)
        if (playerCamera.transform.parent.transform.position != playerCurrentPosition.value || playerCamera.transform.eulerAngles != playerCurrentRotation.value) {
            playerCamera.transform.parent.transform.position = playerCurrentPosition.value;
            playerCamera.transform.eulerAngles = playerCurrentRotation.value;
            rotationX = playerCurrentRotation.value.x;
            rotationY = playerCurrentRotation.value.y;
            Physics.SyncTransforms();
        } else {

            //Update the position of the player given the keyboard inputs
            Vector3 forward = playerCamera.transform.forward;
            forward.y = 0;
            forward.Normalize();
            Vector3 right = playerCamera.transform.right;

            float speedZ = Input.GetAxis("Horizontal") * walkingSpeed;
            float speedX = Input.GetAxis("Vertical") * walkingSpeed;

            moveDirection = right * speedZ + forward * speedX;

            characterController.Move(moveDirection * Time.deltaTime);

            //Update the rotation of the player given the mouse inputs
            rotationX += -Input.GetAxis("Mouse Y") * rotationSpeed;
            rotationX = Mathf.Clamp(rotationX, -rotationXLimit, rotationXLimit);
            rotationY += Input.GetAxis("Mouse X") * rotationSpeed;
            playerCamera.transform.localRotation = Quaternion.Euler(rotationX, rotationY, 0);

        }
        playerCurrentPosition.value = playerCamera.transform.parent.transform.position;
        playerCurrentRotation.value = playerCamera.transform.eulerAngles;
    }
}
