using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionPositions : MonoBehaviour
{

    public GameObject[] interactingPositions;
    public bool[] interactingPositionsState;

    //Si tiene hijos que son posiciones para interactuar, rellena los arrays para que sean accesibles por otras entidades
    void Start()
    {
        interactingPositions = new GameObject[this.transform.childCount];
        interactingPositionsState = new bool[this.transform.childCount];
        int i = 0;
        foreach (Transform child in transform)
        {
            if (child.gameObject.tag == "InteractionPosition")
            {
                interactingPositions[i] = child.gameObject;
                interactingPositionsState[i] = false;
                i++;
            }
        }
    }

    //La entidad que llama a esta funcion recibe una posición para colocarse y minar
    public GameObject getFreeInteractingPosition()
    {
        for (int i = 0; i < interactingPositions.Length; i++)
        {
            if (interactingPositionsState[i] == false)
            {
                interactingPositionsState[i] = true;
                return interactingPositions[i];
            }
        }

        return null;
    }

    //Cuando una entidad deja libre una posición de minado, llama a esta función para liberar la posición
    public void releaseInteractingPosition(GameObject position)
    {
        for (int i = 0; i < interactingPositions.Length; i++)
        {
            if (object.ReferenceEquals(position, interactingPositions[i]))
            {
                interactingPositionsState[i] = false;
                return;
            }
        }
    }

    //Se utiliza para checkear si hay posicion de minado disponible pero sin llegar a ocuparla
    public bool checkFreeInteractingPosition()
    {
        for (int i = 0; i < interactingPositions.Length; i++)
        {
            if (interactingPositionsState[i] == false)
            {
                return true;
            }
        }
        return false;
    }
}
