using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour, IInteractable
{
    [SerializeField] private int value = 1;
    [SerializeField] private Animator coinAnim;
    [SerializeField] private ParticleSystem coinFX;
    [SerializeField] private SpriteRenderer sprite;
    [SerializeField] private AudioManager audioManager;

    private void Awake()
    {
        audioManager = GameObject.FindGameObjectWithTag("Audio Manager").GetComponent<AudioManager>();
    }

    public void OnInteract(PlayerCharacter player)
    {
        audioManager.PlayOneShot("Coined");
        sprite.color = Color.yellow;
        sprite.sortingOrder = 2;
        player.AddCoin(value);
        coinAnim.SetTrigger("Coined");
        coinFX.Stop();
        Destroy(gameObject,1); // One-time interaction
    }
}

