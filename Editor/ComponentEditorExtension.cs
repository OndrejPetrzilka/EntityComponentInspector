using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Unity.Entities;
using Unity.Entities.Editor;
using Unity.Properties;
using UnityEditor;
using UnityEngine;

public static class EntityInspectorExtension
{
    static Type m_proxyEditor = typeof(EntitySelectionProxy).Assembly.GetType("Unity.Entities.Editor.EntitySelectionProxyEditor");
    static FieldInfo m_visitorField = m_proxyEditor.GetField("visitor", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);

    /// <summary>
    /// List of existing editors.
    /// </summary>
    public static readonly List<IPropertyVisitorAdapter> Adapters = new List<IPropertyVisitorAdapter>();

    [InitializeOnLoadMethod]
    static void Init()
    {
        Selection.selectionChanged += OnSelectionChanged;
        foreach (var type in TypeCache.GetTypesDerivedFrom<IComponentEditor>())
        {
            if (!type.IsAbstract && !type.IsGenericType)
            {
                Adapters.Add((IPropertyVisitorAdapter)Activator.CreateInstance(type));
            }
        }
        foreach (var type in TypeCache.GetTypesDerivedFrom<IComponentData>())
        {
            if (!type.IsAbstract && !type.IsGenericType && ComponentEditorMethod.TryGetAdapter(type, out var adapter))
            {
                Adapters.Add(adapter);
            }
        }
    }

    private static void OnSelectionChanged()
    {
        if (Selection.activeObject != null && Selection.activeObject.GetType() == typeof(EntitySelectionProxy))
        {
            var editor = Resources.FindObjectsOfTypeAll(m_proxyEditor)[0];
            var visitor = (PropertyVisitor)m_visitorField.GetValue(editor);
            foreach (var adapter in Adapters)
            {
                visitor.AddAdapter(adapter);
            }
        }
    }
}
