using UnityEngine;
using Photon.Pun;

public class SpawnPlayers : MonoBehaviour
{
    [SerializeField] private PlayerController playerPrefab;

    private void Start()
    {
        Vector3 randomPos = new Vector3(Random.Range(-15, 16), 3, Random.Range(-15, 16));

        object[] customPlayerData = new object[1];
        customPlayerData[0] = MainMenuScript.playerColor;

        PhotonNetwork.Instantiate(playerPrefab.name, randomPos, Quaternion.identity, 0, customPlayerData);
    }
}
