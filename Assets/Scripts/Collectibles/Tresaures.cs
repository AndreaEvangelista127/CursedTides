using System;
using UnityEngine;
using System.Collections;

public class Tresaures : MonoBehaviour, IInteractable
{
    [SerializeField] private int _scoreValue;
    private Animator _animator;
    private bool _isCollected = false;

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
        if (_isCollected) return;
        _isCollected = true;
        ScoreManager.Instance?.Collect(_scoreValue);
        HidePrompt();

        // Start coroutine for animation
        StartCoroutine(OpenChest());

        //Destroy(gameObject);

    }

    IEnumerator OpenChest()
    {
        Debug.Log("Trigger animation");

        _animator.SetTrigger("Interact");

        yield return new WaitWhile(() => _animator.GetCurrentAnimatorStateInfo(0).IsName("IdleChest"));

        while (true)
        {
            float timePassed = _animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
            Debug.Log(timePassed);
            if (timePassed >= 1)
                break;
            yield return null;
        }

        //yield return new WaitWhile(() => _animator.GetCurrentAnimatorStateInfo(0).IsName("OpenChest")); // Wait while the OpenChest animation is running
        //yield return new WaitUntil(() => _animator.GetCurrentAnimatorStateInfo(0).normalizedTime <= 1.0f); // Wait until the current animation is done



        //yield return null;
        Destroy(gameObject);
    }



}
