using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            Vector3 dir = (Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x,
            Input.mousePosition.y,
            10)) - transform.position).normalized;
            //transform.GetComponent<Rigidbody2D>().velocity = dir * 10f;
            transform.GetComponent<Rigidbody2D>().AddForce(dir * 5f, ForceMode2D.Force);
            transform.GetComponent<Rigidbody2D>().velocity = Vector2.ClampMagnitude(transform.GetComponent<Rigidbody2D>().velocity, 10f);
        }
    }
}
