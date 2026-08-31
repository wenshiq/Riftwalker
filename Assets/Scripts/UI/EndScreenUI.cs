using System;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 结束界面（死亡/通关）：代码动态创建面板 + 标题 + "重新开始"按钮。
/// </summary>
public class EndScreenUI : MonoBehaviour
{
    private GameObject panel;
    private Text titleText;
    private Action onRestart;

    private void Awake()
    {
        BuildPanel();
        panel.SetActive(false);
    }

    public void Show(string title, Action onRestart)
    {
        titleText.text = title;
        this.onRestart = onRestart;
        panel.SetActive(true);
    }

    private void BuildPanel()
    {
        GameObject canvasGo = new GameObject("EndScreenCanvas");
        Canvas canvas = canvasGo.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 10; // 盖在一切之上
        canvasGo.AddComponent<CanvasScaler>();
        canvasGo.AddComponent<GraphicRaycaster>();

        panel = new GameObject("EndPanel");
        panel.transform.SetParent(canvasGo.transform, false);
        Image bg = panel.AddComponent<Image>();
        bg.color = new Color(0f, 0f, 0f, 0.75f);
        RectTransform prt = panel.GetComponent<RectTransform>();
        prt.anchorMin = Vector2.zero;
        prt.anchorMax = Vector2.one;
        prt.offsetMin = Vector2.zero;
        prt.offsetMax = Vector2.zero;

        // 标题
        GameObject titleGo = new GameObject("Title");
        titleGo.transform.SetParent(panel.transform, false);
        titleText = titleGo.AddComponent<Text>();
        titleText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        titleText.fontSize = 64;
        titleText.color = Color.white;
        titleText.alignment = TextAnchor.MiddleCenter;
        RectTransform trt = titleGo.GetComponent<RectTransform>();
        trt.anchorMin = new Vector2(0.5f, 0.5f);
        trt.anchorMax = new Vector2(0.5f, 0.5f);
        trt.anchoredPosition = new Vector2(0f, 80f);
        trt.sizeDelta = new Vector2(500f, 100f);

        // 重新开始按钮
        GameObject btnGo = new GameObject("RestartButton");
        btnGo.transform.SetParent(panel.transform, false);
        Image img = btnGo.AddComponent<Image>();
        img.color = new Color(0.25f, 0.55f, 0.9f, 1f);
        RectTransform brt = btnGo.GetComponent<RectTransform>();
        brt.anchorMin = new Vector2(0.5f, 0.5f);
        brt.anchorMax = new Vector2(0.5f, 0.5f);
        brt.anchoredPosition = new Vector2(0f, -60f);
        brt.sizeDelta = new Vector2(240f, 70f);

        Button btn = btnGo.AddComponent<Button>();
        btn.onClick.AddListener(OnRestartClicked);

        GameObject btnTxtGo = new GameObject("Text");
        btnTxtGo.transform.SetParent(btnGo.transform, false);
        Text btnTxt = btnTxtGo.AddComponent<Text>();
        btnTxt.text = "重新开始";
        btnTxt.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        btnTxt.fontSize = 32;
        btnTxt.color = Color.white;
        btnTxt.alignment = TextAnchor.MiddleCenter;
        RectTransform btrt = btnTxtGo.GetComponent<RectTransform>();
        btrt.anchorMin = Vector2.zero;
        btrt.anchorMax = Vector2.one;
        btrt.offsetMin = Vector2.zero;
        btrt.offsetMax = Vector2.zero;
    }

    private void OnRestartClicked()
    {
        panel.SetActive(false);
        onRestart?.Invoke();
    }
}
