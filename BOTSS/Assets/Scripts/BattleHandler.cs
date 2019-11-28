using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleHandler : MonoBehaviour
{

    [SerializeField] private Transform prefabCharacter;
    private CharacterBattle player1; //These variables exist as placeholders.
    private CharacterBattle player2;
    private CharacterBattle activePlayer; //activePlayer is the player who's turn it currently is, this is the player who is able to move their character, attack, etc.
    private bool player1Turn = true;
    private bool player2Turn = false;


    private void Start()
    {
        //SpawnCharacters(true);  //Right now this just spawns a player character, this function can be used later to 
        //SpawnCharacters(false);
    }


    private void SpawnCharacters(bool isPlayer1)
    {
        Vector3 position;
        if (isPlayer1)
        {
            position = new Vector3(-7.82f, 1f, 3.74f);
        }
        else
        {
            position = new Vector3(-3.072615f, 1f, -0.3272709f);
        }
        Instantiate(prefabCharacter, position, Quaternion.identity);
    }

    void Update()
    {
        if (player1Turn)
        {
            processPlayer1Input();
        }
        else if(player2Turn)
        {
            processPlayer2Input();
        }
    }

    void processPlayer1Input()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("Space"); //We will write our actions in here going forward.
            chooseNextActiveCharacter();
        }
    }

    void processPlayer2Input()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            Debug.Log("Player 2");
            chooseNextActiveCharacter();
        }
    }

    private void chooseNextActiveCharacter() // This method sees which character is currently active, currently it writes to the console in place of player 2's turn but this will be improved upon in further work.
    {
        if (player1Turn)
        {
            player1Turn = false;
            player2Turn = true;
        }
        else if (player2Turn)
        {
            player2Turn = false;
            player1Turn = true;
        }
    }

}
