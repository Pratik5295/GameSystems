using System.Collections.Generic;
using UnityEngine;

public class InfoUIManager : MonoBehaviour
{
    public static InfoUIManager instance;
    [SerializeField] private bool toggle = false;

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

    public void OnModelClicked()
    {
        toggle = !toggle;
        OnToggleChanged();
    }

    private void OnToggleChanged()
    {
        if (toggle)
        {
            ShowAnnotations();
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

    public void HideAnnotations()
    {
        foreach (var uiAnnote in uiAnnotations)
        {
            uiAnnote.gameObject.SetActive(false);
        }
        toggle = false;
    }
}
