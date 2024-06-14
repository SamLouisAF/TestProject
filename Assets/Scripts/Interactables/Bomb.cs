using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour, IInteractable
{
   [SerializeField] private int damage = 10;
   [SerializeField] private Animator bombAnim;
   [SerializeField] private ParticleSystem bombFX;
   [SerializeField] private SpriteRenderer sprite;
    [SerializeField] private AudioManager audioManager;

    private void Awake()
    {
        audioManager = GameObject.FindGameObjectWithTag("Audio Manager").GetComponent<AudioManager>();
    }
    public void OnInteract(PlayerCharacter player)
    {
        audioManager.PlayOneShot("Damaged");
        sprite.color = Color.black;
        player.TakeDamage(damage);
        bombAnim.SetTrigger("Explode");
        bombFX.Stop();
    
        Destroy(gameObject,1); // One-time interaction
    }
}
