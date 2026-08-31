using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// HUD：左上角血条 + 右上角房间数。代码动态创建（M3 先保证功能，M4 换正式美术）。
/// 血条用 anchorMax.x 按血量比例控制宽度（比 Image.Filled 更可靠）。
/// </summary>
public class HUD : MonoBehaviour
{
    private RectTransform fillRect;
    private Text roomText;
    private PlayerHealth playerHealth;
    private RoomManager roomManager;

    private void Awake()
    {
        BuildUI();
    }

    private void Start()
    {
        roomManager = RoomManager.Instance;
        FindPlayer();
    }

    private void FindPlayer()
    {
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null)
        {
            playerHealth = p.GetComponent<PlayerHealth>();
            Debug.Log("[HUD] 找到 Player，PlayerHealth=" + (playerHealth != null));
        }
        else
        {
            Debug.LogWarning("[HUD] 没找到 Tag=Player 的对象");
        }
    }

    private void Update()
    {
        // 兜底：没找到玩家就重试（避免时序问题）
        if (playerHealth == null)
        {
            FindPlayer();
            return;
        }

        if (fillRect != null)
        {
            float ratio = (float)playerHealth.CurrentHealth / playerHealth.MaxHealth;
            fillRect.anchorMax = new Vector2(Mathf.Clamp01(ratio), fillRect.anchorMax.y);
        }

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
        bg.raycastTarget = false;
        RectTransform bgRt = bgGo.GetComponent<RectTransform>();
        bgRt.anchorMin = new Vector2(0, 1);
        bgRt.anchorMax = new Vector2(0, 1);
        bgRt.pivot = new Vector2(0, 1);
        bgRt.anchoredPosition = new Vector2(20, -20);
        bgRt.sizeDelta = new Vector2(220, 24);

        // 血条填充：靠 anchorMax.x 控制宽度
        GameObject fillGo = new GameObject("HealthBarFill");
        fillGo.transform.SetParent(bgGo.transform, false);
        Image fillImg = fillGo.AddComponent<Image>();
        fillImg.color = new Color(0.2f, 0.9f, 0.3f);
        fillImg.raycastTarget = false;
        fillRect = fillGo.GetComponent<RectTransform>();
        fillRect.anchorMin = new Vector2(0, 0);
        fillRect.anchorMax = new Vector2(1, 1);
        fillRect.pivot = new Vector2(0, 0.5f);
        fillRect.offsetMin = new Vector2(2, 2);
        fillRect.offsetMax = new Vector2(-2, -2);

        // 房间数
        GameObject txtGo = new GameObject("RoomText");
        txtGo.transform.SetParent(canvasGo.transform, false);
        roomText = txtGo.AddComponent<Text>();
        roomText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        roomText.fontSize = 28;
        roomText.color = Color.white;
        roomText.alignment = TextAnchor.MiddleRight;
        roomText.raycastTarget = false;
        RectTransform txtRt = txtGo.GetComponent<RectTransform>();
        txtRt.anchorMin = new Vector2(1, 1);
        txtRt.anchorMax = new Vector2(1, 1);
        txtRt.pivot = new Vector2(1, 1);
        txtRt.anchoredPosition = new Vector2(-20, -20);
        txtRt.sizeDelta = new Vector2(160, 40);
    }
}
