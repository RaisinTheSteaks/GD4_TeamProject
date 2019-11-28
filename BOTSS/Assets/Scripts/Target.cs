using UnityEngine;

public class Target : MonoBehaviour
{
    [SerializeField] public GameObject character;

    public GameObject[] targets;
    void Start()
    {
        targets = GameObject.FindGameObjectsWithTag("Cube");

        foreach (GameObject target in targets)
        {
            CheckIfTargetable(target);
        }
    }
    void CheckIfTargetable(GameObject target) //Highlighting object needs to be added.
    {

        float dist = Vector3.Distance(character.transform.position, target.gameObject.transform.position);
        if (dist <= 6) //Values need to be changed.
        {
            Destroy(target.gameObject);   
        }
        else
        {
            Debug.Log("Target out of range");
        }
    }
}
