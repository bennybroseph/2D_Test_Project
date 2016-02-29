using UnityEngine;
using UnityEditor;
using System.Collections;

namespace TileMapper
{
    public class WallMapper : Tile
    {
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
                // Checks to make sure the user didn't forget to add a sprite renderer to the anchor
                // One is not required, but recommended for error checking
                if ((m_ChildSpriteRenderer = transform.GetChild(0).GetComponent<SpriteRenderer>()) == null)
                    Debug.LogWarning(transform.GetChild(0).name + " has no sprite renderer!");
            }
            else
                Debug.LogError(name + " does not have an anchor point!");

            base.Start();
        }

        protected override void OnGameStart()
        {
            // Whenever the game is being run in the editor
            if (EditorApplication.isPlaying)
            {
                m_SpriteRenderer.enabled = false; // Stop displaying this object's sprite since we don't need it anymore
                m_ChildSpriteRenderer.enabled = false;
            }
        }
        // Works strangely. Called when the game comes out of play mode
        protected override void OnEditorStart()
        {

        }

        // Update is called once per frame but only while not in play mode
        protected override void OnEditorUpdate()
        {
            // When something changes in the editor, start fresh
            // Not efficient, but will do for now. Try using the Selection class later maybe?
            // Deletes all children of its child
            foreach (Transform ChildChild in transform.GetChild(0).transform)
                DestroyImmediate(ChildChild.gameObject);

            //for(int i = 0; i < Mathf.Round(m_GridSize.x / ))
        }

        protected void OnDrawGizmos()
        {
            // Treat the child object as if it was the parent as far as drawing the Gizmos when selected
            if (Selection.activeGameObject == transform.GetChild(0).gameObject)
                OnDrawGizmosSelected();
        }
        protected void OnDrawGizmosSelected()
        {
            Gizmos.color = new Color(1.0f, 0.0f, 0.0f, 0.2f);
            Gizmos.DrawCube(new Vector3(transform.GetChild(0).transform.position.x, transform.position.y), new Vector3(m_GridSize.x / 100.0f, m_GridSize.y / 100.0f));
            Gizmos.DrawCube(new Vector3(transform.position.x, transform.GetChild(0).transform.position.y), new Vector3(m_GridSize.x / 100.0f, m_GridSize.y / 100.0f));
        }

        public override void SnapToGrid()
        {
            base.SnapToGrid();

            if (m_ShouldSnap)
                foreach (Transform Child in transform)
                    Controller.SnapToGrid(Child.gameObject, m_UnitGridSize, m_UnitOffset);
        }
    }
}