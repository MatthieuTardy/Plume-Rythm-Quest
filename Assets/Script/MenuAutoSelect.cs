using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MenuAutoSelect : MonoBehaviour
{
    public GameObject defaultSelected;

    void OnEnable()
    {
        EventSystem.current.SetSelectedGameObject(null); // Reset
        EventSystem.current.SetSelectedGameObject(defaultSelected);
    }
}
