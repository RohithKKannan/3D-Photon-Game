using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class CreateAndJoinRooms : MonoBehaviourPunCallbacks
{
    [SerializeField] int maxPlayers;

    [Header("Create Room")]
    [SerializeField] GameObject CreatePanel;
    [SerializeField] InputField createInput;
    [SerializeField] Text CreateProgress;

    [Header("Join Room")]
    [SerializeField] GameObject JoinPanel;
    [SerializeField] InputField joinInput;
    [SerializeField] Text JoinProgress;

    [Header("Loading Panel")]
    [SerializeField] GameObject LoadingPanel;

    private void Start()
    {
        CreatePanel.SetActive(true);
        JoinPanel.SetActive(true);
        LoadingPanel.SetActive(false);

        CreateProgress.gameObject.SetActive(false);
        JoinProgress.gameObject.SetActive(false);
    }

    public void CreateRoom()
    {
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = maxPlayers;
        PhotonNetwork.CreateRoom(createInput.text, roomOptions);

        EnableLoading();
    }

    public void JoinRoom()
    {
        PhotonNetwork.JoinRoom(joinInput.text);

        EnableLoading();
    }

    public override void OnJoinedRoom()
    {
        PhotonNetwork.LoadLevel("GameScene_" + PhotonNetwork.CurrentRoom.PlayerCount);
    }

    private void EnableLoading()
    {
        CreatePanel.SetActive(false);
        JoinPanel.SetActive(false);

        LoadingPanel.SetActive(true);
    }

    private void DisableLoading()
    {
        CreatePanel.SetActive(true);
        JoinPanel.SetActive(true);

        LoadingPanel.SetActive(false);
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        CreateProgress.text = message;
        CreateProgress.gameObject.SetActive(true);

        DisableLoading();
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        JoinProgress.text = message;
        JoinProgress.gameObject.SetActive(true);

        DisableLoading();
    }
}
