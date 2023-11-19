using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spaceship_C : MonoBehaviour
{
    public int startEnergyAmount;
    public int maxEnergyAmount;
    public int mineralAmount;
    public int energyGainSpeed;
    public int minerEnergyReq;
    public int defenderEnergyReq;
    public int numSelectedTroops;

    //Inicializa los recursos que tiene la nave
    void Start()
    {
        mineralAmount = 0;
    }

    //Recibe una unidad de un recurso
    public void deployMineral()
    {
        mineralAmount++;
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
}
