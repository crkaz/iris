using UnityEngine;
using UnityEngine.UI;
using Microsoft.MixedReality.Toolkit.Experimental.Dialog;
using Microsoft.MixedReality.Toolkit.Utilities;

public class DialogManager : MonoBehaviour
{
    public GameObject dialogPrefab;
    public string dialogTitle;
    public string dialogContent;
    public bool showOnStart;
    public Text debugText;

    void Start()
    {
        if (showOnStart)
        {
            this.OpenDialog();
        }
    }

    private void OpenDialog()
    {

        Dialog dialog = Dialog.Open(dialogPrefab, DialogButtonType.Yes | DialogButtonType.No, this.dialogTitle, this.dialogContent, true);
        dialog.OnClosed += OnCloseAction;
    }

    private void OnCloseAction(DialogResult obj)
    {
        ImageCapture.Instance.Capture();
        // if (debugText != null)
        // {
        //     // if (obj.Result == DialogButtonType.Yes)
        //     // {
        //     debugText.text += (obj.Result) + "\n";
        //     // }
        // }
    }
}
