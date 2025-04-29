using UnityEngine;
using UnityEngine.UI;

public class SceneSelector : MonoBehaviour
{
    public Button selectButton;        // Le bouton du Canvas
    public string sceneNameToSelect;   // Le nom de la sc�ne � stocker
    public static string selectedScene; // Variable statique pour stocker la sc�ne choisie

    private void Start()
    {
        // Quand on clique sur le bouton, on appelle la fonction SelectScene
        selectButton.onClick.AddListener(SelectScene);
    }

    private void SelectScene()
    {
        // Stocker la sc�ne choisie dans la variable statique
        selectedScene = sceneNameToSelect;
        Debug.Log("Sc�ne s�lectionn�e : " + selectedScene);
    }
}
