using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EnemySpawner : MonoBehaviour
{
    // ����Ԥ�������飨��Inspector����������Ԥ���壩
    public GameObject[] enemyPrefabs;
    public Slider healthSlider;
    public Image weaponIcon;
    public Sprite[] weaponSprites;
    // Hero���������
    public Hero hero;

    // ���ɼ��ʱ��
    public float spawnInterval = 2f;

    // ��Hero����С����
    public float minDistanceFromHero = 3f;

    // ��ͬ���͵��˵�Y��λ��
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
    public TextMeshProUGUI finalScoreText;
    public TextMeshProUGUI panelText;
    public Button restartBtn;
    public GameObject nextLevelBtn;

    private float timer;
    private Camera mainCamera;


    void Start()
    {
        gameOverPanel.SetActive(false);
        mainCamera = Camera.main;
        timer = 0f; 
        instance = this;
        LoadHighScore();
        UpdateScoreUI();
        PlayerPrefs.SetInt("Scene_0_TargetScore", 300);
        PlayerPrefs.SetInt("Scene_1_TargetScore", 1000);
        PlayerPrefs.SetInt("Scene_2_TargetScore", 5000);
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
        // ���ѡ���������
        int randomIndex = Random.Range(0, enemyPrefabs.Length);
        GameObject enemyPrefab = enemyPrefabs[randomIndex];

        // ���ݵ�������ȷ��Y��λ��
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

        // ������Ļ�߽�
        float screenWidth = mainCamera.orthographicSize * mainCamera.aspect;
        float screenHeight = mainCamera.orthographicSize;

        // ����λ�ã�ȷ������Ļ������Hero���־��룩
        Vector3 spawnPos;
        int attempts = 0;
        const int maxAttempts = 10;

        do
        {
            // ���Xλ�ã���Ļ��Χ�ڣ�
            float xPos = Random.Range(-screenWidth, screenWidth);

            // ��������λ��
            spawnPos = new Vector3(xPos, yPos, 10);

            attempts++;
            if (attempts >= maxAttempts)
            {
                // ��ֹ����ѭ��
                return;
            }

        } while (Vector2.Distance(spawnPos, hero.transform.position) < minDistanceFromHero);

        // ʵ��������
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

    // ����UI��ʾ
    private void UpdateScoreUI()
    {
        scoreText.text = "��ǰ��: " + currentScore;
        highScoreText.text = "��߷�: " + highScore;
    }

    // ������߷�
    private void SaveHighScore()
    {
        string highScoreText = SceneManager.GetActiveScene().name + "_HighScore";
        PlayerPrefs.SetInt(highScoreText, highScore);
        PlayerPrefs.Save();
    }

    // ������߷�
    private void LoadHighScore()
    {
        string highScoreText = SceneManager.GetActiveScene().name + "_HighScore";
        highScore = PlayerPrefs.GetInt(highScoreText, 0);
    }

    // ��Ϸ����
    public void GameOver()
    {
        Time.timeScale = 0; // ��ͣ��Ϸ
        gameOverPanel.SetActive(true);
        restartBtn.onClick.AddListener(RestartGame);
        nextLevelBtn.GetComponent<Button>().onClick.AddListener(NextLevel);
        nextLevelBtn.GetComponent<Button>().interactable = false;
        string sceneName = SceneManager.GetActiveScene().name + "_TargetScore";
        int targetScore = PlayerPrefs.GetInt(sceneName, 0);

        panelText.text = "��аʧ�ܣ�\nĿ���:" + targetScore;
        if (currentScore >= targetScore)
        {
            panelText.text = "��а�ɹ���";
            nextLevelBtn.GetComponent<Button>().interactable = true;
            if (SceneManager.GetActiveScene().name.CompareTo("Scene_2") == 0)
                nextLevelBtn.SetActive(false);
        }

        finalScoreText.text = "��ǰ��: " + currentScore;

        // ������߷֣������ǰ�������¸ߣ�
        if (currentScore > highScore)
        {
            SaveHighScore();
        }
    }

    // ���¿�ʼ��Ϸ����Ҫ���ӵ�UI��ť��
    public void RestartGame()
    {
        gameOverPanel.SetActive(false);
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void NextLevel()
    {
        Time.timeScale = 1;
        switch (SceneManager.GetActiveScene().name)
        {
            case "Scene_0":
                SceneManager.LoadScene("Scene_1");
                break;
            case "Scene_1":
                SceneManager.LoadScene("Scene_2");
                break;
        }
    }
}