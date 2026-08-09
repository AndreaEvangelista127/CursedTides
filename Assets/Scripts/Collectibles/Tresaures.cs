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

        StartCoroutine(OpenChest());
    }

    IEnumerator OpenChest()
    {
        Debug.Log("Trigger animation");

        if(_animator != null)
        {
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
        }


        HidePrompt();
        Destroy(gameObject);
    }



}
