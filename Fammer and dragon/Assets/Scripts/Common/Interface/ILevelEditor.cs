using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public struct EditorProperty
{
    public string name;
    public object value;
}
public interface ILevelEditor
{
    EditorProperty[] GetEditorProperties();
    void SetEditorProperty(EditorProperty _property);
    void Sleep();
    void WakeUp();  
}
