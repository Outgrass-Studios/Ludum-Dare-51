using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    [ExecuteInEditMode]
    public class OneWayPlatform : MonoBehaviour
    {
        [SerializeField] BoxCollider2D coll;
        [SerializeField] SpriteRenderer spriteRenderer;

        private void Reset()
        {
            coll = GetComponent<BoxCollider2D>();
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        private void Awake()
        {
            UpdateCollision();
        }

#if UNITY_EDITOR
        private void Update()
        {
            UpdateCollision();
        }
#endif

        void UpdateCollision()
        {
            if (coll == null || spriteRenderer == null) return;
            coll.size = spriteRenderer.size;
        }
    }
}
