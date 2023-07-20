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
    private double startTime;
    private double timerIncrementValue;
    private double timeToDisplay;
    private GameObject thisPlayer;

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
        if (!PhotonNetwork.IsMasterClient)
            startButton.SetActive(false);
        RefreshPlayerList();
    }

    public void StartGame()
    {
        if (PhotonNetwork.IsMasterClient)
        {
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
            }
        }
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
            view.RPC("DestroyAllPlayers", RpcTarget.AllViaServer);
    }

    [PunRPC]
    private void DestroyAllPlayers()
    {
        if (PhotonNetwork.IsMasterClient)
            startButton.SetActive(true);
        lobbyPanel.SetActive(true);

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
