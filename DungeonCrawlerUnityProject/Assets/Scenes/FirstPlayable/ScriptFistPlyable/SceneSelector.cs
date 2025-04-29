using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneSelector : MonoBehaviour
{
    public Button selectButton;         // Bouton du Canvas
    public string sceneNameToSelect;    // Nom de la scène à charger et stocker
    public static string selectedScene; // Variable statique pour garder la scène sélectionnée

    private void Start()
    {
        // Quand on clique, on appelle SelectAndLoadScene
        selectButton.onClick.AddListener(SelectAndLoadScene);
    }

    private void SelectAndLoadScene()
    {
        // Stocker la scène
        selectedScene = sceneNameToSelect;

        // Afficher pour debug
        Debug.Log("Scène sélectionnée : " + selectedScene);

        // Charger la scène
        if (!string.IsNullOrEmpty(selectedScene))
        {
            SceneManager.LoadScene(selectedScene);
        }
        else
        {
            Debug.LogWarning("Nom de scène invalide !");
        }
    }
}
