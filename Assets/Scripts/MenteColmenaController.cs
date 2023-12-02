using UnityEngine;
using UnityEngine.AI;

public class MenteColmenaController: MonoBehaviour
{
    [Header("Gestión de la energía")]
    public float currentEnergy;
    public float updateEnergyGenerationTime;
    public float currentEnergyMultiplier;
    public float energyMultiplierFactor;
    private float updateEnergyGenerationTimer;

    [Header("Coste de los enemigos")]
    public int desertScarabCost;

    private float currentSpawnInterval;
    private float spawnTimer;
    private int unitsLeftToSpawn;

    [Header("Amenaza")]
    public int threadLevel;
    private int groupAttackNum;
    private int groupAttackCounter;

    // Start is called before the first frame update
    void Start()
    {
        getEnergy();
        setSpawnStrategy();
    }
    // Update is called once per frame
    void Update()
    {
        updateEnergyGenerationTimer += Time.deltaTime;
        if (updateEnergyGenerationTimer >= updateEnergyGenerationTime)
        {
            currentEnergyMultiplier += energyMultiplierFactor;
            getEnergy();
            setSpawnStrategy();
            updateEnergyGenerationTimer = 0;
        }

        spawnTimer += Time.deltaTime;
        if (spawnTimer >= currentSpawnInterval)
        {
            spawnUnit();
            spawnTimer = 0;
        }
    }

    //Obtiene la energía de la siguiente franja de tiempo
    private void getEnergy()
    {
        currentEnergy += GameObject.Find("Spaceship_P").GetComponent<Spaceship_C>().getGameEnergy() * currentEnergyMultiplier;
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
        currentSpawnInterval = (updateEnergyGenerationTime * 0.9f) / ((float)unitsLeftToSpawn); //Calcula cada cuanto tiene que generar una unidad para que le de tiempo antes del cambio de intervalo
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
        GameObject newUnit = Instantiate(Resources.Load("Prefabs/DesertScarab"), new Vector3(Random.Range(-3.45f, 3.45f), Random.Range(10.5f, 12.5f), 0), new Quaternion(0, 0, 0, 0)) as GameObject;
        newUnit.GetComponent<EnemyController>().menteColmena = gameObject;
        newUnit.GetComponent<EnemyController>().agent = newUnit.GetComponent<NavMeshAgent>();
        newUnit.GetComponent<NavMeshAgent>().updateRotation = false;
        newUnit.GetComponent<NavMeshAgent>().updateUpAxis = false;
        unitsLeftToSpawn--;

        switch (threadLevel)
        {
            case 0:
                spawnAndAttack(newUnit);
                break;
            case 1:
                tripleAttack(newUnit);
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

            return nearestPlayerUnit;
        }

        return null;
    }

    private void spawnAndAttack(GameObject newUnit)
    {
        newUnit.GetComponent<EnemyController>().changeState(1);
        newUnit.GetComponent<EnemyController>().setTargetAndAttack(findNearestPlayerUnit(newUnit));
    }

    private void oneBigAttack()
    {
        if (unitsLeftToSpawn == 0)
        {
            //Debug.Log("Lanzo ataque");
            GameObject[] swarm = GameObject.FindGameObjectsWithTag("EnemyUnit");
            for (int i = 0; i < swarm.Length; i++)
            {
                swarm[i].GetComponent<EnemyController>().changeState(1);
                swarm[i].GetComponent<EnemyController>().setTargetAndAttack(findNearestPlayerUnit(swarm[i]));
            }
        }
    }

    private void tripleAttack(GameObject newUnit)
    {
        groupAttackCounter++;
        if (unitsLeftToSpawn == 0)
        {
            //Debug.Log("Lanzo ataque");
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
}
