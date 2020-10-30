using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using Cinemachine;

public class GameSession : MonoBehaviour {

    [Header("Player Stat")]
    [SerializeField] int player1Lifes = 1, player2Lifes = 1;
    [SerializeField] int player1Score = 0, player2Score = 0;
    [SerializeField] GameObject player1Prefab, player2Prefab;

    [Header("HP")]
    [SerializeField] int player1HP = 300, player2HP = 300;
    [SerializeField] int maxPlayer1HP = 300, maxPlayer2HP = 300;

    [Header("MP")]
    [SerializeField] float player1MP = 100.0f, player2MP = 100.0f;
    [SerializeField] float maxPlayer1MP = 100.0f, maxPlayer2MP = 100.0f;
    [SerializeField] float regenMP1Timer = 0.0f, regenMP2Timer = 0.0f;
    [SerializeField] float regenMPTimerMax = 1.5f;

    [Header("Stamina")]
    [SerializeField] float player1Stamina = 100.0f, player2Stamina = 100.0f;
    [SerializeField] float maxPlayer1Stamina = 100.0f, maxPlayer2Stamina = 100.0f;
    [SerializeField] float reduceStamina = 30.0f;
    [SerializeField] float minDashStamina = 1.0f;

    [Header("SP")]
    [SerializeField] float player1SP = 0.0f, player2SP = 0.0f;
    [SerializeField] float maxPlayer1SP = 100.0f, maxPlayer2SP =100.0f;

    [Header("UI")]
    [SerializeField] Text player1LivesText, player2LivesText;
    [SerializeField] Text player1ScoresText, player2ScoresText;

    [Header("CheckPoint")]
    [SerializeField] bool isPassedCheckPoint = false;
    [SerializeField] Sprite passedCheckPointSprite;

    [Header("Debug")]
    [SerializeField] bool debugModeFlag = true;

    [Header("Stage Stat")]
    [SerializeField] GameObject[] enemyWaveBoundary;
    [SerializeField] int numOfEnemies = 0;
    [SerializeField] int enemyTriggerIndex = 0;
    [SerializeField] float playerRespawnDelay = 1f;
    [SerializeField] float playerDeadDelay = 1f;
    [SerializeField] float levelLoadDelay = 2f;
    [SerializeField] float levelExitSlowMoFactor = 0.2f;
    [SerializeField] bool clearFlag = false;

    [SerializeField] SimpleHealthBar p1HealthBar, p1MPBar, p1StaminaBar, p1SPBar;
    [SerializeField] SimpleHealthBar p2HealthBar, p2MPBar, p2StaminaBar, p2SPBar;
    Transform startingPoint;

    private void Awake()
    {
        int numGameSessions = FindObjectsOfType<GameSession>().Length;
        if (numGameSessions > 1)
        {
            Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
            SetNumOfPlayers();
            ResetSP();
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    // Player Respwan Point
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("Level Loaded");
        if(scene.buildIndex == 0) { return; }
        if (GameObject.FindGameObjectsWithTag("Player") == null)
        {
            if (isPassedCheckPoint)
            {
                Debug.Log("Set position at check point");
                startingPoint = GameObject.Find("CheckPoint").transform;
                GameObject.Find("CheckPoint").GetComponent<SpriteRenderer>().sprite = passedCheckPointSprite;
                Debug.Log("Player at check point pos");
            }
            else
            {
                Debug.Log("Set position at starting point");
                startingPoint = GameObject.Find("Starting Point").transform;
                Debug.Log("Player at start point pos");
            }
            GameObject player = Instantiate(player1Prefab,
                new Vector2(startingPoint.position.x, startingPoint.position.y),
                Quaternion.identity) as GameObject;
        }
        else
        {
            if (isPassedCheckPoint)
            {
                Debug.Log("Set position at check point");
                startingPoint = GameObject.Find("CheckPoint").transform;
                GameObject.Find("CheckPoint").GetComponent<SpriteRenderer>().sprite = passedCheckPointSprite;
                Debug.Log("Player at check point pos");
            }
            else
            {
                Debug.Log("Set position at starting point");
                startingPoint = GameObject.Find("Starting Point").transform;
                Debug.Log("Player at start point pos");
            }
            GameObject player = GameObject.FindGameObjectWithTag("Player") as GameObject;
            player.transform.position = startingPoint.position;
        }

    }

    private void SetNumOfPlayers()
    {
        int playerNum = FindObjectOfType<Menu>().GetPlayerNum();
        if(playerNum == 1)
        {
            GameObject.Find("Player2").SetActive(false);
        }
    }

    // Use this for initialization
    void Start() {
        player1LivesText.text = player1Lifes.ToString();
        player1ScoresText.text = player1Score.ToString();

        player2LivesText.text = player2Lifes.ToString();
        player2ScoresText.text = player2Score.ToString();
    }

    void Update()
    {
        CheckPlayersExist();
        CheckPlayer2Exist();
        if (GetNumOfEnemies() <= 0 && enemyWaveBoundary != null)
        {
            DisableEnemyWaveBoundary();
            EndBattleState();
            if(clearFlag == true)
            {
                StartCoroutine(ClearGame());
            }
        }
    }

    private void CheckPlayersExist()
    {
        Debug.Log("Num of Players: " + GameObject.FindGameObjectsWithTag("Player").Length);
        if (GameObject.FindGameObjectsWithTag("Player").Length < 1)
        {
            GameOver();
        }
    }

    private void CheckPlayer2Exist()
    {
        if(GameObject.Find("Player2") == null && GameObject.Find("Player2 Stat") != null)
        {
            GameObject.Find("Player2 Stat").SetActive(false);
        }
    }

    public void AddToScore(int pointsToAdd, int playerID)
    {
        if(playerID == 1)
        {
            player1Score += pointsToAdd;
            player1ScoresText.text = player1Score.ToString();
        }
        else
        {
            player2Score += pointsToAdd;
            player2ScoresText.text = player2Score.ToString();
        }
    }

    public void ProcessIncreasePlayerHP(int addHP, int id)
    { 
        if(id == 1)
        {
            if ((player1HP + addHP) > maxPlayer1HP)
            {
                player1HP = maxPlayer1HP;
            }
            else
            {
                player1HP += addHP;
            }
            p1HealthBar.UpdateBar(player1HP, maxPlayer1HP);
        }
        else
        {
            if ((player2HP + addHP) > maxPlayer2HP)
            {
                player2HP = maxPlayer2HP;
            }
            else
            {
                player2HP += addHP;
            }
            p2HealthBar.UpdateBar(player1HP, maxPlayer1HP);
        }
    }

    public void ProcessReducePlayerHP(int damage, int playerID = 1)
    {
        Debug.Log("Player ID: " + playerID + ", Damage: " + damage);
        if (playerID == 1)
        {
            if ((player1HP - damage) > 0)
            {
                player1HP -= damage;
            }
            else
            {
                player1HP = 0;
                ProcessPlayerDeath(1);
            }
            p1HealthBar.UpdateBar(player1HP, maxPlayer1HP);
        }
        else
        {
            if ((player2HP - damage) > 0)
            {
                player2HP -= damage;
            }
            else
            {
                player2HP = 0;
                ProcessPlayerDeath(2);
            }
            p2HealthBar.UpdateBar(player2HP, maxPlayer2HP);
        }
    }

    public void ProcessPlayerDeath(int playerID)
    {
        if (playerID == 1)
        {
            if (player1Lifes > 1)
            {
                TakeLife(1);
            }
            else
            {
                StartCoroutine(PlayerDead(1));
            }
        }
        else
        {
            if (player2Lifes > 1)
            {
                TakeLife(2);
            }
            else
            {
                StartCoroutine(PlayerDead(2));
            }
        }

        if(player1Lifes <= 0 && player2Lifes <= 0)
        {
            GameOver();
        }
    }

    public void ProcessReducePlayerMP(float reduceMP, int id)
    {
        if(id == 1)
        {
            if ((player1MP - reduceMP) > 0)
            {
                player1MP -= reduceMP;
            }
            Debug.Log("playerMP: " + player1MP + ", reduceMP: " + reduceMP);
            p1MPBar.UpdateBar(player1MP, maxPlayer1MP);
        }
        else
        {
            if ((player2MP - reduceMP) > 0)
            {
                player2MP -= reduceMP;
            }
            Debug.Log("playerMP: " + player2MP + ", reduceMP: " + reduceMP);
            p2MPBar.UpdateBar(player2MP, maxPlayer2MP);
        }
    }

    public void ProcessIncreasePlayerMP(float addMP, int id)
    {
        if (id == 1)
        {
            if ((player1MP + addMP) >= maxPlayer1MP)
            {
                player1MP = maxPlayer1MP;
            }
            else
            {
                player1MP += addMP;
            }
            Debug.Log("player1MP: " + player1MP + ", addMP: " + addMP);
            p1MPBar.UpdateBar(player1MP, maxPlayer1MP);
        }
        else
        {
            if ((player2MP + addMP) >= maxPlayer2MP)
            {
                player2MP = maxPlayer2MP;
            }
            else
            {
                player2MP += addMP;
            }
            Debug.Log("player2MP: " + player2MP + ", addMP: " + addMP);
            p2MPBar.UpdateBar(player2MP, maxPlayer2MP);
        }
    }

    public void ProcessReducePlayerStamina(int id)
    {
        if(id == 1)
        {
            if (player1Stamina > 0)
            {
                player1Stamina -= Time.deltaTime * reduceStamina;
            }
            p1StaminaBar.UpdateBar(player1Stamina, maxPlayer1Stamina);
        }
        else
        {
            if (player2Stamina > 0)
            {
                player2Stamina -= Time.deltaTime * reduceStamina;
            }
            p2StaminaBar.UpdateBar(player2Stamina, maxPlayer2Stamina);
        }
    }

    public void ProcessIncreasePlayerSP(float addSP, int playerID)
    {
        if(playerID == 1)
        {
            if (player1SP < maxPlayer1SP)
            {
                player1SP += addSP;
            }
            else
            {
                player1SP = maxPlayer1SP;
            }
            p1SPBar.UpdateBar(player1SP, maxPlayer1SP);
        }
        else
        {
            if (player2SP < maxPlayer2SP)
            {
                player2SP += addSP;
            }
            else
            {
                player2SP = maxPlayer2SP;
            }
            p2SPBar.UpdateBar(player2SP, maxPlayer2SP);
        }
    }

    public void ProcessUseSP(int playerID)
    {
        if(playerID == 1)
        {
            if (player1SP >= maxPlayer1SP)
            {
                player1SP = 0;
            }
            p1SPBar.UpdateBar(player1SP, maxPlayer1SP);
        }
        else
        {
            if (player2SP >= maxPlayer2SP)
            {
                player2SP = 0;
            }
            p2SPBar.UpdateBar(player2SP, maxPlayer2SP);
        }
    }

    // Auto regenerate MP
    public void RegenMP(int playerID)
    {
        if(playerID == 1)
        {
            if (player1MP < maxPlayer1MP && regenMP1Timer <= 0)
            {
                player1MP += Time.deltaTime * 25.0f;
                if (player1MP > maxPlayer1MP)
                {
                    player1MP = maxPlayer1MP;
                }
                p1MPBar.UpdateBar(player1MP, maxPlayer1MP);
            }

            if (regenMP1Timer > 0)
            {
                regenMP1Timer -= Time.deltaTime;
            }
        }
        else
        {
            if (player2MP < maxPlayer2MP && regenMP2Timer <= 0)
            {
                player2MP += Time.deltaTime * 25.0f;
                if (player2MP > maxPlayer2MP)
                {
                    player2MP = maxPlayer2MP;
                }
                p2MPBar.UpdateBar(player2MP, maxPlayer2MP);
            }

            if (regenMP2Timer > 0)
            {
                regenMP2Timer -= Time.deltaTime;
            }
        }
    }

    public void RegenStamina(int playerID)
    {
        if(playerID == 1)
        {
            if (player1Stamina < maxPlayer1Stamina)
            {
                player1Stamina += Time.deltaTime * 25.0f;
                if (player1Stamina > maxPlayer1Stamina)
                {
                    player1Stamina = maxPlayer1Stamina;
                }
                p1StaminaBar.UpdateBar(player1Stamina, maxPlayer1Stamina);
            }
        }
        else
        {
            if (player2Stamina < maxPlayer2Stamina)
            {
                player2Stamina += Time.deltaTime * 25.0f;
                if (player2Stamina > maxPlayer2Stamina)
                {
                    player2Stamina = maxPlayer2Stamina;
                }
                p2StaminaBar.UpdateBar(player1Stamina, maxPlayer2Stamina);
            }
        }
    }

    private void TakeLife(int playerID)
    {
        if(playerID == 1){
            player1Lifes--;
            player1LivesText.text = player1Lifes.ToString();
        }
        else
        {
            player2Lifes--;
            player2LivesText.text = player2Lifes.ToString();
        }
        //StartCoroutine(RestartLevel());
        StartCoroutine(RespwanPlayer(playerID));
    }

    IEnumerator RespwanPlayer(int playerID)
    {
        yield return new WaitForSecondsRealtime(playerRespawnDelay);
        GameObject player = (playerID == 1) ? GameObject.Find("Player1") : GameObject.Find("Player2");
        player.GetComponent<Player>().SetRespawnPosition();
        player.GetComponent<Player>().SetAlive(true);
        player.GetComponent<Player>().SetRespawnState(true);
        player.GetComponent<Animator>().SetBool("Player" + playerID + "Dying", false);
        ResetRespwanPlayerStatus(playerID);
    }

    IEnumerator PlayerDead(int playerID)
    {
        Time.timeScale = levelExitSlowMoFactor;
        yield return new WaitForSecondsRealtime(playerDeadDelay);
        Time.timeScale = 1f;

        string player = (playerID == 1) ? "Player1" : "Player2";
        string playerStat = (playerID == 1) ? "Player1 Stat" : "Player2 Stat";

        if(GameObject.Find(player) != null)
        {
            Destroy(GameObject.Find(player));
        }
        if(GameObject.Find(playerStat) != null)
        {
            GameObject.Find(playerStat).SetActive(false);
        }
    }

    IEnumerator ClearGame()
    {
        Time.timeScale = levelExitSlowMoFactor;
        yield return new WaitForSecondsRealtime(levelLoadDelay);
        Time.timeScale = 1f;

        var currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex+1);
        Destroy(gameObject);
    }

    private void GameOver()
    {
        Debug.Log("Game Over");
        ResetSP();
        StartCoroutine(ResetGameSessions());
    }

    IEnumerator RestartLevel()
    {
        Time.timeScale = levelExitSlowMoFactor;
        yield return new WaitForSecondsRealtime(levelLoadDelay);
        Time.timeScale = 1f;

        var currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex);
        ResetPlayerStatus();
    }

    IEnumerator ResetGameSessions()
    {
        Time.timeScale = levelExitSlowMoFactor;
        yield return new WaitForSecondsRealtime(levelLoadDelay);
        Time.timeScale = 1f;

        SceneManager.LoadScene(0);
        Destroy(gameObject);
    }
            
    private void ResetRespwanPlayerStatus(int playerID)
    {
        if(playerID == 1)
        {
            player1HP = maxPlayer1HP;
            p1HealthBar.UpdateBar(player1HP, maxPlayer1HP);

            player1MP = maxPlayer1MP;
            p1MPBar.UpdateBar(player1MP, maxPlayer1MP);

            player1Stamina = maxPlayer1Stamina;
            p1StaminaBar.UpdateBar(player1Stamina, maxPlayer1Stamina);
        }
        else
        {
            player2HP = maxPlayer2HP;
            p2HealthBar.UpdateBar(player2HP, maxPlayer2HP);

            player2MP = maxPlayer2MP;
            p2MPBar.UpdateBar(player2MP, maxPlayer2MP);

            player2Stamina = maxPlayer2Stamina;
            p2StaminaBar.UpdateBar(player2Stamina, maxPlayer2Stamina);
        }
    }

    private void ResetPlayerStatus()
    {
        ResetHealth();
        ResetMP();
        ResetStamina();
        ResetSP();
    }

    private void ResetHealth()
    {
        player1HP = maxPlayer1HP;
        p1HealthBar.UpdateBar(player1HP, maxPlayer1HP);


        player2HP = maxPlayer2HP;
        p2HealthBar.UpdateBar(player2HP, maxPlayer2HP);
    }

    private void ResetMP()
    {
        player1MP = maxPlayer1MP;
        p1MPBar.UpdateBar(player1MP, maxPlayer1MP);
        
        player2MP = maxPlayer2MP;
        p2MPBar.UpdateBar(player2MP, maxPlayer2MP);
    }

    private void ResetStamina()
    {
        player1Stamina = maxPlayer1Stamina;
        p1StaminaBar.UpdateBar(player1Stamina, maxPlayer1Stamina);
        
        player2Stamina = maxPlayer2Stamina;
        p2StaminaBar.UpdateBar(player2Stamina, maxPlayer2Stamina);
    }

    private void ResetSP()
    {
        player1SP = player2SP = 0f;
        p1SPBar.UpdateBar(player1SP, maxPlayer1SP);
        p1SPBar.UpdateBar(player1SP, maxPlayer1SP);
        if(GameObject.Find("P1 SPMax Text") != null)
        {
            GameObject.Find("P1 SPMax Text").GetComponent<TextMeshProUGUI>().enabled = false;
        }
        if (GameObject.Find("P2 SPMax Text") != null)
        {
            GameObject.Find("P2 SPMax Text").GetComponent<TextMeshProUGUI>().enabled = false;
        }
    }

    private void ResetRegenMPTimer()
    {
        regenMP1Timer = regenMPTimerMax;
    }

    public int GetCurrentHP(int playerID)
    {
        return (playerID == 1) ? player1HP : player2HP;
    }

    public float GetCurrentMP()
    {
        return player1MP;
    }

    public float GetCurrentStamina(int playerID)
    {
        return (playerID == 1) ?  player1Stamina : player2Stamina;
    }

    public float GetCurrentSP(int playerID)
    {
        return (playerID == 1) ? player1SP : player2SP;
    }

    public float GetMaxStamina(int playerID)
    {
        return (playerID == 1) ? maxPlayer1Stamina : maxPlayer2Stamina;
    }

    public float GetMinDashStamina()
    {
        return minDashStamina;
    }

    public float GetMaxSP(int playerID)
    {
        return (playerID == 1) ? maxPlayer1SP : maxPlayer2SP;
    }

    public void PassedCheckPoint()
    {
        isPassedCheckPoint = true;
        GameObject.Find("CheckPoint").GetComponent<SpriteRenderer>().sprite = passedCheckPointSprite;
    }
    
    public bool GetIsPassedCheckPoint()
    {
        return isPassedCheckPoint;
    }

    public bool GetDebugModeFlag()
    {
        return debugModeFlag;
    }

    public void IncreaseNumOfEnemies(int numOfEnemies)
    {
        this.numOfEnemies += numOfEnemies;
    }

    public void DecreaseNumOfEnemies()
    {
        numOfEnemies--;
    }

    int GetNumOfEnemies()
    {
        return numOfEnemies;
    }

    public void SetBoundaryObject(GameObject[] boundaryObjects)
    {
        enemyWaveBoundary = new GameObject[boundaryObjects.Length];
        enemyWaveBoundary = boundaryObjects;
    }

    private void DisableEnemyWaveBoundary()
    {
        for(int i=0; i < enemyWaveBoundary.Length; i++)
        {
            enemyWaveBoundary[i].SetActive(false);
        }
    }

    private void EndBattleState()
    {
        if(enemyTriggerIndex <= 0 ) { return; }
        string battleState = "Battle" + enemyTriggerIndex + "State";
        GameObject.Find("Battle Animator").GetComponent<Animator>().SetBool(battleState, false);
        if(GameObject.Find("Follow Player Camera Boundary")!=null && GameObject.Find("Player1") != null)
        {
            if(GameObject.Find("Follow Player Camera Boundary").GetComponent<CameraBoundary>().GetResetPlayerPosFlag() == false)
            {
                GameObject.Find("Follow Player Camera Boundary").GetComponent<CameraBoundary>().ResetPlayer2PositionAfterBattle();
                GameObject.Find("Follow Player Camera Boundary").GetComponent<CameraBoundary>().SetResetPlayerPosFlag(true);
            }
        }

        // Follow Player Camera will follow player 2 if player 1's life is 0
        if(GameObject.Find("Player1") == null && GameObject.Find("Follow Player Camera") != null)
        {
            Debug.Log("Follow Player Camera" + GameObject.Find("Follow Player Camera"));
            GameObject.Find("Follow Player Camera").GetComponent<CinemachineVirtualCamera>().Follow =
                GameObject.Find("Player2").transform;
        }
    }

    public void SetEnemyTriggerIndex(int triggerIndex)
    {
        enemyTriggerIndex = triggerIndex;
    }

    public void SetClearFlag(bool isClearGameFlag)
    {
        clearFlag = isClearGameFlag;
    }
}
