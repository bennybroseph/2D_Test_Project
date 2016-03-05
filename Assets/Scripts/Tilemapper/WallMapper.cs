using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace TileMapper
{
    public class WallMapper : Tile
    {
        [SerializeField]
        protected GameObject m_Anchor;
        [SerializeField]
        protected GameObject m_PrefabToTile;

        protected SpriteRenderer m_SpriteRenderer;
        protected SpriteRenderer m_ChildSpriteRenderer;

        protected override void Start()
        {
            // Checks to make sure the user didn't forget to add a sprite renderer
            // One is not required, but recommended for error checking
            if ((m_SpriteRenderer = GetComponent<SpriteRenderer>()) == null)
                Debug.LogWarning(name + " has no sprite renderer!");

            if (transform.GetChild(0) != null)
            {
                m_Anchor = transform.GetChild(0).gameObject;
                // Checks to make sure the user didn't forget to add a sprite renderer to the anchor
                // One is not required, but recommended for error checking
                if ((m_ChildSpriteRenderer = m_Anchor.GetComponent<SpriteRenderer>()) == null)
                    Debug.LogWarning(m_Anchor.name + " has no sprite renderer!");
            }
            else
                Debug.LogError(name + " does not have an anchor point!");

            base.Start(); Debug.Log("Update");
        }

        protected override void OnGameStart()
        {
            m_SpriteRenderer.enabled = false; // Stop displaying this object's sprite since we don't need it anymore
            m_ChildSpriteRenderer.enabled = false;
        }

        // Update is called once per frame but only while not in play mode
        protected override void OnEditorUpdateSelected()
        {
            Controller.SnapToGrid(this);

            // When something changes in the editor, start fresh
            // Deletes all children of its child
            if (Selection.activeGameObject.transform.parent == transform)
            {
                Controller.SnapToGrid(m_Anchor, m_UnitGridSize, Vector3.zero);

                // Collect all children, then delete them
                List<GameObject> Children = new List<GameObject>();
                foreach (Transform Child in m_Anchor.transform)
                    Children.Add(Child.gameObject);
                Children.ForEach(child => DestroyImmediate(child));

                // This will be -1 when the anchor is to the left and 1 when it is to the right
                // This ensures that the walls get put in the correct place
                float PosCoefficient = (m_Anchor.transform.localPosition.x > 0) ? 1 : -1;
                for (int i = 0; i <= Mathf.Round(Mathf.Abs(m_Anchor.transform.localPosition.x) / m_UnitGridSize.x); ++i)
                {
                    GameObject NewObject = Instantiate(m_PrefabToTile) as GameObject;

                    NewObject.transform.parent = m_Anchor.transform;
                    NewObject.transform.position = new Vector3(transform.position.x + i * (PosCoefficient * m_UnitGridSize.x), transform.position.y);

                    NewObject = Instantiate(m_PrefabToTile) as GameObject;

                    NewObject.transform.parent = m_Anchor.transform;
                    NewObject.transform.position = new Vector3(transform.position.x + i * (PosCoefficient * m_UnitGridSize.x), m_Anchor.transform.position.y);
                }
                PosCoefficient = (m_Anchor.transform.localPosition.y > 0) ? 1 : -1;
                for (int i = 1; i < Mathf.Round(Mathf.Abs(m_Anchor.transform.localPosition.y) / m_UnitGridSize.y); ++i)
                {
                    GameObject NewObject = Instantiate(m_PrefabToTile) as GameObject;

                    NewObject.transform.parent = m_Anchor.transform;
                    NewObject.transform.position = new Vector3(transform.position.x, transform.position.y + i * (PosCoefficient * m_UnitGridSize.y));

                    NewObject = Instantiate(m_PrefabToTile) as GameObject;

                    NewObject.transform.parent = m_Anchor.transform;
                    NewObject.transform.position = new Vector3(m_Anchor.transform.position.x, transform.position.y + i * (PosCoefficient * m_UnitGridSize.y));
                }
            }
        }

        protected void OnDrawGizmos()
        {
            if (Selection.activeGameObject != null)
            {
                // Treat the child object as if it was the parent as far as drawing the Gizmos when selected
                Transform ParentTransform = Selection.activeGameObject.transform.parent;
                while (ParentTransform != null)
                {
                    if (transform == ParentTransform)
                        OnDrawGizmosSelected();
                    ParentTransform = ParentTransform.parent;
                }
            }
        }
        protected void OnDrawGizmosSelected()
        {
            Gizmos.color = new Color(1.0f, 0.0f, 0.0f, 0.2f);
            Gizmos.DrawCube(new Vector3(transform.GetChild(0).transform.position.x, transform.position.y), new Vector3(m_UnitGridSize.x, m_UnitGridSize.y));
            Gizmos.DrawCube(new Vector3(transform.position.x, transform.GetChild(0).transform.position.y), new Vector3(m_UnitGridSize.x, m_UnitGridSize.y));
        }
    }
}