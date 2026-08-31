using UnityEditor;
using UnityEngine;

/// <summary>
/// 一键构建 M1 灰盒场景：菜单栏 → Riftwalker → 构建 M1 灰盒场景。
/// 从空场景生成相机、地面、玩家、三个敌人，全部用程序化占位美术。
/// 之后点击 Play 即可试玩。
/// </summary>
public static class M1SceneBootstrap
{
    [MenuItem("Riftwalker/构建 M1 灰盒场景")]
    public static void Build()
    {
        // 1. 相机
        Camera cam = Camera.main;
        if (cam == null)
        {
            GameObject camGo = new GameObject("Main Camera");
            camGo.tag = "MainCamera";
            cam = camGo.AddComponent<Camera>();
        }
        cam.orthographic = true;
        cam.orthographicSize = 6f;
        cam.transform.position = new Vector3(0, 0, -10);

        CameraFollow follow = cam.GetComponent<CameraFollow>();
        if (follow == null) follow = cam.gameObject.AddComponent<CameraFollow>();

        // 2. 地面（纯视觉占位，无碰撞）
        CreateGround();

        // 3. 玩家
        GameObject player = CreatePlayer();

        // 4. 三个敌人，随机分布在周围
        for (int i = 0; i < 3; i++)
        {
            Vector2 pos = new Vector2(Random.Range(-5f, 5f), Random.Range(-4f, 4f));
            CreateEnemy(pos);
        }

        follow.SetTarget(player.transform);

        Debug.Log("M1 灰盒场景构建完成，点击 Play 即可试玩。");
    }

    private static void CreateGround()
    {
        GameObject ground = new GameObject("Ground");
        SpriteRenderer sr = ground.AddComponent<SpriteRenderer>();
        sr.sprite = PlaceholderSprite.Create(new Color(0.16f, 0.16f, 0.2f));
        sr.sortingOrder = -10;
        ground.transform.localScale = new Vector3(30f, 30f, 1f);
    }

    private static GameObject CreatePlayer()
    {
        GameObject go = new GameObject("Player");
        go.tag = "Player";

        SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = PlaceholderSprite.Create(new Color(0.3f, 0.7f, 1f));
        sr.sortingOrder = 0;

        Rigidbody2D rb = go.AddComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        CircleCollider2D col = go.AddComponent<CircleCollider2D>();
        col.radius = 0.4f;

        go.AddComponent<PlayerController>();
        go.AddComponent<PlayerHealth>();
        go.AddComponent<PlayerShoot>();
        return go;
    }

    private static void CreateEnemy(Vector2 pos)
    {
        GameObject go = new GameObject("Enemy");
        go.transform.position = pos;

        SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = PlaceholderSprite.Create(new Color(1f, 0.35f, 0.35f));
        sr.sortingOrder = 0;

        Rigidbody2D rb = go.AddComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        CircleCollider2D col = go.AddComponent<CircleCollider2D>();
        col.radius = 0.4f;

        go.AddComponent<Enemy>();
    }
}
