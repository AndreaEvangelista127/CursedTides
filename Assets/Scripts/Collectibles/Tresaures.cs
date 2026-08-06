using System;
using UnityEngine;
using System.Collections;

public class Tresaures : MonoBehaviour, IInteractable
{
    [SerializeField] private int _scoreValue;
    private Animator _animator;

    private void Start()
    {
        _animator = GetComponent<Animator>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ShowPrompt();
            PlayerInteraction playerInteraction = other.GetComponent<PlayerInteraction>();
            playerInteraction?.SetCurrentInteractable(this);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            HidePrompt();
            PlayerInteraction playerInteraction = other.GetComponent<PlayerInteraction>();
            playerInteraction?.SetCurrentInteractable(null);
        }
    }

    public void ShowPrompt()
    {
        InteractionPrompt.Instance?.Show("Press E to steal");
    }

    public void HidePrompt()
    {
        InteractionPrompt.Instance?.Hide();
    }

    public void Interact()
    {
        if (gameObject == null) return;
        ScoreManager.Instance?.Collect(_scoreValue);
        HidePrompt();
        
        // Start coroutine for animation
        StartCoroutine(OpenChest());
        
        Destroy(gameObject);

    }

    IEnumerator OpenChest()
    {
        Debug.Log("Trigger animation");
        _animator.SetTrigger("Interact");

        //yield return new WaitUntil(() => _animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f && !_animator.IsInTransition(0));
        yield return new WaitForSeconds(100.0f);
    }


    
}
