using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BaseResourcesPanel : MonoBehaviour
{
    public TMP_Text astralitaText;
    // Start is called before the first frame update
    void Start()
    {
        updateAstralitaText();
    }

    public void updateAstralitaText()
    {
        astralitaText.text = GameManager.astralitaTotal.ToString();
    }
}
