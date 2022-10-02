using UnityEngine;
using System.Collections.Generic;

namespace Game.SpriteAnimations
{
    [CreateAssetMenu(fileName = "NewSpriteAnimation", menuName = "Scriptable Objects/Sprite Animator/Sprite Animation")]
    public class SpriteAnimation : ScriptableObject
    {
        public bool loop = true;
        public List<Sprite> sprites;
    }
}