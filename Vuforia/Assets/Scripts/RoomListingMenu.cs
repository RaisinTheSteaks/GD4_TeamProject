using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class RoomListingMenu : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private Transform content;
    [SerializeField]
    private RoomListing roomListing;

    private List<RoomListing> listings = new List<RoomListing>();


    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        //for(int i = 0; i < roomList.Count;i++)
        //{
        //    if(roomList[i] == roomList[i+1])
        //    {
        //        roomList.RemoveAt(i + 1);
        //    }
        //}

        foreach(RoomInfo info in roomList)
        {

            //remove from room list
           
            
            int index = listings.FindIndex(x => x.RoomInfo.Name == info.Name);
            if (index != -1)
            {
                 Destroy(listings[index].gameObject);
                 listings.RemoveAt(index);
            }

            if (!info.RemovedFromList)
            {
                RoomListing listing = Instantiate(roomListing, content);
                if (listing != null)
                {
                    //Debug.Log(info.ToStringFull());
                    listing.SetRoomInfo(info);
                    listings.Add(listing);
                }

            }



        }
    }

}
