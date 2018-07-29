using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PublicProperties : MonoBehaviour
{
    public int width;
    public int height;
    public virtual EditorProperty[] GetEditorProperties()
    {
        EditorProperty[] p=new EditorProperty[1];
        p[0].name = string.Empty;
        p[0].value = null;
        return p;
    }
    public virtual void SetEditorProperty(EditorProperty _property) { }
}
