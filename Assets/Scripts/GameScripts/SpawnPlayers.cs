using UnityEngine;
using Photon.Pun;

public class SpawnPlayers : MonoBehaviour
{
    [SerializeField] private PlayerController playerPrefab;

    private void Start()
    {
        Vector3 randomPos = new Vector3(Random.Range(-8.5f, 8.5f), 3, Random.Range(-8.5f, 8.5f));
        PhotonNetwork.Instantiate(playerPrefab.name, randomPos, Quaternion.identity);
    }
}
