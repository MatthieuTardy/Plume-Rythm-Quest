using UnityEngine;

public class SwitchSharedMaterialColor : MonoBehaviour
{
    [SerializeField] private Material sharedMaterial;
    [SerializeField] private string colorPropertyName = "_Color";

    [SerializeField] private Color color1 = Color.red;
    [SerializeField] private Color color2 = Color.blue;

    private bool useFirstColor = true;

    void Start()
    {
        ApplyColor(color1); // couleur initiale
    }

    // Méthode publique à appeler depuis un autre script
    public void SwitchColor()
    {
        useFirstColor = !useFirstColor;
        ApplyColor(useFirstColor ? color1 : color2);
        RefreshRenderers();
    }

    // Méthode pour forcer un "refresh" des renderers
    private void RefreshRenderers()
    {
        foreach (Renderer r in FindObjectsOfType<Renderer>())
        {
            if (r.sharedMaterial == sharedMaterial)
            {
                r.enabled = false;
                r.enabled = true;
            }
        }
    }

    // Méthode interne de changement de couleur
    private void ApplyColor(Color color)
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

    // Optionnel : tu peux toujours tester avec une touche si tu veux
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            SwitchColor();
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            sharedMaterial.SetColor(colorPropertyName, Color.magenta);
        }
    }
}