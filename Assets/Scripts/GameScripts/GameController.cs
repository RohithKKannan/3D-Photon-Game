using UnityEngine.SceneManagement;
using Photon.Pun;

public class GameController : MonoBehaviourPunCallbacks
{
    public void QuitGame()
    {
        PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftRoom()
    {
        SceneManager.LoadScene(1);
    }
}
