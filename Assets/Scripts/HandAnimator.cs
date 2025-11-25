// Assets/Scripts/HandAnimator.cs
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class HandAnimator : MonoBehaviour 
{
    // Variables publiques pour lier les actions d'input
    public InputActionProperty triggerAction;
    public InputActionProperty gripAction;
    
    // Variables privées pour stocker les valeurs
    private float _triggerValue;
    private float _gripValue;
    private Animator animator;
    
    void Start()
    {
        // Récupère le composant Animator sur le même GameObject
        animator = GetComponent<Animator>();
    }
    
    void Update()
    {
        // Lit les valeurs des gâchettes (entre 0 et 1)
        _triggerValue = triggerAction.action.ReadValue<float>();
        _gripValue = gripAction.action.ReadValue<float>();
        
        // Transmet ces valeurs à l'Animator pour animer la main
        animator.SetFloat("Trigger", _triggerValue);
        animator.SetFloat("Grip", _gripValue);
    }
}