using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using System.Reflection;

public class ConnectionManager : MonoBehaviourPunCallbacks
{
    [SerializeField] Button joinButton;    // Gangzang_LJS로 이동하는 버튼
    [SerializeField] Button roomButton;    // residential_LJS로 이동하는 버튼

    void Start()
    {
        SetupPhotonNetwork();

        if (joinButton != null)
        {
            joinButton.onClick.AddListener(ConnectToFirstRoom);
        }

        if (roomButton != null)
        {
            roomButton.onClick.AddListener(ConnectToSecondRoom);
        }
    }

    // Gangzang_LJS로 이동하는 첫 번째 룸 연결
    public void ConnectToFirstRoom()
    {
        
        JoinOrCreateRoom("room1", "Gangzang_LJS");
    }

    // residential_LJS로 이동하는 두 번째 룸 연결
    public void ConnectToSecondRoom()
    {
        JoinOrCreateRoom("room2", "residential_LJS");
    }

    // PhotonNetwork 기본 설정
    private void SetupPhotonNetwork()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.NickName = "";
        PhotonNetwork.GameVersion = "1.0.0";
        PhotonNetwork.SendRate = 30;
        PhotonNetwork.SerializationRate = 30;
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();
        print(MethodInfo.GetCurrentMethod().Name);
        PhotonNetwork.JoinLobby(TypedLobby.Default);
    }

    public override void OnConnected()
    {
        base.OnConnected();

        print(MethodInfo.GetCurrentMethod().Name);
    }

    public override void OnJoinedLobby()
    {
        base.OnJoinedLobby();
        print(MethodInfo.GetCurrentMethod().Name);
    }

    private void JoinOrCreateRoom(string roomName, string sceneToLoad)
    {
        print("방 생성 중...");
        RoomOptions roomOptions = new RoomOptions
        {
            MaxPlayers = 12,
            IsVisible = true,
            IsOpen = true,
            
        };

        var customProperties = new ExitGames.Client.Photon.Hashtable
        {
            { "SceneToLoad", sceneToLoad }
        };
        roomOptions.CustomRoomProperties = customProperties;

        PhotonNetwork.JoinOrCreateRoom(roomName, roomOptions, TypedLobby.Default);
    }

    public override void OnCreatedRoom()
    {
        base.OnCreatedRoom();
        print("방 생성 완료. 방에 접속합니다...");
        
    }

    public override void OnJoinedRoom()
    {
        print("방 접속 완료. \n다른 플레이어를 기다리는 중");
        if (!PhotonNetwork.InRoom)
        {
            Debug.LogError("클라이언트가 같은 룸에 연결되지 않았습니다.");
        }
        base.OnJoinedRoom();

        string sceneToLoad = (string)PhotonNetwork.CurrentRoom.CustomProperties["SceneToLoad"];
        print(sceneToLoad);
        PhotonNetwork.LoadLevel(sceneToLoad);
    }

    public void LoadFirstScene()
    {
        if (PhotonNetwork.InRoom)
        {
            PhotonNetwork.LeaveRoom();
        }
    }

    public override void OnLeftRoom()
    {
        base.OnLeftRoom();
        PhotonNetwork.LoadLevel("Room_KGC");
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);

        print($"{newPlayer.NickName} 님이 입장하셨습니다.");
    }


    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        base.OnPlayerLeftRoom(otherPlayer);
        print($"{otherPlayer.NickName} 님이 방을 떠났습니다.");
    }
}