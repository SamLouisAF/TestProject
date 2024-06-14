using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Medkit : MonoBehaviour, IInteractable
{
    [SerializeField] private int healthRestore = 10;
    [SerializeField] private Animator medkitAnim;
    [SerializeField] private ParticleSystem medkitFX;
    [SerializeField] private SpriteRenderer sprite;
    [SerializeField] private AudioManager audioManager;

    private void Awake()
    {
        audioManager = GameObject.FindGameObjectWithTag("Audio Manager").GetComponent<AudioManager>();
    }
    public void OnInteract(PlayerCharacter player)
    {
        audioManager.PlayOneShot("Healed");
        sprite.color = Color.green;
        sprite.sortingOrder = 2;
        player.HealPlayer(healthRestore);
        medkitAnim.SetTrigger("Heal");
        medkitFX.Stop();
        Destroy(gameObject,1); // One-time interaction
    }
}
