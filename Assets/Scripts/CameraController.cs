using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    //// Start is called before the first frame update
    //void Start()
    //{
    //    transform.Rotate(0, 0, 0);
    //}

    //// Update is called once per frame
    //void Update()
    //{
    //    if (Input.GetKey(KeyCode.UpArrow))
    //    {
    //        transform.Translate(Vector3.up * Time.deltaTime * 3);
    //    }
    //    if (Input.GetKey(KeyCode.DownArrow))
    //    {
    //        transform.Translate(Vector3.down * Time.deltaTime * 3);
    //    }
    //    if (Input.GetKey(KeyCode.Q))
    //    {
    //        transform.Translate(Vector3.left * Time.deltaTime * 3);
    //    }
    //    if (Input.GetKey(KeyCode.D))
    //    {
    //        transform.Translate(Vector3.right * Time.deltaTime * 3);
    //    }
    //    if (Input.GetKey(KeyCode.Z))
    //    {
    //        transform.Translate(Vector3.forward * Time.deltaTime * 3);
    //    }
    //    if (Input.GetKey(KeyCode.S))
    //    {
    //        transform.Translate(Vector3.back * Time.deltaTime * 3);
    //    }

    //    if (Input.mousePosition.x > 0 && Input.mousePosition.x < Screen.width && Input.mousePosition.y > 0 && Input.mousePosition.y < Screen.height)
    //    {
    //        float h = Input.GetAxis("Mouse X");
    //        float v = -1 * Input.GetAxis("Mouse Y");
    //        transform.Rotate(v, h, 0);
    //    }


    //}

    [SerializeField]
    float mouseSensitivity;

    float xAxisClamp = 0;


    [SerializeField]
    Transform player, playerCross;

    void Update()
    {
        Cursor.lockState = CursorLockMode.Locked;
        RotateCamera();
    }

    void RotateCamera()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        float rotAmountX = mouseX * mouseSensitivity;
        float rotAmountY = mouseY * mouseSensitivity;

        xAxisClamp += rotAmountY;

        Vector3 rotPlayerCross = playerCross.transform.rotation.eulerAngles;
        Vector3 rotPlayer = player.transform.rotation.eulerAngles;

        rotPlayerCross.z += rotAmountY;
        rotPlayerCross.x = 0;
        rotPlayer.y += rotAmountX;

        if(xAxisClamp > 90)
        {
            xAxisClamp = 90;
            rotPlayerCross.z = 90;
        }

        else if(xAxisClamp < -90)
        {
            xAxisClamp = -90;
            rotPlayerCross.z = 270;
        }
        playerCross.rotation = Quaternion.Euler(rotPlayerCross);
        player.rotation = Quaternion.Euler(rotPlayer);

    }


}
