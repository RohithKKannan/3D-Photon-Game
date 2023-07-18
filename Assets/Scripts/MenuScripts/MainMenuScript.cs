using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class MainMenuScript : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject MenuPanel;
    [SerializeField] private GameObject ConnectionStatusPanel;

    [SerializeField] private InputField playerName;
    [SerializeField] private Text progressLabel;

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
