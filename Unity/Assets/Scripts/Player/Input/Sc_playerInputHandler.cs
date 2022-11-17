using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

public class Sc_playerInputHandler : MonoBehaviour
{
    //Storage of Events for InputMapping
    public delegate void InputEvent(CallbackContext context);

    public List<InputEvent> OnJumpEvents = new List<InputEvent>();
    public List<InputEvent> OnNetEvents = new List<InputEvent>();
    public List<InputEvent> OnItemAEvents = new List<InputEvent>();
    public List<InputEvent> OnItemBEvents = new List<InputEvent>();

    //public List<InputEvent> OnRightBumperEvents = new List<InputEvent>();
    public List<InputEvent> OnInteractEvents = new List<InputEvent>();

    public List<InputEvent> OnVisorEvents = new List<InputEvent>();
    public List<InputEvent> OnMoveEvents = new List<InputEvent>();
    public List<InputEvent> OnUpEvents = new List<InputEvent>();
    public List<InputEvent> OnDownEvents = new List<InputEvent>();
    public List<InputEvent> OnRightEvents = new List<InputEvent>();
    public List<InputEvent> OnLeftEvents = new List<InputEvent>();

    public List<InputEvent> OnUseWeaponEvents = new List<InputEvent>();
    public List<InputEvent> OnSwitchWeaponEvents = new List<InputEvent>();
    public List<InputEvent> OnUseJetpackEvents = new List<InputEvent>();
    public List<InputEvent> OnSwitchJetpackEvents = new List<InputEvent>();


    public List<InputEvent> OnStartEvents = new List<InputEvent>();

    //Storage of Input Values
    [System.Serializable]
    public class Value
    {
        public float JumpFloat;
        public float NetFloat;
        public float ItemAFloat;
        public float ItemBFloat;

        public float InteractFloat;

        public float UseWeaponFloat;
        public float UseJetpackFloat;
        public float SwitchWeaponFloat;
        public float SwitchJetpackFloat;

        public Vector2 VisorVector2;
        public Vector2 MoveVector2;
        public float UpFloat;
        public float DownFloat;
        public float RightFloat;
        public float LeftFloat;
        public float StartFloat;
    }
    public Value handlerValues = new Value();

    //Hard Input Block
    public bool canInput = true;

    //Can't do more than one special action at a time
    public enum actionState { Jump, Net, ItemA, ItemB, None }
    public actionState actionType;


    void Awake()
    {
        actionType = actionState.None;
    }
    private IEnumerator delayInput(float delay)
    {
        yield return new WaitForSeconds(delay);
        canInput = true;
    }

    #region SpecialAction
    public void OnJump(CallbackContext context)
    {
        handlerValues.JumpFloat = context.ReadValue<float>();

        if (context.ReadValue<float>() == 1 && (actionType == actionState.None || actionType == actionState.Jump))
        {
            actionType = actionState.Jump;
            Events(OnJumpEvents, context);
        }
        else if (context.ReadValue<float>() == 0 && actionType == actionState.Jump)
        {
            Events(OnJumpEvents, context);
            actionType = actionState.None;
        }
    }

    public void OnNet(CallbackContext context)
    {
        handlerValues.NetFloat = context.ReadValue<float>();

        if (context.ReadValue<float>() == 1 && (actionType == actionState.None || actionType == actionState.Net))
        {
            actionType = actionState.Net;
            Events(OnNetEvents, context);
        }
        else if (context.ReadValue<float>() == 0 && actionType == actionState.Net)
        {
            Events(OnNetEvents, context);
            actionType = actionState.None;
        }
    }

    public void OnItemA(CallbackContext context)
    {
        handlerValues.ItemAFloat = context.ReadValue<float>();

        if (context.ReadValue<float>() == 1 && (actionType == actionState.None || actionType == actionState.ItemA))
        {
            actionType = actionState.ItemA;
            Events(OnItemAEvents, context);
        }
        else if (context.ReadValue<float>() == 0 && actionType == actionState.ItemA)
        {
            Events(OnItemAEvents, context);
            actionType = actionState.None;
        }
    }

    public void OnItemB(CallbackContext context)
    {
        handlerValues.ItemBFloat = context.ReadValue<float>();

        if (context.ReadValue<float>() == 1 && (actionType == actionState.None || actionType == actionState.ItemB))
        {
            actionType = actionState.ItemB;
            Events(OnItemBEvents, context);
        }
        else if (context.ReadValue<float>() == 0 && actionType == actionState.ItemB)
        {
            Events(OnItemBEvents, context);
            actionType = actionState.None;
        }
    }
    #endregion

    #region Weapon/Jetpack
    public void OnUseWeapon(CallbackContext context)
    {
        handlerValues.UseWeaponFloat = context.ReadValue<float>();

        Events(OnUseWeaponEvents, context);
    }
    public void OnUseJetpack(CallbackContext context)
    {
        handlerValues.UseJetpackFloat = context.ReadValue<float>();

        Events(OnUseJetpackEvents, context);
    }

    public void OnSwitchWeapon(CallbackContext context)
    {
        handlerValues.SwitchWeaponFloat = context.ReadValue<float>();

        Events(OnSwitchWeaponEvents, context);
    }
    public void OnSwitchJetpack(CallbackContext context)
    {
        handlerValues.SwitchJetpackFloat = context.ReadValue<float>();

        Events(OnSwitchJetpackEvents, context);
    }

    public void OnVisor(CallbackContext context)
    {
        handlerValues.VisorVector2 = context.ReadValue<Vector2>();

        Events(OnVisorEvents, context);
    }
    #endregion

    #region Movement
    public void OnMove(CallbackContext context)
    {
        handlerValues.MoveVector2 = context.ReadValue<Vector2>();

        Events(OnMoveEvents, context);
    }

    public void OnUp(CallbackContext context)
    {
        handlerValues.UpFloat = context.ReadValue<float>();
        handlerValues.MoveVector2.y = handlerValues.UpFloat + handlerValues.DownFloat;

        Events(OnUpEvents, context);
    }
    public void OnDown(CallbackContext context)
    {
        handlerValues.DownFloat = context.ReadValue<float>();
        handlerValues.MoveVector2.y = handlerValues.UpFloat + handlerValues.DownFloat;

        Events(OnDownEvents, context);
    }
    public void OnRight(CallbackContext context)
    {
        handlerValues.RightFloat = context.ReadValue<float>();
        handlerValues.MoveVector2.x = handlerValues.RightFloat + handlerValues.LeftFloat;

        Events(OnRightEvents, context);
    }
    public void OnLeft(CallbackContext context)
    {
        handlerValues.LeftFloat = context.ReadValue<float>();
        handlerValues.MoveVector2.x = handlerValues.RightFloat + handlerValues.LeftFloat;

        Events(OnLeftEvents, context);
    }
    #endregion

    #region other
    public void OnInteract(CallbackContext context)
    {
        handlerValues.InteractFloat = context.ReadValue<float>();

        Events(OnInteractEvents, context);
    }

    public void OnStart(CallbackContext context)
    {
        handlerValues.StartFloat = context.ReadValue<float>();

        Events(OnStartEvents, context);
    }
    #endregion

    private void Events(List<InputEvent> events, CallbackContext context)
    {
        if (canInput)
        {
            for (int i = 0; i < events.Count; i++)
            {
                events[i](context);
            }
        }
    }
}
