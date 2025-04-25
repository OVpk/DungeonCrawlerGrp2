using UnityEngine;

public class EntityLocationDisplayer : MonoBehaviour
{
    [Header("Visuals")]
    [SerializeField] private SpriteRenderer baseRenderer;
    [SerializeField] private Sprite allySprite;
    [SerializeField] private Sprite enemySprite;

    private Material _instanceMaterial;

    private static readonly int UseGrayscale = Shader.PropertyToID("_UseGrayscale");
    private static readonly int OutlineColor = Shader.PropertyToID("_OutlineColor");
    private static readonly int OutlineSize = Shader.PropertyToID("_OutlineSize");

    private void Awake()
    {
        // Clone material instance to avoid global changes
        _instanceMaterial = new Material(baseRenderer.sharedMaterial);
        baseRenderer.material = _instanceMaterial;
        ClearHighlight();
        _instanceMaterial.SetFloat(OutlineSize, 20f);
    }

    public void SetTeam(FightManager.TurnState team)
    {
        baseRenderer.sprite = team == FightManager.TurnState.Player ? allySprite : enemySprite;
    }

    public void SetGrayscale(bool active)
    {
        _instanceMaterial.SetFloat(UseGrayscale, active ? 1f : 0f);
    }

    public void SetHighlight(Color color)
    {
        _instanceMaterial.SetColor(OutlineColor, color);
    }

    public void ClearHighlight()
    {
        _instanceMaterial.SetColor(OutlineColor, Color.clear);
    }
}