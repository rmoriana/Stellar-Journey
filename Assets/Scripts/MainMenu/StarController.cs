using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class StarController : MonoBehaviour
{

    public float speed;

    // Update is called once per frame
    void Update()
    {
        if (transform.position.y < -10)
        {
            Destroy(gameObject);
        }
        else
        {
            transform.Translate(new Vector2(0, -speed * Time.deltaTime));
        }
    }
}
