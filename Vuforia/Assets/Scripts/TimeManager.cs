using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : MonoBehaviour {

    public ParticleSystem sparks;
    public GameObject bot;

	void Update () {

		if(Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(AnimationTest("IsShooting"));
            //sparks.Play();
        }
	}

    public IEnumerator AnimationTest(string boolName)
    {
        Animator animator = bot.GetComponent<Animator>();
        //if(Type == "Tank")
        //{
        //    animator = transform.Find("Body").GetComponent<Animator>();
        //}

        animator.SetBool(boolName, true);
        yield return new WaitForSeconds(0.8f);
        sparks.Play();
        yield return new WaitForSeconds(0.32f);
        animator.SetBool(boolName, false);


    }
}
