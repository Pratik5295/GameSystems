using UnityEngine;

public class ModelClick : MonoBehaviour
{
    [SerializeField] private int infoIndex;     //Info index to open in relation to Info Manager list
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            DetectModelClick();
        }
    }
    private void DetectModelClick()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            if(hit.collider.tag == "Model")
            {
                OnToggle(infoIndex);
            }
        }
    }

    public void OnClickerClicked(int index)
    {
        OnToggle(index);
    }

    private void OnToggle(int index = 0)
    {
        InfoUIManager.instance.HideAnnotations();
        InfoUIManager.instance.OnModelClicked(index);
    }
}
