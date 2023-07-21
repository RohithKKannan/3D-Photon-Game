using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using System;

public class GameController : MonoBehaviourPunCallbacks
{
    private ExitGames.Client.Photon.Hashtable customValue;
    private bool timerStarted;
    private int playerSpawnCount;
    private double startTime;
    private double timerIncrementValue;
    private double timeToDisplay;
    private GameObject thisPlayer;
    private Dictionary<Player, PlayerController> playersAndControllers;

    [SerializeField] private double timerValue = 10;
    [SerializeField] private PhotonView view;
    [SerializeField] private PlayerController playerPrefab;
    [SerializeField] private GameObject startButton;
    [SerializeField] private TMP_Text timer;

    [Header("Player List")]
    [SerializeField] private GameObject lobbyPanel;
    [SerializeField] private GameObject playerNamePrefab;
    [SerializeField] private GameObject playerListContent;

    private void Start()
    {
        if (PhotonNetwork.IsMasterClient)
            playersAndControllers = new Dictionary<Player, PlayerController>();
        else
            startButton.SetActive(false);

        RefreshPlayerList();
    }

    public void StartGame()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            playerSpawnCount = 0;

            customValue = new ExitGames.Client.Photon.Hashtable();
            startTime = PhotonNetwork.Time;
            customValue.Add("startTime", startTime);
            PhotonNetwork.CurrentRoom.SetCustomProperties(customValue);

            timerStarted = true;
            timer.gameObject.SetActive(true);
            startButton.SetActive(false);

            view.RPC("SpawnAllPlayers", RpcTarget.AllViaServer);
            view.RPC("SetStartTime", RpcTarget.AllViaServer);
        }
    }

    [PunRPC]
    public void SetStartTime()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            startTime = double.Parse(PhotonNetwork.CurrentRoom.CustomProperties["startTime"].ToString());
            timerStarted = true;
            timer.gameObject.SetActive(true);
        }
    }

    [PunRPC]
    public void IncrementSpawnCount(Player player, PlayerController newPlayer)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            playerSpawnCount++;
            if (!playersAndControllers.ContainsKey(player))
                playersAndControllers[player] = newPlayer;
            if (playerSpawnCount == PhotonNetwork.PlayerList.Length)
                SetRandomPlayerHat();
        }
    }

    [PunRPC]
    public void SpawnAllPlayers()
    {
        lobbyPanel.SetActive(false);
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            if (player.IsLocal)
            {
                Vector3 randomPos = new Vector3(UnityEngine.Random.Range(-15, 16), 3, UnityEngine.Random.Range(-15, 16));

                object[] customPlayerData = new object[1];
                customPlayerData[0] = MainMenuScript.playerColor;

                thisPlayer = PhotonNetwork.Instantiate(playerPrefab.name, randomPos, Quaternion.identity, 0, customPlayerData);
                view.RPC("IncrementSpawnCount", RpcTarget.MasterClient, player, thisPlayer.GetComponent<PlayerController>());
            }
        }
    }

    private void SetRandomPlayerHat()
    {
        if (!PhotonNetwork.IsMasterClient)
            return;

        int playerIndex = UnityEngine.Random.Range(0, PhotonNetwork.PlayerList.Length);
        Player randomPlayer = PhotonNetwork.PlayerList[playerIndex];

        playersAndControllers[randomPlayer].EnableHat();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log("New Player " + newPlayer.NickName + " has entered the room!");
        RefreshPlayerList();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.Log("Player " + otherPlayer.NickName + " has left the room!");
        RefreshPlayerList();
    }

    public void RefreshPlayerList()
    {
        foreach (Transform item in playerListContent.transform)
        {
            Destroy(item.gameObject);
        }
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            GameObject newPlayerName = Instantiate(playerNamePrefab);
            newPlayerName.GetComponent<TMP_Text>().text = player.NickName;
            newPlayerName.transform.SetParent(playerListContent.transform);
        }
    }

    public void QuitGame()
    {
        PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftRoom()
    {
        SceneManager.LoadScene(1);
    }

    private void UpdateTimeUI()
    {
        TimeSpan timeSpan = TimeSpan.FromSeconds(timeToDisplay);
        timer.text = string.Format("{0:00}:{1:00}", timeSpan.Minutes, timeSpan.Seconds);
    }

    private void GameOver()
    {
        timerStarted = false;
        if (PhotonNetwork.IsMasterClient)
        {
            FindWinner();
            view.RPC("DestroyAllPlayers", RpcTarget.AllViaServer);
            playersAndControllers.Clear();
            startButton.SetActive(true);
        }
        lobbyPanel.SetActive(true);
    }

    private void FindWinner()
    {
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            bool foundWinner = playersAndControllers[player].IsHatEnabled();
            if (foundWinner)
            {
                Debug.Log(player.NickName + " is the WINNER!");
                return;
            }
        }
    }

    [PunRPC]
    private void DestroyAllPlayers()
    {
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            if (player.IsLocal)
            {
                PhotonNetwork.Destroy(thisPlayer);
            }
        }
    }

    private void Update()
    {
        if (!timerStarted)
            return;

        timerIncrementValue = PhotonNetwork.Time - startTime;
        timeToDisplay = timerValue - timerIncrementValue;
        UpdateTimeUI();

        if (timeToDisplay <= 0)
        {
            Debug.Log("Game over!");
            GameOver();
        }
    }
}
