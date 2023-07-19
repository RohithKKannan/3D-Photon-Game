using UnityEngine;
using Photon.Pun;

public class PlayerController : MonoBehaviour, IPunInstantiateMagicCallback
{
    private float horizontalInput;
    private float verticalInput;
    private MeshRenderer meshRenderer;

    [SerializeField] private float speed = 10f;
    [SerializeField] private PhotonView view;
    [SerializeField] Material[] materials;

    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
    }

    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        object[] instantiationData = info.photonView.InstantiationData;

        switch ((PlayerColor)instantiationData[0])
        {
            case PlayerColor.Red:
                meshRenderer.material = materials[0];
                break;
            case PlayerColor.Blue:
                meshRenderer.material = materials[1];
                break;
            case PlayerColor.Green:
                meshRenderer.material = materials[2];
                break;
        }
    }

    private void Update()
    {
        if (view.IsMine)
        {
            GetInput();

            if (horizontalInput != 0 || verticalInput != 0)
                Move();

            if (Input.GetKeyDown(KeyCode.Space))
                view.RPC("SendMessage", RpcTarget.All, view.Owner.NickName, "Good Luck!");
        }
    }

    private void GetInput()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");
    }

    private void Move()
    {
        Vector3 movement = new Vector3(horizontalInput, 0, verticalInput);
        transform.Translate(movement * speed * Time.deltaTime);
    }

    [PunRPC]
    private void SendMessage(string name, string text)
    {
        Debug.Log($"{name} : {text}");
    }
}
