using System;
using UnityEngine;
using UnityEngine.AI;

public class MenteColmenaController: MonoBehaviour
{
    private const int KEEP = 0;
    private const int REDUCE = -1;
    private const int REDUCE_DOUBLE = -2;
    private const int INCREASE = 1;

    [Header("Gestión de la energía")]
    private float currentEnergy;
    public float baseEnergy;
    public float cycleTime;
    private float currentEnergyMultiplier;
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
    public int threatLevel;
    private int groupAttackNum;
    private int groupAttackCounter;
    private int totalCycles;
    private int angerLvl;
    public int lastCyclePlayerUnitsKilled;

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
                if(timeBetweenCycleTimer >= timeBetweenCycle && !spaceship.GetComponent<Spaceship_C>().getGameHasFinished())
                {
                    peaceBetweenCycle = false;
                    getEnergy();
                    setSpawnStrategy();
                    cycleTimer = 0;
                    timeBetweenCycleTimer = 0;
                }
            }
            else
            {
                spawnTimer += Time.deltaTime;
                if (spawnTimer >= currentSpawnInterval && unitsLeftToSpawn > 0)
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
        currentEnergyMultiplier += energyMultiplierFactor;
        currentEnergy += baseEnergy * currentEnergyMultiplier;
        if(!startFirstCycle)
        {
            totalCycles++;
            updateAngerLevel();
        }
    }

    //Calcular las unidades que va a invocar en la siguiente franja de tiempo
    private void setSpawnStrategy()
    {
        if(GameManager.currentLevel == 0)
        {
            if (startFirstCycle)
            {
                threatLevel = 0;
            }
            else
            {
                switch (getNewThreatLevel())
                {
                    case REDUCE: //Reducir el nivel de amenaza
                        if (threatLevel > 0)
                        {
                            threatLevel--;
                        }
                        break;
                    case INCREASE: //Aumentar el nivel de amenaza
                        if (threatLevel < 2)
                        {
                            threatLevel++;
                        }
                        break;
                    case REDUCE_DOUBLE:
                        if (threatLevel == 2)
                        {
                            threatLevel -= 2;
                        }
                        else if (threatLevel > 0)
                        {
                            threatLevel--;
                        }
                        break;
                    default: threatLevel = KEEP; break;
                }
            }
        }
        else //El sistema diseñado para el nivel 0 no funcionará bien en el nivel 1, así que por falta de tiempo en este nivel hará rotaciones de estrategias
        {
            if (startFirstCycle)
            {
                threatLevel = 0;
            }
            else
            {
                switch(threatLevel)
                {
                    case 0: threatLevel = 1; break;
                    case 1: threatLevel = 2; break;
                    case 2: threatLevel = 0; break;
                }
            }
        }

        lastCyclePlayerUnitsKilled = 0;

        unitsLeftToSpawn = Mathf.RoundToInt(currentEnergy / desertScarabCost); //Calcula cuantas unidades puede generar con la energía que tiene
        currentSpawnInterval = (cycleTime * 0.9f) / ((float)unitsLeftToSpawn); //Calcula cada cuanto tiene que generar una unidad para que le de tiempo antes del cambio de intervalo
        currentEnergy -= desertScarabCost * unitsLeftToSpawn; //Resta la energía de todas las unidades que va a generar en este intervalo
        Debug.Log("Nuevo ciclo: Es el número " + totalCycles);
        Debug.Log("Saldran " + unitsLeftToSpawn + " unidades, una cada " + currentSpawnInterval + " segundos");
        //Debug.Log("El nivel ira es: " + angerLvl);
        //Debug.Log("El nivel de amenaza es: " + threatLevel);
        //Setea las variables para dividir la fase en tres ataques
        groupAttackCounter = 0;
        if (threatLevel == 1)
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
        //Debug.Log("Quedan " + unitsLeftToSpawn +" por spawnear");

        switch (threatLevel)
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
                if (!playerUnits[i].GetComponent<CombatController>().isBeingDeployed && NavMesh.CalculatePath(asker.transform.position, playerUnits[i].transform.position, NavMesh.AllAreas, path))
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
            FindObjectOfType<AudioManager>().Play("BigAttackHorn");
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
        return spawnPoints[UnityEngine.Random.Range(0, spawnPoints.Length)];
    }

    public void updateEnemiesTarget()
    {
        GameObject[] swarm = GameObject.FindGameObjectsWithTag("EnemyUnit");
        if(swarm.Length > 0)
        {
            for (int i = 0; i < swarm.Length; i++)
            {
                swarm[i].GetComponent<EnemyController>().setTarget(findNearestPlayerUnit(swarm[i]));
            }
        }
    }

    public void addUnitKilled()
    {
        lastCyclePlayerUnitsKilled++;
    }

    private int getNewThreatLevel()
    {
        //En cualquier fase de la partida, si no se ha destruido ninguna unidad del jugador la amenaza debe aumentar.
        //Debug.Log("En este ciclo han muerto "+ lastCyclePlayerUnitsKilled + " unidades del jugador");
        if (lastCyclePlayerUnitsKilled == 0) 
        {
            return INCREASE;
        }

        //En cualquier fase de la partida, si pasado el tiempo entre ciclos quedan unidades enemigas vivas, la amenaza se debe reducir
        if(GameObject.FindGameObjectsWithTag("EnemyUnit").Length > 0)
        {
            return REDUCE;
        }

        switch (angerLvl)
        {
            case 0: //Fase 1: Si ha matado 1 unidad se mantiene y si ha matado más de 1 se reduce
                if (lastCyclePlayerUnitsKilled == 1) return KEEP;
                else if (lastCyclePlayerUnitsKilled == 2) return REDUCE;
                else return REDUCE_DOUBLE;
            case 1: //Fase 2: Si ha matado 1 unidad aumenta la amenaza, si ha matado 2 se mantiene y si ha matado 3 o más se reduce
                if (lastCyclePlayerUnitsKilled <= 2) return KEEP;
                else if (lastCyclePlayerUnitsKilled <= 4) return REDUCE;
                else return REDUCE_DOUBLE;
            case 2: //Fase 3: Si ha matado menos de 3 unidades aumenta la amenaza, si ha matado entre 3 y 6 se mantiene y si ha matado más de 6 se reduce
                if (lastCyclePlayerUnitsKilled <= 2) return INCREASE;
                else if (lastCyclePlayerUnitsKilled == 3) return KEEP;
                else if (lastCyclePlayerUnitsKilled <= 4) return REDUCE;
                else return REDUCE_DOUBLE;
            case 3: //Fase 4: A por todas
                if (lastCyclePlayerUnitsKilled <= 4) return INCREASE;
                else if (lastCyclePlayerUnitsKilled == 6) return KEEP;
                else return REDUCE;
            default: return KEEP; //Nunca debería entrar en este default pero hay que ponerlo para que siempre devuelva algo
        }
    }

    private void updateAngerLevel()
    {
        if (totalCycles <= 4) angerLvl = 0;
        else if (totalCycles <= 7) angerLvl = 1;
        else if (totalCycles <= 12) angerLvl = 2;
        else angerLvl = 3;
    }
}
