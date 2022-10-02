using UnityEngine;

namespace Game.SpriteAnimations
{
    public class SpriteAnimator : MonoBehaviour
    {
        public SpriteRenderer spriteRenderer;
        [Min(1)] public int framesPerSecond;
        [SerializeField] SpriteAnimation anim;

        float _time;
        int frame;

        public SpriteAnimation CurrentAnimation { get => anim; }

        private void Reset()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        private void Update()
        {
            if (spriteRenderer == null || CurrentAnimation == null || CurrentAnimation.sprites.Count == 0) return;

            _time += Time.deltaTime;

            float frameTime = 1f / framesPerSecond;

            if (_time >= frameTime)
            {
                _time -= frameTime;

                frame++;
                UpdateFrame();
            }
        }

        public void SwapAnimation(SpriteAnimation anim)
        {
            this.anim = anim;
            UpdateFrame();
        }

        public void ChangeAnimation(SpriteAnimation anim)
        {
            if (anim == this.anim)
                return;

            frame = 0;
            _time = 0f;
            SwapAnimation(anim);
        }

        void UpdateFrame()
        {
            if (spriteRenderer == null || CurrentAnimation == null || CurrentAnimation.sprites.Count == 0) return;

            if (frame >= CurrentAnimation.sprites.Count)
            {
                if (!CurrentAnimation.loop)
                    return;

                frame = 0;
            }

            spriteRenderer.sprite = CurrentAnimation.sprites[frame];
        }
    }
}