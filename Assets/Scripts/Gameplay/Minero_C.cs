using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Minero_C : MonoBehaviour
{
    private const int IDLE = 0;
    private const int MOVING_TO_TARGET = 1;
    private const int MINING = 2;
    private const int DEPLOYING = 3;

    public int cargo;
    public int cargoType;
    public int cargoCapacity;
    public GameObject currentTarget;
    public GameObject targetInteractionPosition;
    public int currentState;
    public GameObject spaceship;
    public NavMeshAgent agent;
    public float nextActionTimer;
    public float miningSpeed;
    private float miningCooldown;
    public float deployingSpeed;
    private float deployingCooldown;

    //Inicializa algunas variables y controla que las unidades no roten
    void Start()
    {
        spaceship = GameObject.Find("Spaceship_P");
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
    }

    //Si la unidad no está siendo colocada, actualiza la máquina de estados
    void Update()
    {
        if (!GetComponent<CombatController>().isBeingDeployed && !spaceship.GetComponent<Spaceship_C>().getGameHasFinished())
        {
            stateMachine();
        }
    }

    private void stateMachine()
    {
        switch (currentState)
        {
            case IDLE: //Si tiene recursos, intentará dejarlos en la nave. Si no tiene o no puede y puede obtener más recursos, intentará minar.
                if (nextActionTimer >= 2)
                {
                    nextActionTimer = 0;
                    getNextAction();
                }
                else
                {
                    nextActionTimer += Time.deltaTime;
                }

                break;
            case MOVING_TO_TARGET: //Se desplaza hacia el objetivo, concretamente hacia el punto desde el cual interactua con el mismo
                if (currentTarget != null && targetInteractionPosition != null)
                {
                    if (Vector2.Distance(transform.position, targetInteractionPosition.transform.position) < 0.1f)
                    {
                        if (currentTarget.CompareTag("Cristal"))
                        {
                            currentState = MINING;
                        }
                        else if (currentTarget.CompareTag("PlayerUnit"))
                        {
                            currentState = DEPLOYING;
                        }
                    }
                }
                else
                {
                    currentState = IDLE;
                    getNextAction();
                }
                break;
            case MINING:
                if (currentTarget != null && targetInteractionPosition != null)
                {
                    if (cargo < cargoCapacity)
                    {
                        if (miningCooldown >= miningSpeed)
                        {
                            cargo++;
                            miningCooldown = 0;
                        }
                        else
                        {
                            miningCooldown += Time.deltaTime;
                        }
                    }
                    else
                    {
                        currentState = IDLE;
                        currentTarget.GetComponent<InteractionPositions>().releaseInteractingPosition(targetInteractionPosition);
                        getNextAction();
                    }
                }
                else
                {
                    currentState = IDLE;
                    getNextAction();
                }
                break;
            case DEPLOYING:
                if (cargo > 0)
                {
                    if (deployingCooldown >= deployingSpeed)
                    {
                        cargo--;
                        currentTarget.GetComponent<Spaceship_C>().deployMineral(0);
                        deployingCooldown = 0;
                    }
                    else
                    {
                        deployingCooldown += Time.deltaTime;
                    }
                }
                else
                {
                    spaceship.GetComponent<InteractionPositions>().releaseInteractingPosition(targetInteractionPosition);
                    currentState = IDLE;
                    getNextAction();
                }
                break;
        }
    }

    //Comprueba si se debe descargar recursos o ir a recolectar mas y establece la siguiente acción en consecuencia
    private void getNextAction()
    {
        if (cargo == cargoCapacity)
        {
            targetInteractionPosition = spaceship.GetComponent<InteractionPositions>().getFreeInteractingPosition();
            if (targetInteractionPosition != null)
            {
                currentState = MOVING_TO_TARGET;
                currentTarget = spaceship;
                agent.destination = targetInteractionPosition.transform.position;
            }
        }
        else if (cargo < cargoCapacity)
        {
            currentTarget = findNearestAvailableMineral();
            if (currentTarget != null)
            {
                currentState = MOVING_TO_TARGET;
                targetInteractionPosition = currentTarget.GetComponent<InteractionPositions>().getFreeInteractingPosition();
                agent.destination = targetInteractionPosition.transform.position;
            }
        }
    }

    //Devuelve la veta de mineral con espacios disponibles más cercana
    private GameObject findNearestAvailableMineral()
    {
        GameObject[] veinArray = GameObject.FindGameObjectsWithTag("Cristal");
        if (veinArray.Length > 0)
        {
            GameObject nearestVein = null;
            float distance = 9999;
            for (int i = 0; i < veinArray.Length; i++)
            {
                if (veinArray[i].GetComponent<InteractionPositions>().checkFreeInteractingPosition())
                {
                    if (nearestVein == null)
                    {
                        nearestVein = veinArray[i];
                        distance = Vector2.Distance(transform.position, nearestVein.transform.position);
                    }
                    else
                    {
                        if (distance > Vector2.Distance(transform.position, veinArray[i].transform.position))
                        {
                            nearestVein = veinArray[i];
                            distance = Vector2.Distance(transform.position, nearestVein.transform.position);
                        }
                    }
                }
            }

            return nearestVein;
        }

        return null;
    }
}