using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spaceship_C : MonoBehaviour
{
    public int mineralAmount;

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
}
