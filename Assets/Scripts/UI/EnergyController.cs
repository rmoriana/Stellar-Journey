using UnityEngine;
using TMPro;

public class EnergyController : MonoBehaviour
{

    public TMP_Text energyText;
    private int maxEnergy, currentEnergy;
    private float energyGainSpeed;
    private float energyGainCooldown;

    //Obtiene la energía máxima y actual y actualiza el HUD
    void Start()
    {
        maxEnergy = GameObject.Find("Spaceship_P").GetComponent<Spaceship_C>().getMaxGameEnergy();
        currentEnergy = GameObject.Find("Spaceship_P").GetComponent<Spaceship_C>().getGameEnergy();
        energyGainSpeed = GameObject.Find("Spaceship_P").GetComponent<Spaceship_C>().getEnergyGainSpeed();
        updateEnergyText();
    }

    //Gestiona la producción de energía
    void Update()
    {
        energyProduction();
    }

    //Comprueba si hay suficiente energía para realizar la acción escogida por el jugador
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

    //Hace efectivo el consumo de energía realizado por una acción
    public void useEnergy(int quantity)
    {
        currentEnergy -= quantity;
        updateEnergyText();
    }

    //Actualiza el contador de energía
    private void updateEnergyText()
    {
        energyText.text = currentEnergy.ToString() + "/" + maxEnergy.ToString();
    }

    //Cada vez que transcurre el tiempo establecido, suma un punto de energía a la cantidad actual
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
