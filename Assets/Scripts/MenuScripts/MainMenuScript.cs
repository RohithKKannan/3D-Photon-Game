using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public enum PlayerColor
{
    Red, Blue, Green
}

public class MainMenuScript : MonoBehaviourPunCallbacks
{
    public static PlayerColor playerColor;

    [SerializeField] private GameObject MenuPanel;
    [SerializeField] private GameObject ConnectionStatusPanel;

    [SerializeField] private TMP_InputField playerName;
    [SerializeField] private TMP_Dropdown playerColorSelection;
    [SerializeField] private TMP_Text progressLabel;

    private void Start()
    {
        MenuPanel.SetActive(true);
        ConnectionStatusPanel.SetActive(false);
        progressLabel.gameObject.SetActive(false);
    }

    public void StartGame()
    {
        if (playerName.text == "")
            return;

        progressLabel.gameObject.SetActive(false);

        PhotonNetwork.NickName = playerName.text;
        playerColor = (PlayerColor)playerColorSelection.value;

        Connect();
    }

    private void Connect()
    {
        MenuPanel.SetActive(false);
        ConnectionStatusPanel.SetActive(true);

        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        GoNextScene();
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        progressLabel.text = cause.ToString();

        MenuPanel.SetActive(true);
        ConnectionStatusPanel.SetActive(false);
        progressLabel.gameObject.SetActive(true);
    }

    private void GoNextScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
