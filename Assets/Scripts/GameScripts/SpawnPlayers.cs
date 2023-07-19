using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class SpawnPlayers : MonoBehaviourPunCallbacks
{
    [SerializeField] private PlayerController playerPrefab;

    public void StartGame()
    {
        if (PhotonNetwork.IsMasterClient)
            SpawnAllPlayers();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log(newPlayer.NickName + " has entered the room");
    }

    [PunRPC]
    public void SpawnAllPlayers()
    {
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            if (player == PhotonNetwork.LocalPlayer)
            {
                Vector3 randomPos = new Vector3(Random.Range(-15, 16), 3, Random.Range(-15, 16));

                object[] customPlayerData = new object[1];
                customPlayerData[0] = MainMenuScript.playerColor;

                PhotonNetwork.Instantiate(playerPrefab.name, randomPos, Quaternion.identity, 0, customPlayerData);
            }
        }
    }
}
