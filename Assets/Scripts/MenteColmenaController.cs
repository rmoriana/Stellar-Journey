using UnityEngine;
using UnityEngine.AI;

public class MenteColmenaController: MonoBehaviour
{
    [Header("Gestión de la energía")]
    private float currentEnergy;
    public float baseEnergy;
    public float cycleTime;
    public float currentEnergyMultiplier;
    public float energyMultiplierFactor;
    private float cycleTimer;
    public float timeBetweenCycle;
    private float timeBetweenCycleTimer;
    private bool peaceBetweenCycle;

    [Header("Coste de los enemigos")]
    public int desertScarabCost;

    private float currentSpawnInterval;
    private float spawnTimer;
    private int unitsLeftToSpawn;

    [Header("Amenaza")]
    public int threadLevel;
    private int groupAttackNum;
    private int groupAttackCounter;

    public GameObject spaceship;
    public float peaceTime;
    private float peaceTimer;
    public GameObject[] spawnPoints;  
    private bool startFirstCycle;
    private bool firstCycleFinished;

    // Update is called once per frame
    void Update()
    {
        if (startFirstCycle)
        {
            if (peaceTime > peaceTimer)
            {
                peaceTimer += Time.deltaTime;
            }
            else
            {
                getEnergy();
                setSpawnStrategy();
                startFirstCycle = false;
                firstCycleFinished = true;
            }
        }

        if (firstCycleFinished)
        {
            
            if (cycleTimer >= cycleTime)
            {
                peaceBetweenCycle = true;
            }
            else
            {
                cycleTimer += Time.deltaTime;
            }

            if (peaceBetweenCycle)
            {
                timeBetweenCycleTimer += Time.deltaTime;
                if(timeBetweenCycleTimer >= timeBetweenCycle)
                {
                    peaceBetweenCycle = false;
                    currentEnergyMultiplier += energyMultiplierFactor;
                    getEnergy();
                    setSpawnStrategy();
                    cycleTimer = 0;
                    timeBetweenCycleTimer = 0;
                }
            }

            if (!peaceBetweenCycle)
            {
                spawnTimer += Time.deltaTime;
                if (spawnTimer >= currentSpawnInterval)
                {
                    spawnUnit();
                    spawnTimer = 0;
                }
            }
        }
    }

    //Obtiene la energía de la siguiente franja de tiempo
    private void getEnergy()
    {
        currentEnergy += baseEnergy * currentEnergyMultiplier;
    }

    //Calcular las unidades que va a invocar en la siguiente franja de tiempo
    private void setSpawnStrategy()
    {
        //Lógica simple y temporal del nivel de amenaza
        if (threadLevel == 0)
        {
            threadLevel = 1;
        }
        else if (threadLevel == 1)
        {
            threadLevel = 2;
        }
        else
        {
            threadLevel = 0;
        }

        unitsLeftToSpawn = Mathf.RoundToInt(currentEnergy / desertScarabCost); //Calcula cuantas unidades puede generar con la energía que tiene
        currentSpawnInterval = (cycleTime * 0.9f) / ((float)unitsLeftToSpawn); //Calcula cada cuanto tiene que generar una unidad para que le de tiempo antes del cambio de intervalo
        currentEnergy -= desertScarabCost * unitsLeftToSpawn; //Resta la energía de todas las unidades que va a generar en este intervalo
        //Debug.Log("Nuevo ciclo");
        //Debug.Log("Saldran " + unitsLeftToSpawn + " unidades, una cada " + currentSpawnInterval + " segundos");
        //Setea las variables para dividir la fase en tres ataques
        groupAttackCounter = 0;
        if (threadLevel == 1)
        {
            groupAttackNum = Mathf.RoundToInt(unitsLeftToSpawn / 3);
        }
    }

    //Invoca una nueva unidad
    private void spawnUnit()
    {
        GameObject spawnPoint = getSpawnPoint();
        GameObject newUnit = Instantiate(Resources.Load("Prefabs/DesertScarab"), spawnPoint.transform.position, new Quaternion(0, 0, 0, 0)) as GameObject;
        newUnit.GetComponent<EnemyController>().menteColmena = gameObject;
        newUnit.GetComponent<EnemyController>().agent = newUnit.GetComponent<NavMeshAgent>();
        newUnit.GetComponent<EnemyController>().spaceship = spaceship;
        newUnit.GetComponent<EnemyController>().currentState = 0;
        newUnit.GetComponent<CombatController>().isBeingDeployed = true;

        unitsLeftToSpawn--;

        switch (threadLevel)
        {
            case 0:
                spawnAndAttack(newUnit);
                break;
            case 1:
                tripleAttack();
                break;
            case 2:
                oneBigAttack();
                break;
        }
    }

    //Devuelve la unidad del jugador más cercana.
    public GameObject findNearestPlayerUnit(GameObject asker)
    {
        GameObject[] playerUnits = GameObject.FindGameObjectsWithTag("PlayerUnit");
        NavMeshPath path = new NavMeshPath();
        if (playerUnits.Length > 0)
        {
            GameObject nearestPlayerUnit = null;
            float distance = 9999;
            for (int i = 0; i < playerUnits.Length; i++)
            {
                if (NavMesh.CalculatePath(asker.transform.position, playerUnits[i].transform.position, NavMesh.AllAreas, path))
                {
                    if (path.status == NavMeshPathStatus.PathComplete)
                    {
                        asker.GetComponent<NavMeshAgent>().path = path;
                        if (nearestPlayerUnit == null)
                        {
                            nearestPlayerUnit = playerUnits[i];
                            distance = ExtensionMethods.GetPathRemainingDistance(asker.GetComponent<NavMeshAgent>());
                        }
                        else
                        {
                            if (distance > ExtensionMethods.GetPathRemainingDistance(asker.GetComponent<NavMeshAgent>()))
                            {
                                nearestPlayerUnit = playerUnits[i];
                                distance = ExtensionMethods.GetPathRemainingDistance(asker.GetComponent<NavMeshAgent>());
                            }
                        }
                    }
                }
            }

            if(nearestPlayerUnit == GameObject.Find("Spaceship_P"))
            {
                if (nearestPlayerUnit.GetComponent<Spaceship_C>().getGameHasFinished())
                {
                    return null;
                }
            }

            return nearestPlayerUnit;
        }

        return null;
    }

    private void spawnAndAttack(GameObject newUnit)
    {
        newUnit.GetComponent<EnemyController>().setTargetAndAttack(findNearestPlayerUnit(newUnit));
    }

    private void oneBigAttack()
    {
        if (unitsLeftToSpawn == 0)
        {
            GameObject[] swarm = GameObject.FindGameObjectsWithTag("EnemyUnit");
            for (int i = 0; i < swarm.Length; i++)
            {
                swarm[i].GetComponent<EnemyController>().changeState(1);
                swarm[i].GetComponent<EnemyController>().setTargetAndAttack(findNearestPlayerUnit(swarm[i]));
            }
        }
    }

    private void tripleAttack()
    {
        groupAttackCounter++;
        if (unitsLeftToSpawn == 0)
        {
            setSwarmToAttack(GameObject.FindGameObjectsWithTag("EnemyUnit"));
        }
        else if (groupAttackCounter >= groupAttackNum)
        {
            setSwarmToAttack(GameObject.FindGameObjectsWithTag("EnemyUnit"));
            groupAttackCounter = 0;
        }
    }

    private void setSwarmToAttack(GameObject[] swarm)
    {
        for (int i = 0; i < swarm.Length; i++)
        {
            swarm[i].GetComponent<EnemyController>().changeState(1);
            swarm[i].GetComponent<EnemyController>().setTargetAndAttack(findNearestPlayerUnit(swarm[i]));
        }
    }

    public void setFirstCycle()
    {
        startFirstCycle = true;
    }

    private GameObject getSpawnPoint()
    {
        return spawnPoints[Random.Range(0, spawnPoints.Length)];
    }
}
