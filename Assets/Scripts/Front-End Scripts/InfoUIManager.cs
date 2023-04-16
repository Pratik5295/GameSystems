using System.Collections.Generic;
using UnityEngine;

public class InfoUIManager : MonoBehaviour
{
    public static InfoUIManager instance;
    [SerializeField] private bool toggle = false;
    [SerializeField] private bool showAll = false;

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

    public void OnModelClicked(int index)
    {
        toggle = !toggle;
        OnToggleChanged(index);
    }

    private void OnToggleChanged(int index = 0)
    {
        if (toggle)
        {
            if (showAll)
                ShowAnnotations();
            else
                ShowSpecificAnnotation(index);
        }
        else
        {
            HideAnnotations();
        }
    }

    public void ShowAnnotations()
    {
        foreach(var uiAnnote in uiAnnotations)
        {
            uiAnnote.gameObject.SetActive(true);
        }
        toggle = true;
    }

    public void ShowSpecificAnnotation(int index)
    {
        if (index < uiAnnotations.Count)
        {
            uiAnnotations[index].gameObject.SetActive(true);
            toggle = true;
        }
    }

    public void HideAnnotations()
    {
        foreach (var uiAnnote in uiAnnotations)
        {
            uiAnnote.gameObject.SetActive(false);
        }
        toggle = false;
    }
}
