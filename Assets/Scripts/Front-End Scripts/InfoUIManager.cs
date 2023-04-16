using System.Collections.Generic;
using UnityEngine;

public class InfoUIManager : MonoBehaviour
{
    public static InfoUIManager instance;


    [Header("UI to Show and Hide")]
    public List<GameObject> uiAnnotations = new List<GameObject>();

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        HideAnnotations();
    }

    public void ShowAnnotations()
    {
        foreach(var uiAnnote in uiAnnotations)
        {
            uiAnnote.gameObject.SetActive(true);
        }
    }

    public void HideAnnotations()
    {
        foreach (var uiAnnote in uiAnnotations)
        {
            uiAnnote.gameObject.SetActive(false);
        }
    }
}
