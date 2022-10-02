using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    [ExecuteInEditMode]
    public class OneWayPlatform : MonoBehaviour
    {
        BoxCollider2D coll;
        SpriteRenderer spriteRenderer;

        [ExecuteInEditMode]
        private void Awake()
        {
            coll = GetComponent<BoxCollider2D>();
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

#if UNITY_EDITOR
        [ExecuteInEditMode]
        private void Update()
        {
            if (coll == null || spriteRenderer == null) return;
            coll.size = spriteRenderer.size;
        }
#endif
    }
}
