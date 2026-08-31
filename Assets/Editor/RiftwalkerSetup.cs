using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

/// <summary>
/// 一键搭建 M2 场景：生成数据资产（到 Assets/Resources/Data）+ 构建场景。
/// 用法：打开场景后，菜单栏 → Riftwalker → 一键搭建 M2 场景。
/// 可重复执行（会清理旧敌人、复用已有对象）。
/// </summary>
public static class RiftwalkerSetup
{
    [MenuItem("Riftwalker/一键搭建 M2 场景")]
    public static void BuildAll()
    {
        GenerateDataAssets();
        BuildScene();
    }

    // ================= 1. 生成数据资产（放到 Resources，运行时用 Resources.LoadAll 加载） =================

    private static void GenerateDataAssets()
    {
        EnsureFolder("Assets/Resources/Data");

        CreateEnemyAsset("Enemy_Chaser", "追猎者", EnemyBehavior.Chaser, 30, 2.5f, 10, new Color(1f, 0.35f, 0.35f));
        CreateEnemyAsset("Enemy_Shooter", "射手", EnemyBehavior.Shooter, 20, 2f, 0, new Color(1f, 0.6f, 0.2f));
        CreateEnemyAsset("Enemy_Exploder", "自爆", EnemyBehavior.Exploder, 15, 4f, 0, new Color(1f, 0.2f, 0.55f));
        CreateEnemyAsset("Enemy_Boss", "Boss", EnemyBehavior.Chaser, 300, 3f, 20, new Color(0.7f, 0.1f, 0.25f), true, 3f);

        CreateUpgradeAsset("Upgrade_MoveSpeed", "移速提升", "移动速度 +1", UpgradeType.MoveSpeed, 1f);
        CreateUpgradeAsset("Upgrade_FireRate", "攻速提升", "射击频率 +1", UpgradeType.FireRate, 1f);
        CreateUpgradeAsset("Upgrade_Damage", "伤害提升", "子弹伤害 +5", UpgradeType.Damage, 5f);
        CreateUpgradeAsset("Upgrade_MaxHealth", "生命上限", "生命上限 +25 并回血", UpgradeType.MaxHealth, 25f);
        CreateUpgradeAsset("Upgrade_MultiShot", "弹幕分裂", "每次多射 1 发", UpgradeType.MultiShot, 1f);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("数据资产生成完成：Assets/Resources/Data");
    }

    private static void CreateEnemyAsset(string fileName, string displayName, EnemyBehavior behavior, int hp, float speed, int dmg, Color color, bool isBoss = false, float scale = 1f)
    {
        string path = "Assets/Resources/Data/" + fileName + ".asset";
        EnemyData data = AssetDatabase.LoadAssetAtPath<EnemyData>(path);
        if (data == null)
        {
            data = ScriptableObject.CreateInstance<EnemyData>();
            AssetDatabase.CreateAsset(data, path);
        }
        data.displayName = displayName;
        data.behavior = behavior;
        data.maxHealth = hp;
        data.moveSpeed = speed;
        data.contactDamage = dmg;
        data.color = color;
        data.isBoss = isBoss;
        data.scale = scale;
        EditorUtility.SetDirty(data);
    }

    private static void CreateUpgradeAsset(string fileName, string displayName, string desc, UpgradeType type, float value)
    {
        string path = "Assets/Resources/Data/" + fileName + ".asset";
        UpgradeData data = AssetDatabase.LoadAssetAtPath<UpgradeData>(path);
        if (data == null)
        {
            data = ScriptableObject.CreateInstance<UpgradeData>();
            AssetDatabase.CreateAsset(data, path);
        }
        data.upgradeName = displayName;
        data.description = desc;
        data.type = type;
        data.value = value;
        EditorUtility.SetDirty(data);
    }

    private static void EnsureFolder(string path)
    {
        if (AssetDatabase.IsValidFolder(path)) return;
        string[] parts = path.Split('/');
        string cur = parts[0];
        for (int i = 1; i < parts.Length; i++)
        {
            string next = cur + "/" + parts[i];
            if (!AssetDatabase.IsValidFolder(next))
                AssetDatabase.CreateFolder(cur, parts[i]);
            cur = next;
        }
    }

    // ================= 2. 构建场景 =================

    private static void BuildScene()
    {
        // 清掉旧敌人（重复搭建时避免残留）
        foreach (Enemy e in UnityEngine.Object.FindObjectsOfType<Enemy>())
            UnityEngine.Object.DestroyImmediate(e.gameObject);

        Camera cam = EnsureCamera();

        GameObject ground = FindOrCreate("Ground");
        SetupGround(ground);

        GameObject player = FindOrCreate("Player");
        SetupPlayer(player);

        EnsureEventSystem();

        GameObject mgr = FindOrCreate("GameManager");
        EnsureComponent<RoomManager>(mgr);
        EnsureComponent<UpgradeUI>(mgr);
        EnsureComponent<HUD>(mgr);
        EnsureComponent<EndScreenUI>(mgr);
        EnsureComponent<GameManager>(mgr);

        CameraFollow follow = EnsureComponent<CameraFollow>(cam.gameObject);
        follow.SetTarget(player.transform);

        // 标记场景已修改，保存场景时才会把新建对象写进去
        EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());

        Debug.Log("M2 场景搭建完成，点击 Play 试玩。");
    }

    private static Camera EnsureCamera()
    {
        Camera cam = Camera.main;
        if (cam == null)
        {
            GameObject camGo = new GameObject("Main Camera");
            camGo.tag = "MainCamera";
            cam = camGo.AddComponent<Camera>();
        }
        cam.orthographic = true;
        cam.orthographicSize = 7f;
        cam.transform.position = new Vector3(0, 0, -10);
        return cam;
    }

    private static void SetupGround(GameObject ground)
    {
        SpriteRenderer sr = ground.GetComponent<SpriteRenderer>();
        if (sr == null) sr = ground.AddComponent<SpriteRenderer>();
        sr.sprite = PlaceholderSprite.Create(new Color(0.16f, 0.16f, 0.2f));
        sr.sortingOrder = -10;
        ground.transform.localScale = new Vector3(30f, 30f, 1f);
    }

    private static void SetupPlayer(GameObject go)
    {
        go.tag = "Player";

        SpriteRenderer sr = go.GetComponent<SpriteRenderer>();
        if (sr == null) sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = PlaceholderSprite.Create(new Color(0.3f, 0.7f, 1f));
        sr.sortingOrder = 0;

        Rigidbody2D rb = go.GetComponent<Rigidbody2D>();
        if (rb == null) rb = go.AddComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        CircleCollider2D col = go.GetComponent<CircleCollider2D>();
        if (col == null) col = go.AddComponent<CircleCollider2D>();
        col.radius = 0.4f;

        EnsureComponent<PlayerController>(go);
        EnsureComponent<PlayerHealth>(go);
        EnsureComponent<PlayerStats>(go);
        EnsureComponent<PlayerShoot>(go);
    }

    private static void EnsureEventSystem()
    {
        if (UnityEngine.Object.FindObjectOfType<EventSystem>() != null) return;
        GameObject es = new GameObject("EventSystem");
        es.AddComponent<EventSystem>();
        es.AddComponent<StandaloneInputModule>();
    }

    private static GameObject FindOrCreate(string name)
    {
        GameObject go = GameObject.Find(name);
        if (go == null) go = new GameObject(name);
        return go;
    }

    private static T EnsureComponent<T>(GameObject go) where T : Component
    {
        T c = go.GetComponent<T>();
        if (c == null) c = go.AddComponent<T>();
        return c;
    }
}
