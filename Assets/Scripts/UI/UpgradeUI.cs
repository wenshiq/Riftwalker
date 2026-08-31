using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 强化三选一 UI：用代码动态创建 Canvas + 三个按钮。
/// M2 先保证功能；M3/M4 再做正式排版与美术。
/// </summary>
public class UpgradeUI : MonoBehaviour
{
    private Canvas canvas;
    private GameObject panel;
    private Action<UpgradeData> onChosen;

    private void Awake()
    {
        BuildCanvas();
        panel.SetActive(false);
    }

    private void BuildCanvas()
    {
        GameObject canvasGo = new GameObject("UpgradeCanvas");
        canvas = canvasGo.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasGo.AddComponent<CanvasScaler>();
        canvasGo.AddComponent<GraphicRaycaster>();

        panel = new GameObject("Panel");
        panel.transform.SetParent(canvasGo.transform, false);
        Image bg = panel.AddComponent<Image>();
        bg.color = new Color(0f, 0f, 0f, 0.7f);
        RectTransform prt = panel.GetComponent<RectTransform>();
        prt.anchorMin = Vector2.zero;
        prt.anchorMax = Vector2.one;
        prt.offsetMin = Vector2.zero;
        prt.offsetMax = Vector2.zero;
    }

    public void Show(List<UpgradeData> choices, Action<UpgradeData> onChosen)
    {
        this.onChosen = onChosen;

        // 清掉上一轮的按钮
        foreach (Transform t in panel.transform) Destroy(t.gameObject);

        float startX = -300f;
        for (int i = 0; i < choices.Count; i++)
            CreateButton(i, startX + i * 300f, choices[i]);

        panel.SetActive(true);
    }

    private void CreateButton(int index, float x, UpgradeData up)
    {
        GameObject btnGo = new GameObject("Upgrade_" + up.upgradeName);
        btnGo.transform.SetParent(panel.transform, false);

        Image img = btnGo.AddComponent<Image>();
        img.color = new Color(0.25f, 0.25f, 0.3f, 0.95f);
        RectTransform rt = btnGo.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(260f, 140f);
        rt.anchoredPosition = new Vector2(x, 0f);

        Button btn = btnGo.AddComponent<Button>();
        btn.onClick.AddListener(() => Choose(up));

        GameObject txtGo = new GameObject("Text");
        txtGo.transform.SetParent(btnGo.transform, false);
        Text txt = txtGo.AddComponent<Text>();
        txt.text = up.upgradeName + "\n" + up.description;
        txt.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        txt.fontSize = 22;
        txt.alignment = TextAnchor.MiddleCenter;
        txt.color = Color.white;
        RectTransform trt = txtGo.GetComponent<RectTransform>();
        trt.anchorMin = Vector2.zero;
        trt.anchorMax = Vector2.one;
        trt.offsetMin = Vector2.zero;
        trt.offsetMax = Vector2.zero;
    }

    private void Choose(UpgradeData up)
    {
        panel.SetActive(false);
        onChosen?.Invoke(up);
    }
}
