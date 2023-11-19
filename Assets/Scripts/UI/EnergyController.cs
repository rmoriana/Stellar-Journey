using UnityEngine;
using TMPro;

public class EnergyController : MonoBehaviour
{

    public TMP_Text energyText;
    private int maxEnergy, currentEnergy;
    private float energyGainSpeed;
    private float energyGainCooldown;

    //Obtiene la energ�a m�xima y actual y actualiza el HUD
    void Start()
    {
        maxEnergy = GameObject.Find("Spaceship_P").GetComponent<Spaceship_C>().getMaxGameEnergy();
        currentEnergy = GameObject.Find("Spaceship_P").GetComponent<Spaceship_C>().getGameEnergy();
        energyGainSpeed = GameObject.Find("Spaceship_P").GetComponent<Spaceship_C>().getEnergyGainSpeed();
        updateEnergyText();
    }

    //Gestiona la producci�n de energ�a
    void Update()
    {
        energyProduction();
    }

    //Comprueba si hay suficiente energ�a para realizar la acci�n escogida por el jugador
    public bool checkIfEnoughEnergy(int quantity)
    {
        if (currentEnergy >= quantity)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    //Hace efectivo el consumo de energ�a realizado por una acci�n
    public void useEnergy(int quantity)
    {
        currentEnergy -= quantity;
        updateEnergyText();
    }

    //Actualiza el contador de energ�a
    private void updateEnergyText()
    {
        energyText.text = currentEnergy.ToString() + "/" + maxEnergy.ToString();
    }

    //Cada vez que transcurre el tiempo establecido, suma un punto de energ�a a la cantidad actual
    private void energyProduction()
    {
        energyGainCooldown += Time.deltaTime;
        if (energyGainCooldown >= energyGainSpeed)
        {
            if (currentEnergy < maxEnergy) currentEnergy++;
            updateEnergyText();
            energyGainCooldown = 0;
        }
    }
}
