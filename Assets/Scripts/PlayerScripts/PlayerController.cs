using UnityEngine;
using Photon.Pun;

public class PlayerController : MonoBehaviour
{
    private float horizontalInput;
    private float verticalInput;

    [SerializeField] private float speed = 10f;
    [SerializeField] private PhotonView view;

    private void Update()
    {
        if (view.IsMine)
        {
            GetInput();

            if (horizontalInput != 0 || verticalInput != 0)
                Move();

            if (Input.GetKeyDown(KeyCode.Space))
                view.RPC("SendMessage", RpcTarget.All, view.Owner.NickName, "Hello World!");
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
