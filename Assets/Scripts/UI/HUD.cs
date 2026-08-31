using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// HUD：左上角血条 + 右上角房间数。代码动态创建（M3 先保证功能，M4 换正式美术）。
/// </summary>
public class HUD : MonoBehaviour
{
    private Image healthFill;
    private Text roomText;
    private PlayerHealth playerHealth;
    private RoomManager roomManager;

    private void Awake()
    {
        BuildUI();
    }

    private void Start()
    {
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) playerHealth = p.GetComponent<PlayerHealth>();
        roomManager = RoomManager.Instance;
    }

    private void Update()
    {
        if (playerHealth != null && healthFill != null)
            healthFill.fillAmount = (float)playerHealth.CurrentHealth / playerHealth.MaxHealth;

        if (roomManager != null && roomText != null)
            roomText.text = "房间 " + roomManager.CurrentRoom;
    }

    private void BuildUI()
    {
        GameObject canvasGo = new GameObject("HUDCanvas");
        Canvas canvas = canvasGo.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 1; // 在强化/结束面板之下
        canvasGo.AddComponent<CanvasScaler>();

        // 血条背景
        GameObject bgGo = new GameObject("HealthBarBG");
        bgGo.transform.SetParent(canvasGo.transform, false);
        Image bg = bgGo.AddComponent<Image>();
        bg.color = new Color(0f, 0f, 0f, 0.6f);
        RectTransform bgRt = bgGo.GetComponent<RectTransform>();
        bgRt.anchorMin = new Vector2(0, 1);
        bgRt.anchorMax = new Vector2(0, 1);
        bgRt.pivot = new Vector2(0, 1);
        bgRt.anchoredPosition = new Vector2(20, -20);
        bgRt.sizeDelta = new Vector2(220, 24);

        // 血条填充
        GameObject fillGo = new GameObject("HealthBarFill");
        fillGo.transform.SetParent(bgGo.transform, false);
        healthFill = fillGo.AddComponent<Image>();
        healthFill.color = new Color(0.2f, 0.9f, 0.3f);
        RectTransform fillRt = fillGo.GetComponent<RectTransform>();
        fillRt.anchorMin = Vector2.zero;
        fillRt.anchorMax = Vector2.one;
        fillRt.offsetMin = new Vector2(2, 2);
        fillRt.offsetMax = new Vector2(-2, -2);
        healthFill.type = Image.Type.Filled;
        healthFill.fillMethod = Image.FillMethod.Horizontal;

        // 房间数
        GameObject txtGo = new GameObject("RoomText");
        txtGo.transform.SetParent(canvasGo.transform, false);
        roomText = txtGo.AddComponent<Text>();
        roomText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        roomText.fontSize = 28;
        roomText.color = Color.white;
        roomText.alignment = TextAnchor.MiddleRight;
        RectTransform txtRt = txtGo.GetComponent<RectTransform>();
        txtRt.anchorMin = new Vector2(1, 1);
        txtRt.anchorMax = new Vector2(1, 1);
        txtRt.pivot = new Vector2(1, 1);
        txtRt.anchoredPosition = new Vector2(-20, -20);
        txtRt.sizeDelta = new Vector2(160, 40);
    }
}
