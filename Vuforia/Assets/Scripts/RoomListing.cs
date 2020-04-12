using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomListing : MonoBehaviour
{
    [SerializeField]
    private Text text;

    public Text selectedRoom;


    public RoomInfo RoomInfo { get; private set; }

    public void Start()
    {
        selectedRoom = GameObject.Find("SelectedRoom").transform.Find("Text").GetComponent<Text>();
    }
    public void SetRoomInfo(RoomInfo roomInfo)
    {
        RoomInfo = roomInfo;
        text.text = roomInfo.Name;
    }

    public void updateSelectedRoom()
    {
        selectedRoom.enabled = true;
        selectedRoom.text = text.text;
    }

}
