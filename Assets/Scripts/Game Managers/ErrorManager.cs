using TMPro;
using UnityEngine;

public class ErrorManager : MonoBehaviour
{
    public static ErrorManager instance = null;

    [SerializeField] private GameObject errorDialogBox;
    [SerializeField] private TextMeshProUGUI errorText;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(instance);
        }
    }

    public void CloseErrorBox()
    {
        errorDialogBox.SetActive(false);
    }

    private void ShowErrorBox()
    {
        errorDialogBox.SetActive(true);
    }

    public void PopErrorMessage(string text)
    {
        errorText.text = text;
        ShowErrorBox();
    }


}
