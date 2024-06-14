using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using TMPro;

public class UIManager : MonoBehaviour
{
    [SerializeField] private PlayerCharacter playerCharacter;
    [SerializeField] private TextMeshProUGUI hpText;
    [SerializeField] private TextMeshProUGUI coinsText;
    [SerializeField] private Animator animator;


    private void Start()
    {
        if (playerCharacter != null)
        {
            // Set initial text values
            hpText.text = playerCharacter.HP.ToString();
            coinsText.text = playerCharacter.Coins.ToString();
            animator.SetTrigger("InitialStart");
            playerCharacter.HPChanged.Subscribe(hp =>
            {
                hpText.text =  hp.ToString();
                
            }).AddTo(this);

            playerCharacter.CoinsChanged.Subscribe(coins =>
            {
                coinsText.text = coins.ToString();
            }).AddTo(this);

            // Subscribe to player death event
            playerCharacter.PlayerDeath.Subscribe(_ =>
            {
                animator.SetTrigger("GameOver");
            }).AddTo(this);
        }
        else
        {
            hpText.text = "0";
            coinsText.text = "0";
        }
    }
}
