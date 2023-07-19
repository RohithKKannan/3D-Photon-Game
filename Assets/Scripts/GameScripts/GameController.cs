using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class GameController : MonoBehaviourPunCallbacks
{
    [SerializeField] private PhotonView view;
    [SerializeField] private PlayerController playerPrefab;
    [SerializeField] private GameObject startButton;

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
            view.RPC("SpawnAllPlayers", RpcTarget.AllViaServer);
            startButton.SetActive(false);
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
                Vector3 randomPos = new Vector3(Random.Range(-15, 16), 3, Random.Range(-15, 16));

                object[] customPlayerData = new object[1];
                customPlayerData[0] = MainMenuScript.playerColor;

                PhotonNetwork.Instantiate(playerPrefab.name, randomPos, Quaternion.identity, 0, customPlayerData);
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
}
