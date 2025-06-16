using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EnemySpawner : MonoBehaviour
{
    // 敌人预制体数组（在Inspector中拖入三种预制体）
    public GameObject[] enemyPrefabs;
    public Slider healthSlider;
    public Image weaponIcon;
    public Sprite[] weaponSprites;
    // Hero对象的引用
    public Hero hero;

    // 生成间隔时间
    public float spawnInterval = 2f;

    // 与Hero的最小距离
    public float minDistanceFromHero = 3f;

    // 不同类型敌人的Y轴位置
    public float skyEnemyY = 5f;
    public float groundEnemyY = 0f;
    public float undergroundEnemyY = -3f;

    public static EnemySpawner instance;

    [Header("Score System")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI highScoreText;
    private int currentScore = 0;
    private int highScore = 0;

    [Header("Game Over")]
    public GameObject gameOverPanel;
    public Text finalScoreText;

    private float timer;
    private Camera mainCamera;


    void Start()
    {
        mainCamera = Camera.main;
        timer = spawnInterval; // 立即生成第一个敌人
        instance = this;
        LoadHighScore();
        UpdateScoreUI();
    }

    void Update()
    {
        healthSlider.value = hero.health / 10f;
        weaponIcon.sprite = weaponSprites[hero.currentWeaponIndex];
        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            timer = 0f;
            SpawnRandomEnemy();
        }
    }

    void SpawnRandomEnemy()
    {
        // 随机选择敌人类型
        int randomIndex = Random.Range(0, enemyPrefabs.Length);
        GameObject enemyPrefab = enemyPrefabs[randomIndex];

        // 根据敌人类型确定Y轴位置
        float yPos = 0f;
        switch (enemyPrefab.name)
        {
            case "Enemy_Sky":
                yPos = skyEnemyY;
                break;
            case "Enemy_Ground":
                yPos = groundEnemyY;
                break;
            case "Enemy_UnderGround":
                yPos = undergroundEnemyY;
                break;
        }

        // 计算屏幕边界
        float screenWidth = mainCamera.orthographicSize * mainCamera.aspect;
        float screenHeight = mainCamera.orthographicSize;

        // 生成位置（确保在屏幕内且与Hero保持距离）
        Vector3 spawnPos;
        int attempts = 0;
        const int maxAttempts = 10;

        do
        {
            // 随机X位置（屏幕范围内）
            float xPos = Random.Range(-screenWidth, screenWidth);

            // 创建生成位置
            spawnPos = new Vector3(xPos, yPos, 10);

            attempts++;
            if (attempts >= maxAttempts)
            {
                // 防止无限循环
                return;
            }

        } while (Vector2.Distance(spawnPos, hero.transform.position) < minDistanceFromHero);

        // 实例化敌人
        Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
    }

    public void AddScore(int points)
    {
        currentScore += points;
        if (currentScore > highScore)
        {
            highScore = currentScore;
            SaveHighScore();
        }
        UpdateScoreUI();
    }

    // 更新UI显示
    private void UpdateScoreUI()
    {
        scoreText.text = "Score: " + currentScore;
        highScoreText.text = "High Score: " + highScore;
    }

    // 保存最高分
    private void SaveHighScore()
    {
        PlayerPrefs.SetInt("HighScore", highScore);
        PlayerPrefs.Save();
    }

    // 加载最高分
    private void LoadHighScore()
    {
        highScore = PlayerPrefs.GetInt("HighScore", 0);
    }

    // 游戏结束
    public void GameOver()
    {
        Time.timeScale = 0; // 暂停游戏
        gameOverPanel.SetActive(true);
        finalScoreText.text = "Final Score: " + currentScore;

        // 保存最高分（如果当前分数是新高）
        if (currentScore > highScore)
        {
            SaveHighScore();
        }
    }

    // 重新开始游戏（需要附加到UI按钮）
    public void RestartGame()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}