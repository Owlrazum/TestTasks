using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class InputModeUI : MonoBehaviour
{
    [SerializeField]
    private GameObject K;
    [SerializeField]
    private GameObject KM;

    private bool isKM = false;

    private void Start()
    {
        GetComponent<Button>().onClick.AddListener(ProcessClick);
        isKM = true;
    }

    private void ProcessClick()
    {
        if (isKM)
        {
            KM.SetActive(false);
            K.SetActive(true);
        }
        else 
        {
            KM.SetActive(true);
            K.SetActive(false);
        }
        isKM = !isKM;
    }
}
