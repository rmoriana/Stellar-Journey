using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class UnitDeployment : MonoBehaviour
{
    private const int DEFAULT = 0;
    private const int PRESSED = 2;
    private const int DISABLED = 3;

    public Button[] troopBtns;
    public Sprite[] troopBtnsSprites;
    public GameObject mainCamera;
    private GameObject nuevaUnidad;
    private bool deployingUnit;
    private Vector3 mouseWorldPos;
    private int minerEnergyReq;
    private int defenderEnergyReq;
    private int minerUranioEnergyReq;
    private int unitEnergyReq;
    private string unitName;
    private int numSelectedTroops;
    private int lastUnitType;
    private bool cameraMovedRecently;
    private float cameraMovedRecentlyTimer;

    private void Start()
    {
        //numSelectedTroops = GameObject.Find("Spaceship_P").GetComponent<Spaceship_C>().getNumSelectedTroops();
        if(GameManager.currentLevel == 0)
        {
            numSelectedTroops = 2;
        }
        else if(GameManager.currentLevel == 1)
        {
            numSelectedTroops = 3;
        }

        minerEnergyReq = GameObject.Find("Spaceship_P").GetComponent<Spaceship_C>().getMinerEnergyReq();
        defenderEnergyReq = GameObject.Find("Spaceship_P").GetComponent<Spaceship_C>().getDefenderEnergyReq();
        minerUranioEnergyReq = GameObject.Find("Spaceship_P").GetComponent<Spaceship_C>().getMinerUranioEnergyReq();

        for (int i = 3; i >= numSelectedTroops; i--)
        {
            troopBtns[i].GetComponent<Image>().sprite = troopBtnsSprites[DISABLED];
            troopBtns[i].interactable = false;
        }

        lastUnitType = -1;
    }

    // Update is called once per frame
    void Update()
    {
        if (deployingUnit)
        {
            updateUnitPosition();
        }

        deployKeysListener();

        //Adapta la actualización de la posicion de una tropa que se está desplegando al suavizado del movimiento de la cámara
        if (cameraMovedRecently)
        {
            cameraMovedRecentlyTimer += Time.deltaTime;
            if(cameraMovedRecentlyTimer > 2)
            {
                cameraMovedRecently = false;
            }
        }
    }

    //Identifica la unidad que se quiere desplegar y comprueba si el jugador tiene energía suficiente
    public void selectUnit(int unitType)
    {
        if (!checkIfEnoughEnergy(unitType))
        {
            //TODO: Mensaje de que no hay energía para esa unidad
            FindObjectOfType<AudioManager>().Play("ErrorBtn");
            return;
        }

        if(unitType == 2 && GameObject.Find("MineroUranio(Clone)") != null)
        {
            return;
        }

        if(unitType == 2 && GameManager.currentLevel == 0)
        {
            return;
        }

        FindObjectOfType<AudioManager>().Play("BtnClick");
        //Se entiende que si le da al boton otra vez es para cancelar la accion
        if (lastUnitType == unitType)
        {
            Destroy(nuevaUnidad);
            troopBtns[lastUnitType].GetComponent<Image>().sprite = troopBtnsSprites[DEFAULT];
            deployingUnit = false;
            lastUnitType = -1;
            return;
        }

        if (deployingUnit)
        {
            Destroy(nuevaUnidad);
            troopBtns[lastUnitType].GetComponent<Image>().sprite = troopBtnsSprites[DEFAULT];
        }

        switch (unitType)
        {
            case 0: //Minero
                deployingUnit = true;
                unitEnergyReq = minerEnergyReq;
                unitName = "Minero";
                break;
            case 1: //Defensor
                deployingUnit = true;
                unitEnergyReq = defenderEnergyReq;
                unitName = "Defender";                              
                break;
            case 2: //Minero de Uranio
                deployingUnit = true;
                unitEnergyReq = minerUranioEnergyReq;
                unitName = "MineroUranio";
                break;
        }

        mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        nuevaUnidad = Instantiate(Resources.Load("Prefabs/" + unitName), new Vector3(mouseWorldPos.x, mouseWorldPos.y, 0), new Quaternion(0, 0, 0, 0)) as GameObject;
        if(unitType != 2)
        {
            nuevaUnidad.GetComponent<NavMeshAgent>().enabled = false;
        }

        if(lastUnitType != -1)
        {
            troopBtns[lastUnitType].GetComponent<Image>().sprite = troopBtnsSprites[DEFAULT];
        }
        
        lastUnitType = unitType;
        troopBtns[unitType].GetComponent<Image>().sprite = troopBtnsSprites[PRESSED];
    }

    //Si se está desplegando una unidad, actualiza la posición y completa o cancela el despliegue en función del input del jugador
    private void updateUnitPosition()
    {
        if (movementKeysListener() || cameraMovedRecently)
        {
            mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            nuevaUnidad.transform.position = new Vector3(mouseWorldPos.x, mouseWorldPos.y, 0);
        }

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            int layerMask;
            if(lastUnitType == 2)
            {
                layerMask = 1 << LayerMask.NameToLayer("VetaUranio");
            }
            else
            {
                layerMask = 1 << LayerMask.NameToLayer("Suelo");
            }
            Collider2D hit = Physics2D.OverlapPoint(mouseWorldPos, layerMask);
            if (hit != null)
            {
                if(lastUnitType == 2)
                {
                    if (hit.GetComponent<BoxCollider2D>().CompareTag("VetaUranio"))
                    {
                        if (GetComponent<EnergyController>().checkIfEnoughEnergy(unitEnergyReq))
                        {
                            //Se completa el despliegue de la unidad
                            FindObjectOfType<AudioManager>().Play(unitName);
                            GetComponent<EnergyController>().useEnergy(unitEnergyReq);
                            nuevaUnidad.GetComponent<CombatController>().isBeingDeployed = false;
                            deployingUnit = false;
                            troopBtns[lastUnitType].GetComponent<Image>().sprite = troopBtnsSprites[DEFAULT];
                            nuevaUnidad.transform.position = new Vector2(hit.transform.position.x, hit.transform.position.y -1);
                            nuevaUnidad.GetComponent<NavMeshObstacle>().enabled = true;
                            nuevaUnidad = null;
                            lastUnitType = -1;
                            GameObject.Find("MenteColmena").GetComponent<MenteColmenaController>().updateEnemiesTarget();
                        }
                    }
                    else
                    {
                        FindObjectOfType<AudioManager>().Play("ErrorBtn");
                    }
                }
                else if (hit.GetComponent<CompositeCollider2D>().CompareTag("Suelo"))
                {
                    if (GetComponent<EnergyController>().checkIfEnoughEnergy(unitEnergyReq))
                    {
                        //Se completa el despliegue de la unidad
                        FindObjectOfType<AudioManager>().Play(unitName);
                        GetComponent<EnergyController>().useEnergy(unitEnergyReq);
                        nuevaUnidad.GetComponent<CombatController>().isBeingDeployed = false;
                        nuevaUnidad.GetComponent<NavMeshAgent>().enabled = true;
                        deployingUnit = false;
                        troopBtns[lastUnitType].GetComponent<Image>().sprite = troopBtnsSprites[DEFAULT];
                        nuevaUnidad = null;
                        lastUnitType = -1;
                        GameObject.Find("MenteColmena").GetComponent<MenteColmenaController>().updateEnemiesTarget();
                    }
                }
                else
                {
                    FindObjectOfType<AudioManager>().Play("ErrorBtn");
                }
            }
            else
            {
                FindObjectOfType<AudioManager>().Play("ErrorBtn");
            }
        }

        //Cancela el despliegue de unidades en curso
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            cancelDeploy();
        }
    }

    public void cancelDeploy()
    {
        Destroy(nuevaUnidad);
        deployingUnit = false;
        if(lastUnitType != -1)
        {
            troopBtns[lastUnitType].GetComponent<Image>().sprite = troopBtnsSprites[DEFAULT];
        }
        lastUnitType = -1;
    }

    //Accesos rápidos para seleccionar tropa con las teclas 1-4
    private void deployKeysListener()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            selectUnit(0);
        }else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            selectUnit(1);
        }else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            selectUnit(2);
        }else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            selectUnit(3);
        }
    }

    private bool movementKeysListener()
    {
        if(Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D)) {
            cameraMovedRecently = true;
            cameraMovedRecentlyTimer = 0;
            return true;
        }else if (Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private bool checkIfEnoughEnergy(int unitType)
    {
        switch (unitType)
        {
            case 0:
                if (this.GetComponent<EnergyController>().checkIfEnoughEnergy(minerEnergyReq))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            case 1:
                if (this.GetComponent<EnergyController>().checkIfEnoughEnergy(defenderEnergyReq))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            case 2:
                if (this.GetComponent<EnergyController>().checkIfEnoughEnergy(minerUranioEnergyReq))
                {
                    return true;
                }
                else
                {
                    return false;
                }
        }
        return false;
    }
}

