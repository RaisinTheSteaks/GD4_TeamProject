using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Photon.Pun;


namespace Tests
{
    public class ActionSelectionTestScript
    {

        [UnityTest]
        public IEnumerator TestToggleMoving()
        {
            // Use the Assert class to test conditions.
            // Use yield to skip a frame.
            GameObject playerUI = MonoBehaviour.Instantiate(Resources.Load<GameObject>("Prefabs/PlayerUI_Canvas"));
            GameObject player = MonoBehaviour.Instantiate(Resources.Load<GameObject>("_Player"));

            LogAssert.ignoreFailingMessages = true;

            GameObject button = GameObject.Find("HideUnhideButton");
            ToggleAction toggle = button.GetComponent<ToggleAction>();
          
            toggle.toggleHideWindow();

            yield return new WaitForSeconds(0.1f);
           
            if (!toggle.showWindow)
                Assert.Less(toggle.gameObject.transform.position.x, toggle.maxX);
            else
                Assert.Less(toggle.minX, toggle.gameObject.transform.position.x);
            yield return null;
            
        }

        [UnityTest]
        public IEnumerator TestAssignButton()
        {
            GameObject player1 = MonoBehaviour.Instantiate(Resources.Load<GameObject>("_Player"));
            PhotonNetwork.NickName = "player1";   
            GameManager.instance.players[0] = player1.GetComponent<PlayerController>();

            GameObject playerUI = MonoBehaviour.Instantiate(Resources.Load<GameObject>("Prefabs/PlayerUI_Canvas"));
            AssignButtonEvent buttonEvent = playerUI.GetComponent<AssignButtonEvent>();
            LogAssert.ignoreFailingMessages = true;

            Assert.AreEqual(buttonEvent.allAssigned, false);

            yield return new WaitForSeconds(0.1f);
            Assert.AreEqual(buttonEvent.allAssigned, true);


            yield return null;
        }
    }
}
