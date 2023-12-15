using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BulletController : MonoBehaviour
{

    public float speed;
    public float acceleration;
    private GameObject target;
    private int dmg;

    // Update is called once per frame
    void Update()
    {
        if (target == null)
        {
            Destroy(gameObject);
        }
        else
        {
            speed += acceleration*2;
            transform.position = Vector2.MoveTowards(transform.position, target.transform.position, speed * Time.deltaTime);
            if (Vector2.Distance(transform.position, target.transform.position) <= 0.1f)
            {
                target.GetComponent<CombatController>().receiveDmg(dmg);
                Destroy(gameObject);
            }
        }
    }

    public void setTarget(GameObject target)
    {
        this.target = target;
        
    }

    public void setDmg(int dmg)
    {
        this.dmg = dmg;
    }
}
