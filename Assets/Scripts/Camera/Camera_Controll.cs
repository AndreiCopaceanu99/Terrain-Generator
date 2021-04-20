using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera_Controll : MonoBehaviour
{
    [SerializeField]
    float Speed;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Input.GetKey(KeyCode.D))
        {
            transform.position += transform.right * Speed * Time.fixedDeltaTime;
        }
        if (Input.GetKey(KeyCode.A))
        {
            transform.position -= transform.right * Speed * Time.fixedDeltaTime;
        }
        if (Input.GetKey(KeyCode.W))
        {
            transform.position += transform.forward * Speed * Time.fixedDeltaTime;
        }
        if (Input.GetKey(KeyCode.S))
        {
            transform.position -= transform.forward * Speed * Time.fixedDeltaTime;
        }
        if (Input.GetKey(KeyCode.E))
        {
            transform.Rotate(0, 100 * Time.fixedDeltaTime, 0, Space.World);
        }
        if (Input.GetKey(KeyCode.Q))
        {
            transform.Rotate(0, -100 * Time.fixedDeltaTime, 0, Space.World);
        }
    }
}
