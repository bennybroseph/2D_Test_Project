using UnityEngine;
using UnityEditor;
using System.Collections;
using System;

namespace TileMapper
{
    [ExecuteInEditMode]
    abstract public class ExecuteInEditor : MonoBehaviour
    {
        protected bool m_EditorCompiling;

        protected virtual void Start()
        {
            m_EditorCompiling = false;

            if (EditorApplication.isPlaying)
                OnGameStart();
            else
                OnEditorStart();
        }
        protected virtual void Update()
        {
            if (EditorApplication.isPlaying)
                OnGameUpdate();
            else if (EditorApplication.isCompiling && !m_EditorCompiling)
                m_EditorCompiling = true;
            else if (m_EditorCompiling)
            {
                m_EditorCompiling = false;
                OnEditorStart();
            }
            else if (Selection.activeGameObject != null && (Selection.activeGameObject == gameObject || Selection.activeGameObject.transform.parent == transform))
                OnEditorUpdateSelected();
            else
                OnEditorUpdate();
        }
        
        abstract protected void OnEditorStart();
        abstract protected void OnEditorUpdate();
        abstract protected void OnEditorUpdateSelected();
        abstract protected void OnGameStart();
        abstract protected void OnGameUpdate();
    }
}