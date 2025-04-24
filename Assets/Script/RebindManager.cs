using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class RebindManager : MonoBehaviour
{
    [Header("Actions à rebind")]
    public InputActionReference action1;
    public InputActionReference action2;

    [Header("Textes UI TMP")]
    public TMP_Text action1Text;
    public TMP_Text action2Text;

    private InputActionRebindingExtensions.RebindingOperation rebindOperation;

    public void StartRebindAction1() => StartRebind(action1, action1Text, 0);
    public void StartRebindAction2() => StartRebind(action2, action2Text, 0);

    private void StartRebind(InputActionReference actionRef, TMP_Text targetText, int bindingIndex)
    {
        var action = actionRef.action;
        if (action == null) return;

        action.Disable();

        // Affichage temporaire
        targetText.text = "Appuyez sur une touche...";

        // ANNULER l'ancien binding et REBINDER à sa place (grâce à l'index)
        rebindOperation = action.PerformInteractiveRebinding(bindingIndex)
            .WithControlsExcluding("Mouse")
            .OnMatchWaitForAnother(0.2f)
            .OnComplete(operation =>
            {
                action.Enable();

                string readable = InputControlPath.ToHumanReadableString(
                    action.bindings[bindingIndex].effectivePath,
                    InputControlPath.HumanReadableStringOptions.OmitDevice
                );

                targetText.text = readable;
                Debug.Log($"Touche rebindée pour {action.name} : {readable}");

                operation.Dispose();
            })
            .Start();
    }
}
