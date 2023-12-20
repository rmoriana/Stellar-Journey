using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BulletController : MonoBehaviour
{
    public float speed;
    public float acceleration;
    public float maxSpeed;
    public float maxTimeAlive;
    private GameObject target;
    private int dmg;
    Vector2 vectorDirector;
    private float movementX, movementY;
    private float timeAlive;

    // Update is called once per frame
    void Update()
    {
        //El vector director establece la dirección de la bala. Si se pone en el start la bala irá recta, mientras que si se pone aquí corregirá su dirección para acertar el disparo
        //Si se normalizan los valores la trayectoria de la bala se corrige de forma más suave
        if(target != null)
        {
            vectorDirector = (target.transform.position - transform.position).normalized;
        }

        if(speed < maxSpeed )
        {
            speed += acceleration * 2 * Time.deltaTime;
        }
        
        movementX = speed * vectorDirector.x;
        movementY = speed * vectorDirector.y;
        transform.position = new Vector2(transform.position.x + movementX, transform.position.y + movementY);
        timeAlive += Time.deltaTime;

        if(timeAlive > maxTimeAlive)
        {
            Destroy(gameObject);
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

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("EnemyUnit"))
        {
            other.GetComponent<CombatController>().receiveDmg(dmg);
            Destroy(gameObject);
        }else if (other.CompareTag("Pared"))
        {
            if(timeAlive >= 0.15f) //Evita que se autodestruya nada más crearse por el límite de la colisión con la pared
            {
                Destroy(gameObject);
            }
        }
    }
}
