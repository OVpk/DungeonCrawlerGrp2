using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneSelector : MonoBehaviour
{
    public Button selectButton;         // Bouton du Canvas
    public string sceneNameToSelect;    // Nom de la sc�ne � charger et stocker
    public static string selectedScene; // Variable statique pour garder la sc�ne s�lectionn�e

    private void Start()
    {
        // Quand on clique, on appelle SelectAndLoadScene
        selectButton.onClick.AddListener(SelectAndLoadScene);
    }

    private void SelectAndLoadScene()
    {
        // Stocker la sc�ne
        selectedScene = sceneNameToSelect;

        // Afficher pour debug
        Debug.Log("Sc�ne s�lectionn�e : " + selectedScene);

        // Charger la sc�ne
        if (!string.IsNullOrEmpty(selectedScene))
        {
            SceneManager.LoadScene(selectedScene);
        }
        else
        {
            Debug.LogWarning("Nom de sc�ne invalide !");
        }
    }
}
