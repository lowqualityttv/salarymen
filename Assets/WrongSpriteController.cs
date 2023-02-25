using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WrongSpriteController : MonoBehaviour
{
    [SerializeField] private float speed = 1;

    float x;
    float y;

    public float deathTimer = 1.5f;
    // Start is called before the first frame update
    void Start()
    {
        x = Random.Range(-3f, 3f);
        y = Random.Range(-3f, 3f);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += new Vector3(x, y, 0) * Time.deltaTime * speed;

        deathTimer -= Time.deltaTime;
        if (deathTimer <= 0)
        {
            Destroy(this.gameObject);
        }
    }
}
