using UnityEngine;
using UnityEditor;
using System.Collections;
using System;

namespace TileMapper
{
    [ExecuteInEditMode]
    abstract public class ExecuteInEditor : MonoBehaviour
    {
        protected virtual void Start()
        {
            if (EditorApplication.isPlaying)
                OnGameStart();
            else
                OnEditorStart();
        }
        protected virtual void Update()
        {
            if (EditorApplication.isPlaying)
                OnGameUpdate();
            else if (Selection.activeGameObject != null && (Selection.activeGameObject == gameObject || Selection.activeGameObject.transform.parent == transform))
                OnEditorUpdate();
        }
        
        abstract protected void OnEditorStart();
        abstract protected void OnEditorUpdate();
        abstract protected void OnGameStart();
        abstract protected void OnGameUpdate();
    }
}