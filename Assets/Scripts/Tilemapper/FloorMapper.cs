using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;

namespace TileMapper
{
    [ExecuteInEditMode]
    public class FloorMapper : Tile
    {
        [SerializeField]
        protected GameObject m_PrefabToTile;

        protected SpriteRenderer m_SpriteRenderer;

        protected override void Start()
        {
            // Checks to make sure the user didn't forget to add a sprite renderer
            // One is not required, but recommended for error checking
            if ((m_SpriteRenderer = GetComponent<SpriteRenderer>()) == null)
                Debug.LogWarning(name + " has no sprite renderer!");

            base.Start();
        }
        protected override void OnGameStart()
        {
            m_SpriteRenderer.enabled = false; // Stop displaying this object's sprite since we don't need it anymore
        }
        // Used to instantiate the requred objects to fill the space
        protected override void OnEditorUpdateSelected()
        {
            Controller.SnapToGrid(this);

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

                        // Avoid floating point error. Unnecessary in some cases, but for the most part having 0.080000001 is unacceptable
                        // Converts something like that to 0.08, which is correct.                        
                        TempObject.transform.position = new Vector3(
                            Convert.ToInt32(TempObject.transform.position.x * 100.0f) / 100.0f,
                            Convert.ToInt32(TempObject.transform.position.y * 100.0f) / 100.0f,
                            Convert.ToInt32(TempObject.transform.position.z * 100.0f) / 100.0f);
                    }
            }
            else
            {
                Debug.LogError(name + " has no prefab to tile!\nProviding visual confirmation...");

                // Display a different sprite when there is no prefab so that 
                // it is easier to determine which object has an issue
                m_SpriteRenderer.sprite = Resources.Load<Sprite>("NULL_SPRITE");
                m_SpriteRenderer.sprite.texture.filterMode = FilterMode.Point;

                m_SpriteRenderer.sprite = Sprite.Create(m_SpriteRenderer.sprite.texture, m_SpriteRenderer.sprite.rect, new Vector2(0, 1));
            }
        }
    }
}
