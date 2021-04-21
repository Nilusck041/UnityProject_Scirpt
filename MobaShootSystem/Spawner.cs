using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public bool devMode;

    public Wave[] waves;
    public Enemy enemy;

    LivingEntity playerEntity;
    Transform playerT;

    Wave currentWave;
    int currentWaveNumber;

    int enemiesRemainingToSpawn;
    int enemiesRemainingAlive;
    float nextSpawnTime;

    MapGenerator map;

    //存这些变量 为了达到："玩家长时间不移动会在附近刷怪" 的效果
    float timeBetweenCampingChecks = 2;
    float campThresholdDistance = 1.5f;
    float nextCampCheckTime;
    Vector3 campPositionOld;
    bool isCamping;

    bool isDisabled;

    public event System.Action<int> OnNewWave;

    private void Start()
    {
        playerEntity = FindObjectOfType<Player>();
        playerT = playerEntity.transform;

        nextCampCheckTime = timeBetweenCampingChecks + Time.time;
        campPositionOld = playerT.position;
        playerEntity.OnDeath += OnPlayerDeath;

        map = FindObjectOfType<MapGenerator>();
        NextWave();
    }

    private void Update()
    {
        if(!isDisabled)
        {
            if (Time.time > nextCampCheckTime)
            {
                nextCampCheckTime = Time.time + timeBetweenCampingChecks;

                isCamping = (Vector3.Distance(playerT.position, campPositionOld) < campThresholdDistance);
                campPositionOld = playerT.position;
            }

            //判断生成敌人的条件，生成敌人
            if ((enemiesRemainingToSpawn > 0 || currentWave.infinite )&& Time.time > nextSpawnTime)    
            {
                enemiesRemainingToSpawn--;
                nextSpawnTime = Time.time + currentWave.timeBetweenSpawn;

                StartCoroutine("SpawnEnemy");
            }
        } 

        //装后门，方便设计模式调试，快速到达下一波敌人
        if(devMode)
        {
            if(Input.GetKeyDown(KeyCode.Return))
            {
                StopCoroutine("SpawnEnemy");
                foreach (Enemy enemy in FindObjectsOfType<Enemy>())
                {
                    GameObject.Destroy(enemy.gameObject);
                }
                NextWave();
            }
        }
    }

     #region #########生成敌人
    IEnumerator SpawnEnemy()
    {
        float spawnDelay = 1;
        float tileFlashSpeed = 4;

        Transform spawnTile = map.GetRandomOpenTile();
        if(isCamping)
        {
            spawnTile = map.GetTileFromPosition(playerT.position);
        }
        Material tileMat = spawnTile.GetComponent<Renderer>().material;
        Color initialColor = tileMat.color;//会出现bug：闪烁频率慢导致地板变回原来的颜色时为闪烁的颜色
        Color flashColor = Color.red;
        float spawnTimer = 0;

        while(spawnTimer < spawnDelay)
        {
            tileMat.color = Color.Lerp(initialColor, flashColor, Mathf.PingPong(spawnTimer * tileFlashSpeed, 1));

            spawnTimer += Time.deltaTime;
            yield return null;
        }

        Enemy spawnedEnemy = Instantiate(enemy, spawnTile.position + Vector3.up, Quaternion.identity) as Enemy;
        spawnedEnemy.OnDeath += OnEnemyDeath;
        spawnedEnemy.SetCharacteristics(currentWave.moveSpeed, currentWave.hitsToKillPlayer, currentWave.enemyHealth, currentWave.skinColor);
    }
    #endregion

    void OnPlayerDeath()
    {
        isDisabled = true;
    }

    void OnEnemyDeath()
    {
        enemiesRemainingAlive--;

        if(enemiesRemainingAlive == 0)
        {
            NextWave();
        }
    }
    
    //重置玩家位置
   void ResetPlayerPosition()
    {
        playerT.position = map.GetTileFromPosition(Vector3.zero).position + Vector3.up * 3;
    }

    void NextWave()
    {
        if(currentWaveNumber > 0)
        {
            AudioManager.instance.PlaySound2D("Level Complete");
        }
        currentWaveNumber++;

        if(currentWaveNumber-1< waves.Length)
        {
            currentWave = waves[currentWaveNumber - 1];

            enemiesRemainingToSpawn = currentWave.enemyCount;
            enemiesRemainingAlive = enemiesRemainingToSpawn;

            if(OnNewWave != null)
            {
                OnNewWave(currentWaveNumber);
            }
            ResetPlayerPosition();
        }
        
    }

    [System.Serializable]
   public class Wave
    {
        public bool infinite;

        public int enemyCount;
        public float timeBetweenSpawn;

        public float moveSpeed;
        public int hitsToKillPlayer;
        public float enemyHealth;
        public Color skinColor;
    }
}
