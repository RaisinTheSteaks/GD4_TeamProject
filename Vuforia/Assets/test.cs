using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject bot;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
           Animator animator = bot.GetComponent<Animator>();
           animator.SetBool("IsShooting", true);
        }
    }
}
