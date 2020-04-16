using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
public class ExplosionController : MonoBehaviourPunCallbacks
{

    public ParticleSystem explosionEffect;
    void Start()
    {
   
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void PlayExplosion()
    {
        photonView.RPC("PrintSomething", RpcTarget.AllBuffered);
    }

    [PunRPC]
    public void PrintSomething()
    {
        Debug.Log("safg");
    }

    [PunRPC]
    public void MissileEffect()
    {
        explosionEffect.Play();
        explosionEffect.Stop();
    }
}
