using UnityEngine;
using UnityEngine.UIElements;

public class GameUIHandler : MonoBehaviour
{
    public BaseCharacter player;
    public UIDocument UIDoc;
    private Label m_HealthLabel;
    private VisualElement m_HealthBarMask;
    private VisualElement m_ShieldBarMask;
    private Label m_ShieldLabel;
    public float defaultCrosshairSpread = 10f; // Default crosshair spread value
    private VisualElement crosshairTop, crosshairBottom, crosshairLeft, crosshairRight;
    private float crosshairDistance = 10f; // Distance from center to each arm


    private void OnEnable()
    {
        var root = UIDoc.rootVisualElement;
        var crosshairContainer = new VisualElement();
        crosshairContainer.name = "CrosshairContainer";
        crosshairContainer.style.position = Position.Absolute;
        crosshairContainer.style.top = new Length(50, LengthUnit.Percent);
        crosshairContainer.style.left = new Length(50, LengthUnit.Percent);
        crosshairContainer.style.transformOrigin = new TransformOrigin(Length.Percent(50), Length.Percent(50));
        crosshairContainer.style.translate = new StyleTranslate(new Translate(new Length(-50, LengthUnit.Percent), new Length(-50, LengthUnit.Percent)));

        // Create lines
        crosshairTop = CreateLine("Top", new Vector2(2, 10), "Top");
        crosshairBottom = CreateLine("Bottom", new Vector2(2, 10), "Bottom");
        crosshairLeft = CreateLine("Left", new Vector2(10, 2), "Left");
        crosshairRight = CreateLine("Right", new Vector2(10, 2), "Right");

        // Add to container
        crosshairContainer.Add(crosshairTop);
        crosshairContainer.Add(crosshairBottom);
        crosshairContainer.Add(crosshairLeft);
        crosshairContainer.Add(crosshairRight);

        // Add to root
        root.Add(crosshairContainer);
        var dot = new VisualElement();
        dot.name = "CrosshairDot";
        dot.style.width = 3;
        dot.style.height = 3;
        dot.style.backgroundColor = Color.white;
        dot.style.opacity = 1f;
        dot.style.position = Position.Absolute;
        dot.style.left = -1.5f;
        dot.style.top = -1.5f;
        crosshairContainer.Add(dot);
        SetCrosshairSpread(defaultCrosshairSpread);

    }

    private void Start()
    {
        player = FindFirstObjectByType<BaseCharacter>();
        player.OnHealthChange += HealthChanged;
        m_HealthLabel = UIDoc.rootVisualElement.Q<Label>("HealthLabel");
        m_HealthBarMask = UIDoc.rootVisualElement.Q<VisualElement>("HealthBarMask");
        m_ShieldBarMask = UIDoc.rootVisualElement.Q<VisualElement>("ShieldBarMask");
        m_ShieldLabel = UIDoc.rootVisualElement.Q<Label>("ShieldLabel");
        player.OnShieldChange += ShieldChanged;

        HealthChanged();
        ShieldChanged();
    }


    void HealthChanged()
    {
        float healthRatio = (float)player.health.BaseValue / player.maxHealth.BaseValue;
        float healthPercent = Mathf.Lerp(19, 88, healthRatio); // 8 is the min, 88 is the max. This is a percentage of the bar width
        m_HealthBarMask.style.width = Length.Percent(healthPercent);
        m_HealthLabel.text = $"{player.health.BaseValue:F0}/{player.maxHealth.BaseValue:F0}";

    }
    void ShieldChanged()
    {
        float Shield = player.Shield.BaseValue / player.maxHealth.BaseValue;
        float ShieldPercent = Mathf.Lerp(19, 88, Shield);
        m_ShieldBarMask.style.width = Length.Percent(ShieldPercent);
        if (player.Shield.BaseValue <= 0)
            m_ShieldLabel.text = "";
        else if (player.Shield.BaseValue >= 1)
            m_ShieldLabel.text = $"+ {player.Shield.BaseValue:F0}";
    }
    private VisualElement CreateLine(string name, Vector2 size, string direction)
    {
        var line = new VisualElement();
        line.name = $"Crosshair{name}";
        line.style.position = Position.Absolute;
        line.style.width = size.x;
        line.style.height = size.y;
/*        line.style.left = position.x;
        line.style.top = position.y;*/
        line.style.backgroundColor = Color.white;
        line.style.opacity = 0.9f;
        switch (direction)
        {
            case "Top":
                line.style.left = new StyleLength(-size.x * 0.5f); // center horizontally
                line.style.top = new StyleLength(-crosshairDistance - size.y);
                break;
            case "Bottom":
                line.style.left = new StyleLength(-size.x * 0.5f);
                line.style.top = new StyleLength(crosshairDistance);
                break;
            case "Left":
                line.style.top = new StyleLength(-size.y * 0.5f); // center vertically
                line.style.left = new StyleLength(-crosshairDistance - size.x);
                break;
            case "Right":
                line.style.top = new StyleLength(-size.y * 0.5f);
                line.style.left = new StyleLength(crosshairDistance);
                break;
        }

                return line;
    }
    public void SetCrosshairSpread(float spread)
    {
        float topHeight = crosshairTop.resolvedStyle.height;
        float bottomHeight = crosshairBottom.resolvedStyle.height;
        float leftWidth = crosshairLeft.resolvedStyle.width;
        float rightWidth = crosshairRight.resolvedStyle.width;

        // Move arms outward from the center by (spread + half their size)
        crosshairTop.style.top = -spread - topHeight;
        crosshairBottom.style.top = spread;
        crosshairLeft.style.left = -spread - leftWidth;
        crosshairRight.style.left = spread;
    }

}