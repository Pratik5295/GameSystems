using UnityEngine;

public class ModelClick : MonoBehaviour
{
    [SerializeField] private int infoIndex;     //Info index to open in relation to Info Manager list

    [SerializeField] private bool separateClickers = false;

    [SerializeField] private int kIndex, pIndex, eIndex;
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
            Debug.Log($"Hit: {hit.collider.gameObject.name}");
            if (!separateClickers)
            {
                if (hit.collider.tag == "Model")
                {
                    OnToggle(infoIndex);
                }
            }
            else
            {
                if(hit.collider.tag == "ModelK")
                {
                    OnToggle(kIndex);
                }
                else if(hit.collider.tag == "ModelP")
                {
                    OnToggle(pIndex);
                }
                else if(hit.collider.tag == "ModelE")
                {
                    OnToggle(eIndex);
                }
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
