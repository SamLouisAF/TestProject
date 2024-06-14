using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UniRx;
using System;

public class PlayerCharacter : MonoBehaviour
{
    [Header("Managers")]
    [SerializeField] private InputManager inputManager;
    [SerializeField] private PlayerLocomotion playerLocomotion;
    [SerializeField] private AudioManager audioManager;
    [Header("Animation")]
    [SerializeField] private Animator animator;


    [Header("Player flags")]
    public bool isInteracting;
    public bool canInteract;

    [Header("Player Stats and Modifiers")]
    [SerializeField] private float interactionRange;
    [SerializeField] private int _maxHp;
    [SerializeField] private int _hp;
    [SerializeField] private int _coins;


    private Subject<int> _hpSubject = new Subject<int>();
    private Subject<int> _coinsSubject = new Subject<int>();
    private Subject<Unit> _playerDeathSubject = new Subject<Unit>();

    public IObservable<int> HPChanged => _hpSubject;
    public IObservable<int> CoinsChanged => _coinsSubject;
    public IObservable<Unit> PlayerDeath => _playerDeathSubject;
    public int HP
    {
        get => _hp;
        set
        {
            _hp = value;
            
            if (_hp <= 0)
            {
                audioManager.PlayOneShot("Death");
                audioManager.Stop("Main Music");
                audioManager.Stop("Ambiance 1");
                animator.SetBool("IsAlive", false);
                _playerDeathSubject.OnNext(Unit.Default); // Notify subscribers about player death
                inputManager.enabled = false;
            }
            _hpSubject.OnNext(_hp);// Notify subscribers
        }
    }

    public int Coins
    {
        get => _coins;
        set
        {
            _coins = value;
            _coinsSubject.OnNext(_coins); // Notify subscribers
        }
    }

    private void Awake()
    {
        inputManager = GetComponent<InputManager>();
        playerLocomotion = GetComponent<PlayerLocomotion>();
        audioManager = GameObject.FindGameObjectWithTag("Audio Manager").GetComponent<AudioManager>();
    }
    private void Start()
    {


    }
    private void Update()
    {
        inputManager.HandleAllInputs();
    }


    private void FixedUpdate()
    {
/*        playerLocomotion.HandleAllMovement();*/
    }

    public void TakeDamage(int damage)
    {
        
        HP -= damage;
        animator.SetTrigger("Hurt");
    }
    public void HealPlayer(int heal)
    {

        HP += heal;

    }
    public void AddCoin(int addedCoin)
    {

        Coins += addedCoin;

    }
    public void Spawn()
    {
        audioManager.PlayOneShot("Respawn");
        animator.SetTrigger("Spawn");
        animator.SetBool("IsAlive", true);
        inputManager.enabled = true;
        HP = _maxHp;
        Coins = 0;
    }
    public void TryInteract()
    {
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, interactionRange);
        foreach (var hitCollider in hitColliders)
        {
            IInteractable interactable = hitCollider.GetComponent<IInteractable>();
            if (interactable != null)
            {
                interactable.OnInteract(this);
                
                break; // Interact with the first valid interactable found
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionRange);
    }


}
