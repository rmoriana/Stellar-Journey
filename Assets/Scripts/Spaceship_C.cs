using Cinemachine;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Spaceship_C : MonoBehaviour
{
    [Header("Parámetros globales")]
    public int startEnergyAmount;
    public int maxEnergyAmount;
    public int mineralAmount;
    public float energyGainSpeed;
    public int minerEnergyReq;
    public int defenderEnergyReq;
    public int numSelectedTroops;

    [Header("Inicio y final del nivel")]
    private bool gameStarting;
    public bool gameEnding;
    public int cuentaAtrasMaxima;
    public GameObject spaceshipCamera;
    public GameObject mainCamera;
    public float speed;
    public float acceleration;
    public int spaceshipUpLimit;
    public GameObject resumeCanvas;
    public GameObject hud;
    private bool gameHasFinished;
    private bool waitingForCamera;
    private bool endSequencePhase2;
    private bool spaceshipFlyDelay;
    private Vector3 cameraLastPosition;
    public float startingGameY;
    public float finishLandingY;
    public float landingSpeed;
    public float landingBrake;
    public float minLandingSpeed;
    public GameObject mentecolmena;

    //Inicializa los recursos que tiene la nave
    void Start()
    {
        mineralAmount = 0;
        gameEnding = false;
        resumeCanvas.SetActive(false);
        hud.SetActive(false);
        transform.position = new Vector2(transform.position.x, startingGameY);
        gameStarting = true;
    }

    private void Update()
    {
        if (gameHasFinished && waitingForCamera)
        {
            if (checkIfCameraIsIdle())
            {
                waitingForCamera = false;
                endSequencePhase2 = true;
            }
            trackCameraValues();
        }

        if (endSequencePhase2)
        {
            speed += acceleration * Time.deltaTime;
            transform.Translate(new Vector2(0, speed * Time.deltaTime));
            if (transform.position.y >= spaceshipUpLimit)
            {
                spaceshipCamera.GetComponent<CinemachineVirtualCamera>().Follow = null;
                Invoke("showLvlResume", 1);
                endSequencePhase2 = false;
                spaceshipFlyDelay = true;
            }
        }else if (spaceshipFlyDelay)
        {
            transform.Translate(new Vector2(0, speed * Time.deltaTime));
        }

        if(gameStarting)
        {
            if(landingSpeed > minLandingSpeed)
            {
                landingSpeed -= landingBrake * Time.deltaTime;
            }
            else
            {
                landingSpeed = minLandingSpeed;
            }
            
            if(transform.position.y <= finishLandingY)
            {
                gameStarting = false;
                hud.SetActive(true);
                mentecolmena.GetComponent<MenteColmenaController>().setFirstCycle();
            }
            else
            {
                transform.position = new Vector2(transform.position.x, transform.position.y - (landingSpeed * Time.deltaTime));
            }           
        }
    }

    //Recibe una unidad de un recurso
    public void deployMineral()
    {
        mineralAmount++;
        GameObject.Find("BottomRightPanel").GetComponent<RecursosController>().addResource();
    }

    //Inicia la secuencia de finalización de partida
    public void startEndGameSequence()
    {
        gameHasFinished = true;
        GameObject.Find("BottomLeftPanel").GetComponent<UnitDeployment>().cancelDeploy();
        GameObject.Find("HUD").SetActive(false);
        spaceshipCamera.GetComponent<CinemachineVirtualCamera>().Priority = 2;
        waitingForCamera = true;
    }

    private void showLvlResume()
    {
        spaceshipFlyDelay = false;
        resumeCanvas.SetActive(true);
        GameObject.Find("AstralitaQuantityTxt").GetComponent<TMP_Text>().text = mineralAmount.ToString();
    }

    public void goToBase()
    {
        Invoke("changeScene", 0.2f);
    }

    private void changeScene()
    {
        SceneManager.LoadScene("Base");
    }

    private bool checkIfCameraIsIdle()
    {
        return Equals(spaceshipCamera.transform.position, cameraLastPosition);
    }

    private void trackCameraValues()
    {
        cameraLastPosition = mainCamera.transform.position;
    }

    //Indica la energía que dispone el jugador al inicio del nivel
    public int getGameEnergy()
    {
        return startEnergyAmount;
    }

    //Indica la energía máxima que puede almacenar el jugador
    public int getMaxGameEnergy()
    {
        return maxEnergyAmount;
    }

    public float getEnergyGainSpeed()
    {
        return energyGainSpeed;
    }

    public int getMinerEnergyReq()
    {
        return minerEnergyReq;
    }

    public int getDefenderEnergyReq()
    {
        return defenderEnergyReq;
    }

    public int getNumSelectedTroops()
    {
        return numSelectedTroops;
    }

    public bool getGameHasFinished()
    {
        return gameHasFinished;
    }

    public bool getGameStarting()
    {
        return gameStarting;
    }
}
