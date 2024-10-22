using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using TMPro;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    [Header("UI References")]
    public TMP_InputField createRoomInput;
    public TMP_InputField joinRoomInput;
    public Button createRoomButton;
    public Button joinRoomButton;
    public TMP_Text statusText;
    public GameObject lobbyUI;
    public GameObject connectingUI;

    void Start()
    {
        // UI 초기 설정
        SetUIInteractable(false);
        connectingUI.SetActive(true);
        lobbyUI.SetActive(false);
        statusText.text = "마스터 서버에 연결 중...";

        // 버튼에 리스너 추가
        createRoomButton.onClick.AddListener(CreateRoom);
        joinRoomButton.onClick.AddListener(JoinRoom);

        // 서버 연결
        PhotonNetwork.ConnectUsingSettings();
    }

    private void SetUIInteractable(bool interactable)
    {
        createRoomButton.interactable = interactable;
        joinRoomButton.interactable = interactable;
        createRoomInput.interactable = interactable;
        joinRoomInput.interactable = interactable;
    }

    public override void OnConnectedToMaster()
    {
        connectingUI.SetActive(false);
        lobbyUI.SetActive(true);
        SetUIInteractable(true);
        statusText.text = "서버에 연결됨";
    }

    public void CreateRoom()
    {
        if (string.IsNullOrEmpty(createRoomInput.text))
        {
            statusText.text = "방 이름을 입력하세요";
            return;
        }

        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 10;
        PhotonNetwork.CreateRoom(createRoomInput.text, roomOptions);
        statusText.text = "방 생성 중...";
        SetUIInteractable(false);
    }

    public void JoinRoom()
    {
        if (string.IsNullOrEmpty(joinRoomInput.text))
        {
            statusText.text = "방 이름을 입력하세요";
            return;
        }

        PhotonNetwork.JoinRoom(joinRoomInput.text);
        statusText.text = "방 참가 중...";
        SetUIInteractable(false);
    }

    public override void OnJoinedRoom()
    {
        statusText.text = "방 참가 성공!";
        PhotonNetwork.LoadLevel("CharacterCustomization");
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        statusText.text = "방 생성 실패: " + message;
        SetUIInteractable(true);
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        statusText.text = "방 참가 실패: " + message;
        SetUIInteractable(true);
    }
}