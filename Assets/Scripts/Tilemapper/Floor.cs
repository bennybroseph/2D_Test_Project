using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace TileMapper
{
    [ExecuteInEditMode]
    public class Floor : Tile
    {
        [SerializeField]
        protected GameObject m_PrefabToTile;

        protected SpriteRenderer m_SpriteRenderer;

        // Used to instantiate the requred objects to fill the space
        void Update()
        {
            if (gameObject == null)
            {
                Debug.LogError("This script must be attatched to a game object to function!");
                return;
            }
            // Checks to make sure the user didn't forget to add a sprite renderer
            // One is not required, but recommended for error checking
            if ((m_SpriteRenderer = GetComponent<SpriteRenderer>()) == null)
                Debug.LogWarning(name + " has no sprite renderer!");

            // Collect all children, then delete them
            List<GameObject> Children = new List<GameObject>();
            foreach (Transform child in transform)
                Children.Add(child.gameObject);
            Children.ForEach(child => DestroyImmediate(child));
            
            // Make sure the scale is set to a whole number only
            if (transform.localScale.x % 1 != 0 || transform.localScale.y % 1 != 0)
                transform.localScale = new Vector3(Mathf.Round(transform.localScale.x), Mathf.Round(transform.localScale.y), transform.localScale.z);

            // Checks to make sure the user didn't forget to add the 
            // floor prefab that will be used to tile the floor
            if (m_PrefabToTile != null)
            {
                for (int i = 0; i < transform.localScale.x; ++i)
                    for (int j = 0; j < transform.localScale.y; ++j)
                    {
                        // The positional value used assumes that this object is anchored at its center
                        GameObject TempObject = Instantiate(m_PrefabToTile,
                            new Vector3(
                                (transform.position.x + i * 0.16f) + 0.08f,
                                (transform.position.y - j * 0.16f) - 0.08f, 0.0f),
                            Quaternion.identity) as GameObject;

                        TempObject.transform.parent = transform; // Parent the new object to this one for organization reasons
                    }

                // Whenever the game is being run in the editor
                if (EditorApplication.isPlaying)
                    m_SpriteRenderer.enabled = false; // Stop displaying this object's sprite since we don't need it anymore
            }
            else
            {
                Debug.LogError(name + " has no prefab to tile!\nProviding visual confirmation...");

                // Display a different sprite when there is no prefab so that 
                // it is easier to determine which object has an issue
                m_SpriteRenderer.sprite = Resources.Load<Sprite>("NULL_SPRITE");
                m_SpriteRenderer.sprite.texture.filterMode = FilterMode.Point;
            }
        }
    }
}
