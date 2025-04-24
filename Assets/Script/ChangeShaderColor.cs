using UnityEngine;

public class SwitchSharedMaterialColor : MonoBehaviour
{
    [SerializeField] private Material sharedMaterial;
    [SerializeField] private string colorPropertyName = "_Color"; // ← Très important !

    [SerializeField] private Color color1 = Color.red;
    [SerializeField] private Color color2 = Color.blue;

    private bool useFirstColor = true;

    void Start()
    {
        ApplyColor(color1); // couleur initiale
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            useFirstColor = !useFirstColor;
            ApplyColor(useFirstColor ? color1 : color2);
            foreach (Renderer r in FindObjectsOfType<Renderer>())
            {
                if (r.sharedMaterial == sharedMaterial)
                {
                    r.enabled = false;
                    r.enabled = true;
                }
            }


        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            sharedMaterial.SetColor("_Color", Color.magenta);
        }

    }

    void ApplyColor(Color color)
    {
        if (sharedMaterial != null && sharedMaterial.HasProperty(colorPropertyName))
        {
            sharedMaterial.SetColor(colorPropertyName, color);
            Debug.Log($"✔️ Changed color to {color} on material {sharedMaterial.name}");
        }
        else
        {
            Debug.LogWarning($"❌ Material is null or does not have property '{colorPropertyName}'");
        }
    }

}
