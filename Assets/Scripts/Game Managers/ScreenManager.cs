using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder (1)]
public class ScreenManager : MonoBehaviour
{
    public static ScreenManager Instance = null;

    [SerializeField]private List<GameObject> screensList = new List<GameObject>();

    [SerializeField] private GameObject currentActiveScreen;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        OverallGameManager.Instance.onGameStateAction += OnGameStateActionHandler;
    }

    private void OnDisable()
    {
        OverallGameManager.Instance.onGameStateAction -= OnGameStateActionHandler;
    }

    private void Start()
    {
        ShowScreenByIndex((int)OverallGameManager.Instance.GetCurrentState());
    }

    private void HideAllScreens()
    {
        foreach(var screen in screensList)
        {
            screen.SetActive(false);
        }
    }

    private void ShowScreenByIndex(int index)
    {
        HideAllScreens();

        currentActiveScreen = screensList[index].gameObject;
        currentActiveScreen.SetActive(true);
    }

    private void OnGameStateActionHandler(GAMESTATE _currentState)
    {
        ShowScreenByIndex((int)_currentState);
    }
}
