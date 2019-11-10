# EntityComponentInspector
Package for implementing custom entity component inspectors.

## Installation
Add following to your package manifest (Packages/manifest.json):

`"entity-inspector-extension": "https://github.com/OndrejPetrzilka/EntityComponentInspector.git"`

## Usage
### Adding inspector code directly to component struct

Add method `void OnEditorGUI(string label)` to your component.

```
using Unity.Entities;

public struct DebugName : IComponentData
{
    public NativeString64 Value;

#if UNITY_EDITOR
    void OnEditorGUI(string label)
    {
        Value = new NativeString64(UnityEditor.EditorGUILayout.TextField(label, Value.ToString()));
    }
#endif
}
```
  
### Writing separate editor class

Create new class implementing `IComponentEditor<T>` where T is component type.

```
using Unity.Properties;
using UnityEditor;

public class DebugNameEditor : IComponentEditor<DebugName>
{
    public VisitStatus Visit<TProperty, TContainer>(IPropertyVisitor visitor, TProperty property, ref TContainer container, ref DebugName value, ref ChangeTracker changeTracker)
        where TProperty : IProperty<TContainer, DebugName>
    {
        EditorGUI.BeginChangeCheck();

        value.Value = new NativeString64(EditorGUILayout.TextField(property.GetName(), value.Value.ToString()));

        if (EditorGUI.EndChangeCheck())
        {
            changeTracker.MarkChanged();
        }
        return VisitStatus.Handled;
    }
}
```
