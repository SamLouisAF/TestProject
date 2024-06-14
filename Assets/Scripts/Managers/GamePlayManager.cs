using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UniRx;

public class GamePlayManager : MonoBehaviour
{
    [Header("Autio")]
    [SerializeField] private AudioManager audioManager;
    [Header("Player")]
    [SerializeField] private Transform playerSpawnPoint;
    [SerializeField] private PlayerCharacter player;
    [Header("To Spawn")]
    [SerializeField] private List<GameObject> interactables;
    [SerializeField] private List<Transform> spawnLoc;
    [SerializeField] private GameObject spawnParent;
    [Header("Game Values")]
    [SerializeField] private int totalCoins;
    [SerializeField] private int playerCoins;
    [Header("Game Event")]
    [SerializeField] private GameEvent startGame;
    [SerializeField] private GameEvent endGame;


    private void Start()
    {
        if (player != null) {
            player.CoinsChanged.Subscribe(coins =>
            {
                playerCoins = coins;
                if (playerCoins == totalCoins)
                {
                    endGame.Raise();
                    audioManager.Stop("Main Music");
                    audioManager.Stop("Ambiance 1");
                    audioManager.PlayOneShot("Success");
                    player.GetComponent<InputManager>().enabled = false;
                }
            }).AddTo(this);

        }
    }
    public void StartGame()
    {
        audioManager.Play("Main Music");
        audioManager.Play("Ambiance 1");
        spawnInteractables();
        player.transform.position = playerSpawnPoint.position;
        player.Spawn();
        
    }

    public void spawnInteractables()
    {
        totalCoins = 0;
        if (spawnParent.transform.childCount > 0)
        {
            foreach (Transform child in spawnParent.transform)
            {
                Destroy(child.gameObject);
            }
        }
        foreach (Transform loc in spawnLoc)
        {
            int interactableIndex = Random.Range(0, interactables.Count);
            if (interactableIndex == 1)
            {
                totalCoins ++;
            }
            Instantiate(interactables[interactableIndex], loc.position, Quaternion.identity, spawnParent.transform);

        }
    }
}
