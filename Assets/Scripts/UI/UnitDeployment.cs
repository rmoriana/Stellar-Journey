using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class UnitDeployment : MonoBehaviour
{
    private const int DEFAULT = 0;
    private const int HIGHLIGHT = 1;
    private const int PRESSED = 2;
    private const int DISABLED = 3;

    public Button[] troopBtns;
    public Sprite[] troopBtnsSprites;
    private GameObject nuevaUnidad;
    private bool deployingUnit;
    private Vector3 mouseWorldPos;
    private int minerEnergyReq;
    private int defenderEnergyReq;
    private int unitEnergyReq;
    private string unitName;
    private int numSelectedTroops;
    private int lastUnitType;
    private void Start()
    {
        numSelectedTroops = GameObject.Find("Spaceship_P").GetComponent<Spaceship_C>().getNumSelectedTroops();
        minerEnergyReq = GameObject.Find("Spaceship_P").GetComponent<Spaceship_C>().getMinerEnergyReq();
        defenderEnergyReq = GameObject.Find("Spaceship_P").GetComponent<Spaceship_C>().getDefenderEnergyReq();

        for(int i = 3; i >= numSelectedTroops; i--)
        {
            troopBtns[i].GetComponent<Image>().sprite = troopBtnsSprites[3];
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

        keyboardListener();
    }

    //Identifica la unidad que se quiere desplegar y comprueba si el jugador tiene energ�a suficiente
    public void selectUnit(int unitType)
    {
        if (!checkIfEnoughEnergy(unitType))
        {
            //TODO: Mensaje de que no hay energ�a para esa unidad
            return;
        }

        //Se entiende que si le da al boton otra vez es para cancelar la accion
        if(lastUnitType == unitType)
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
                    unitName = "Minero_P";
                break;
            case 1: //Defensor
                    deployingUnit = true;
                    unitEnergyReq = defenderEnergyReq;
                    unitName = "Luchador_P";                              
                break;
        }

        mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        nuevaUnidad = Instantiate(Resources.Load("Prefabs/" + unitName), new Vector3(mouseWorldPos.x, mouseWorldPos.y, 0), new Quaternion(0, 0, 0, 0)) as GameObject;
        nuevaUnidad.GetComponent<NavMeshAgent>().enabled = false;

        if(lastUnitType != -1)
        {
            troopBtns[lastUnitType].GetComponent<Image>().sprite = troopBtnsSprites[DEFAULT];
        }
        
        lastUnitType = unitType;
        troopBtns[unitType].GetComponent<Image>().sprite = troopBtnsSprites[PRESSED];
    }

    //Si se est� desplegando una unidad, actualiza la posici�n y completa o cancela el despliegue en funci�n del input del jugador
    private void updateUnitPosition()
    {
        if (Input.GetAxis("Mouse X") < 0 || Input.GetAxis("Mouse X") > 0 || Input.GetAxis("Mouse Y") > 0 || Input.GetAxis("Mouse Y") < 0)
        {
            mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            nuevaUnidad.transform.position = new Vector3(mouseWorldPos.x, mouseWorldPos.y, 0);
        }

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mouseWorldPos, Vector2.zero);
            if (hit.collider != null)
            {
                Debug.Log(hit.collider.tag);
                if (hit.collider.tag == "Suelo")
                {
                    if (this.GetComponent<EnergyController>().checkIfEnoughEnergy(unitEnergyReq))
                    {
                        this.GetComponent<EnergyController>().useEnergy(unitEnergyReq);
                        nuevaUnidad.GetComponent<Minero_C>().isBeingDeployed = false;
                        nuevaUnidad.GetComponent<NavMeshAgent>().enabled = true;
                        deployingUnit = false;
                        troopBtns[lastUnitType].GetComponent<Image>().sprite = troopBtnsSprites[DEFAULT];
                        nuevaUnidad = null;
                        lastUnitType = -1;
                    }
                }
            }
        }

        //Cancela el despliegue de unidades en curso
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            Destroy(nuevaUnidad);
            deployingUnit = false;
            troopBtns[lastUnitType].GetComponent<Image>().sprite = troopBtnsSprites[DEFAULT];
            lastUnitType = -1;
        }
    }

    //Accesos r�pidos para seleccionar tropa con las teclas 1-4
    private void keyboardListener()
    {
        if (Input.GetKeyUp(KeyCode.Alpha1))
        {
            selectUnit(0);
        }else if (Input.GetKeyUp(KeyCode.Alpha2))
        {
            selectUnit(1);
        }else if (Input.GetKeyUp(KeyCode.Alpha3))
        {
            selectUnit(2);
        }else if (Input.GetKeyUp(KeyCode.Alpha4))
        {
            selectUnit(3);
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
        }
        return false;
    }
}

