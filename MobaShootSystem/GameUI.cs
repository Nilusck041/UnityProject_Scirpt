using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameUI : MonoBehaviour
{
    public Image fadePlane;
    public GameObject gameOverUI;

    public RectTransform newWaveBanner;
    public Text newWaveTitle;
    public Text newWaveEnemyCount;
    public Text scoreUI;
    public Text gameOverScoreUI;
    public RectTransform healthBar;

    Spawner spawner;
    Player player;

    void Start()
    {
        //OnDeath±»call£¬OnGameOver±»Ö´ÐÐ
        player = FindObjectOfType<Player>();
            player.OnDeath += OnGameOver;
    }

    private void Awake()
    {
        spawner = FindObjectOfType<Spawner>();
        spawner.OnNewWave += OnNewWave;
    }

    private void Update()
    {
        scoreUI.text = ScoreKeeper.score.ToString("D6");
        float healthPercent = 0;
        if(player != null)
        {
           healthPercent = player.health / player.startingHealth;
        }
        healthBar.localScale = new Vector3(healthPercent, 1, 1);
    }

    void OnNewWave(int waveNumber)
    {
        string[] numbers = { "One", "Two", "Three", "Four", "Five" };
        newWaveTitle.text = "- Wave " + numbers [waveNumber - 1] + " -";
        string enemyCountSting = (spawner.waves[waveNumber - 1].infinite ? "infinite" : spawner.waves[waveNumber - 1].enemyCount + " ");
        newWaveEnemyCount.text = "Enemies: " + enemyCountSting;

        StopCoroutine("AnimateNewWaveBanner");
        StartCoroutine("AnimateNewWaveBanner");
    }

   void OnGameOver()
    {
        Cursor.visible = true;
        StartCoroutine(Fade(Color.clear, new Color(0,0,0,.95f), 1));
        gameOverScoreUI.text = scoreUI.text;
        scoreUI.gameObject.SetActive(false);
        healthBar.transform.parent.gameObject.SetActive(false);
        gameOverUI.SetActive(true);
    }

    IEnumerator AnimateNewWaveBanner()
    {
        float delayTime = 1.5f;
        float speed = 2.5f;
        float animationPercent = 0;
        int dir = 1;

        float endDelayTime = Time.time + 1 / speed + delayTime;

        while (animationPercent >= 0)
        {
            animationPercent += Time.deltaTime * speed * dir;

            if(animationPercent >= 1)
            {
                animationPercent = 1;
                if(Time.time > endDelayTime)
                {
                    dir = -1;
                }
            }

            newWaveBanner.anchoredPosition = Vector2.up * Mathf.Lerp(-150, 300, animationPercent);
            yield return null; 
        }
    }

    IEnumerator Fade(Color from, Color to, float time)
    {
        float speed = 1 / time;
        float percent = 0;

        while(percent < 1)
        {
            percent += Time.deltaTime * speed;
            fadePlane.color = Color.Lerp(from, to, percent);
            yield return null;
        }
    }

    //UI Input
    public void StartNewGame()
    {
        SceneManager.LoadScene("Game");
    }

    public void ReturnToMenu()
    {
        SceneManager.LoadScene("Menu");
    }
}
