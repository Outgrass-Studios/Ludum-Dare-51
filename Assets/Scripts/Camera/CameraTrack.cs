using UnityEngine;

using UnityCamera = UnityEngine.Camera;

namespace Game.Camera
{
    public class CameraTrack : MonoBehaviour
    {
        public enum Dimension2D { X, Y }

        [SerializeField] Dimension2D dimension;
        [SerializeField] float start;
        [SerializeField] float end;
        [SerializeField] CameraTrack previousTrack;
        [SerializeField] CameraTrack nextTrack;
        [EditorButton(nameof(SetTrack))]
        [SerializeField] Vector3 offset;

        public float StartPoint => start;
        public float EndPoint => end;
        public Dimension2D Dimension => dimension;
        public Vector3 Offset => transform.position + offset;
        public CameraTrack PreviousTrack => previousTrack;
        public CameraTrack NextTrack => nextTrack;

        private void OnDrawGizmos()
        {
            const float Z_SIZE = 0.1f;
            const float POINT_RADIUS = 0.5f;

            UnityCamera cam = UnityCamera.main;

            float yRadius = cam.orthographicSize;
            float xRadius = yRadius * cam.aspect;

            Vector3 vector = GetVector();

            Vector3 size = dimension switch
            {
                Dimension2D.X => new Vector3(end - start + xRadius * 2f, yRadius * 2f, Z_SIZE),
                Dimension2D.Y => new Vector3(xRadius * 2f, end - start + yRadius * 2f, Z_SIZE),
                _ => Vector3.zero,
            };

            Gizmos.color = Color.blue;
            Gizmos.DrawWireCube(vector * (start + end) / 2f + Offset, size);
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(vector * start + Offset, POINT_RADIUS);
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(vector * end + Offset, POINT_RADIUS);
        }

        public void SetTrack()
        {
            CameraController.Main.SetTrack(this);
        }

        Vector2 GetVector()
        {
            return dimension switch
            {
                Dimension2D.X => Vector2.right,
                Dimension2D.Y => Vector2.up,
                _ => Vector2.zero,
            };
        }

        public float GetPointOffset()
        {
            return dimension switch
            {
                Dimension2D.X => transform.position.x,
                Dimension2D.Y => transform.position.y,
                _ => 0f,
            };
        }
    }
}
