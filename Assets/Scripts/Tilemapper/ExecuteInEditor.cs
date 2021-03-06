﻿using UnityEngine;  // Required for all 'MonoBehavior's
using UnityEditor;  // Required for 'EditorAplication'

namespace TileMapper    // namespace denoting that this is part of the tile mapping functionality inside the editor
{
    // Executes 'Start()', and 'Update()' while in the editor
    [ExecuteInEditMode]
    abstract public class ExecuteInEditor : MonoBehaviour
    {
        protected bool m_EditorCompiling;   // Used to determine whether Unity is currently compiling

        /// <summary>
        /// Run automatically by Unity on 3 known conditions
        /// 1. Unity starts up
        /// 2. Unity enters play mode
        /// 3. Unity exits play mode
        /// </summary>
        protected virtual void Start()
        {
            m_EditorCompiling = false;

            // Unity is in play mode
            if (EditorApplication.isPlaying)
                OnGameStart();      // Run Game initialization
            // Unity started up || Game stopped running
            else
                OnEditorStart();    // Run Editor initialization
        }
        /// <summary>
        /// Run automatically by Unity on 2 known conditions
        /// 1. Unity is in play mode. Happens every frame
        /// 2. Unity is not in play mode. Happens when something changes in the scene
        /// </summary>
        protected virtual void Update()
        {
            if (EditorApplication.isPlaying)
                OnGameUpdate();     // Game is updating
            // Unity is not in play mode and it is compiling without us knowing
            else if (EditorApplication.isCompiling && !m_EditorCompiling)
                m_EditorCompiling = true;   // Unity is compiling
            // Unity is not compiling, but it WAS compiling last we knew
            else if (m_EditorCompiling)
            {
                m_EditorCompiling = false;
                OnEditorStart();    // Run initialization code again
            }
            // Unity was not compiling last we knew. A game object is currently selected and that 'gameObject' is this one or this 'gameObject's parent
            else if (Selection.activeGameObject != null && (Selection.activeGameObject == gameObject || Selection.activeGameObject.transform.parent == transform))
                OnEditorUpdateSelected(); // Run special update function for when this 'gameObject' is considered selected
            // Nothing currently selected or what IS selected isn't this 'gameObject' or this 'gameObject's parent
            else
                OnEditorUpdate();
        }
        
        // Virtual Functions to be implemented by inheriting class
        abstract protected void OnEditorStart();
        abstract protected void OnEditorUpdate();
        abstract protected void OnEditorUpdateSelected();
        abstract protected void OnGameStart();
        abstract protected void OnGameUpdate();
    }
}