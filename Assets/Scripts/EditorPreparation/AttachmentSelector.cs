using UnityEngine;

public class AttachmentSelector : MonoBehaviour
{
    [SerializeField] private AttachmentData[] _attachments;

    public string[] GetAttachmentNames()
    {
        if (_attachments == null) return null;

        string[] names = new string[_attachments.Length];
        for (int i = 0; i < _attachments.Length; i++)
            names[i] = _attachments[i].attachmentName;
        return names;
    }

    public void SetAttachmentActive(int index, bool active)
    {
        if (index < 0 || index >= _attachments.Length) return;
        _attachments[index].attachment.SetActive(active);
    }

    public int AttachmentCount => _attachments.Length;
}
