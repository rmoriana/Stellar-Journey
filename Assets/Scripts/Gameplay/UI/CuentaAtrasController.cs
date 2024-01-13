using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CuentaAtrasController : MonoBehaviour
{

    private const int DEFAULT = 0;
    private const int PRESSED = 1;

    public TMP_Text infoTxt;
    public TMP_Text tiempoRestanteTxt;
    public GameObject spaceship;
    private float cuentaAtrasActual;
    private int cuentaAtrasMaxima;
    public Button despegueBtn;
    public Sprite[] despegueBtnSprites;
    public Image leftRocket, rightRocket;

    private void Start()
    {
        if (GameManager.shipUpgrades[1]) //Si la mejora de despegue está activada
        {
            cuentaAtrasMaxima = GameManager.spaceshipImprovedLaunchTime;
        }
        else
        {
            cuentaAtrasMaxima = GameManager.spaceshipDefaultLaunchTime;
        }
        cuentaAtrasActual = cuentaAtrasMaxima;
        infoTxt.text = "";
        tiempoRestanteTxt.text = "";
        GetComponent<Image>().enabled = false;
        leftRocket.GetComponent<Image>().enabled = false;
        rightRocket.GetComponent<Image>().enabled = false;
    }
    // Update is called once per frame
    void Update()
    {
        if (spaceship.GetComponent<Spaceship_C>().gameEnding)
        {
            if (cuentaAtrasActual >= 0) cuentaAtrasActual -= Time.deltaTime;
            tiempoRestanteTxt.text = cuentaAtrasActual.ToString("F0");
        }

        if (cuentaAtrasActual <= 0)
        {
            if (!spaceship.GetComponent<Spaceship_C>().getGameHasFinished())
            {
                spaceship.GetComponent<Spaceship_C>().startEndGameSequence();
            }
        }

        keyboardListener();
    }

    //Activa o desactiva la cuenta atrás, reiniciando los valores en el proceso
    public void SpaceshipBtnAction()
    {
        spaceship.GetComponent<Spaceship_C>().gameEnding = !spaceship.GetComponent<Spaceship_C>().gameEnding;
        FindObjectOfType<AudioManager>().Play("BtnBigClick");

        if (spaceship.GetComponent<Spaceship_C>().gameEnding)
        {
            //Boton pulsado
            infoTxt.text = "Despegue en";
            GetComponent<Image>().enabled = true;
            despegueBtn.GetComponent<Image>().sprite = despegueBtnSprites[PRESSED];
            leftRocket.GetComponent<Image>().enabled = true;
            rightRocket.GetComponent<Image>().enabled = true;
            FindObjectOfType<AudioManager>().PlayLoop("SpaceshipCuentaAtras");
        }
        else
        {
            //Boton despulsado, reinicio de la cuenta atrás y desaparecen los campos con el tiempo restante
            infoTxt.text = "";
            tiempoRestanteTxt.text = "";
            GetComponent<Image>().enabled = false;
            cuentaAtrasActual = cuentaAtrasMaxima;
            despegueBtn.GetComponent<Image>().sprite = despegueBtnSprites[DEFAULT];
            leftRocket.GetComponent<Image>().enabled = false;
            rightRocket.GetComponent<Image>().enabled = false;
            FindObjectOfType<AudioManager>().Stop("SpaceshipCuentaAtras");
        }
    }

    //Botón de despegue de la nave
    private void keyboardListener()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            SpaceshipBtnAction();
        }
    }
}
