using UnityEngine;
using UnityEngine.InputSystem;

public class MouseController : MonoBehaviour
{
    public float mouseSensitivity = 100.0f;
    public Transform playerBody;

    private float XRotation = 0.0f;

    [SerializeField]
    InputActionAsset inputActions;
    InputAction mousePos;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        mousePos = inputActions.FindAction("Point");
    }

    // Update is called once per frame
    void Update()
    {
        float mouseX = mousePos.ReadValue<Vector2>().x * mouseSensitivity;
        float mouseY = mousePos.ReadValue<Vector2>().y * mouseSensitivity;

        XRotation -= mouseY;
        XRotation = Mathf.Clamp(XRotation, -90.0f, 90.0f);

        transform.localRotation = Quaternion.Euler(XRotation, 0.0f, 0.0f);
        playerBody.Rotate(Vector3.up * mouseX);
    }
}
