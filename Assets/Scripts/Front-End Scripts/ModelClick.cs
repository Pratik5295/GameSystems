using UnityEngine;

public class ModelClick : MonoBehaviour
{
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
                OnToggle();
            }
        }
    }

    private void OnToggle()
    {
        InfoUIManager.instance.OnModelClicked();
    }
}
