using UnityEngine;
using TMPro;

public class DropableDropdown : MonoBehaviour
{
    public GameObject dropdownGO;
    private TMP_Dropdown dropdown;
    GameManager gm;

    private void Start() {
        dropdown = dropdownGO.GetComponent<TMP_Dropdown>();
        gm = (GameManager)FindObjectOfType(typeof(GameManager));
    }

    public void ValueChanged()
    {
        gm.shop.FoodToSpawnDropdownIndex = dropdown.value;
    }
}
