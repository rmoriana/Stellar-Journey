using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.GraphicsBuffer;

public class EnemyController : MonoBehaviour
{

    private const int IDLE = 0;
    private const int MOVING_TO_TARGET = 1;
    private const int ATTACKING = 2;

    public int currentState;
    public NavMeshAgent agent;
    public GameObject currentTarget;
    public GameObject menteColmena;

    [Header("Movimiento IDLE")]
    public float minIdleMoveTime;
    public float maxIdleMoveTime;
    private float currentIdleMoveTime;
    public float minDistance;
    public float maxDistance;
    private float idleMoveTimer;


    void Start()
    {
        currentIdleMoveTime = Random.Range(minIdleMoveTime, maxIdleMoveTime);
    }
    // Update is called once per frame
    void Update()
    {
        if (GetComponent<CombatController>().isBeingDeployed)
        {
            return;
        }

        stateMachine();
    }

    private void stateMachine()
    {
        switch (currentState)
        {
            case IDLE:
                if (idleMoveTimer >= currentIdleMoveTime)
                {
                    agent.isStopped = true;
                    idleMoveTimer = 0;
                    move();
                }
                else
                {
                    idleMoveTimer = idleMoveTimer += Time.deltaTime;
                }
                break;
            case MOVING_TO_TARGET:
                agent.isStopped = false;
                if (currentTarget != null)
                {
                    agent.destination = currentTarget.transform.position;
                    if (Vector2.Distance(transform.position, currentTarget.transform.position) < 1f)
                    {
                        currentState = ATTACKING;
                    }
                }
                else
                {
                    getClosestTarget();
                }
                break;
            case ATTACKING:
                agent.isStopped = true;
                if (currentTarget != null)
                {
                    GetComponent<CombatController>().attack(currentTarget);
                }
                else
                {
                    getClosestTarget();
                }
                break;
        }
    }

    //Recibe la orden de cambiar de estado
    public void changeState(int newState)
    {
        currentState = newState;
    }

    //Recibe un objetivo
    public void setTarget(GameObject target)
    {
        if (target != null)
        {
            currentTarget = target;
        }
    }

    //Recibe un objetivo y la orden de atacarle immediatamente
    public void setTargetAndAttack(GameObject target)
    {
        if (target != null)
        {
            currentTarget = target;
            currentState = MOVING_TO_TARGET;
        }
    }

    //Recibe la orden de atacar al objetivo
    public void attack()
    {
        currentState = MOVING_TO_TARGET;
    }

    //Obtiene la unidad enemiga mas cercana o en su defecto la nave
    private void getClosestTarget()
    {
        currentTarget = menteColmena.GetComponent<MenteColmenaController>().findNearestPlayerUnit(gameObject);
        if (currentTarget == null) //La única unidad es la nave por lo que ataca a la nave
        {
            currentTarget = GameObject.Find("Spaceship_P");
            if (currentTarget == null)
            {
                currentState = IDLE;
            }
            else
            {
                currentState = MOVING_TO_TARGET;
            }
        }
        else
        {
            currentState = MOVING_TO_TARGET;
        }
    }

    //Se mueve a una posición cercana
    private void move()
    {
        agent.isStopped = false;
        int contadorAux = 0;
        bool spotFound = false;
        float moveDistance = 0;
        Vector2 possiblePos = transform.position;
        int layerMask = 1 << LayerMask.NameToLayer("Pared"); //Así detecta solo esta capa
        while (!spotFound && contadorAux < 20)
        {
            moveDistance = Random.Range(minDistance, maxDistance);
            possiblePos = new Vector2(possiblePos.x + moveDistance, possiblePos.y + moveDistance);

            RaycastHit2D hit = Physics2D.Linecast(transform.position, possiblePos, layerMask);
            Debug.DrawLine(transform.position, possiblePos, Color.red);
            if (hit != false && hit.transform.tag != "Pared")
            {
                spotFound = true;
            }

            contadorAux++; //Para evitar el bucle infinito
        }

        agent.SetDestination(possiblePos);
    }
}
