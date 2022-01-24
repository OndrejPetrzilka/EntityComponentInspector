using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Unity.Entities;
using Unity.Properties;
using Unity.Properties.Adapters;
using UnityEditor;

/// <summary>
/// Component editor, which uses OnEditorGUI(string label) method on component itself.
/// </summary>
class ComponentEditorMethod
{
    abstract class Visitor : IPropertyVisitorAdapter
    {
        public abstract void Init(MethodInfo method);
    }

    class Visitor<T> : Visitor, IVisit<T>
        where T : struct, IComponentData
    {
        delegate void EditorGUIDelegate(ref T value, string label);

        EditorGUIDelegate m_guiMethod;

        public override void Init(MethodInfo method)
        {
            m_guiMethod = (EditorGUIDelegate)Delegate.CreateDelegate(typeof(EditorGUIDelegate), method);
        }

        public VisitStatus Visit<TContainer>(Property<TContainer, T> property, ref TContainer container, ref T value)
        {
            EditorGUI.BeginChangeCheck();

            m_guiMethod(ref value, property.Name);

            if (EditorGUI.EndChangeCheck())
            {
                //changeTracker.MarkChanged(); 
            }
            return VisitStatus.Stop;
        }
    }

    public static bool TryGetAdapter(Type componentType, out IPropertyVisitorAdapter visitor)
    {
        var method = componentType.GetMethod("OnEditorGUI", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        if (method != null && method.ReturnType == typeof(void))
        {
            var args = method.GetParameters();
            if (args.Length == 1 && args[0].ParameterType == typeof(string))
            {
                var result = (Visitor)Activator.CreateInstance(typeof(Visitor<>).MakeGenericType(componentType));
                result.Init(method);
                visitor = result;
                return true;
            }
        }
        visitor = null;
        return false;
    }
}
