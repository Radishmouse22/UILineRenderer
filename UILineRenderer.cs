using UnityEngine;
using UnityEngine.UI;

namespace Radishmouse
{
    [RequireComponent(typeof(CanvasRenderer))]
    public class UILineRenderer : MaskableGraphic
    {
        public Vector2[] points;

        public float thickness = 10f;
        public bool center = true;

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();

            if (points.Length < 2)
                return;

            for (int i = 0; i < points.Length-1; i++)
            {
                // Create a line segment between the next two points
                CreateLineSegment(points[i], points[i+1], vh);

                int index = i * 5;

                // Add the line segment to the triangles array
                vh.AddTriangle(index, index+1, index+3);
                vh.AddTriangle(index+3, index+2, index);

                // These two triangles create the beveled edges
                // between line segments using the end point of
                // the last line segment and the start points of this one
                if (i != 0)
                {
                    vh.AddTriangle(index, index-1, index-3);
                    vh.AddTriangle(index+1, index-1, index-2);
                }
            }
        }

        /// <summary>
        /// Creates a rect from two points that acts as a line segment
        /// </summary>
        /// <param name="point1">The starting point of the segment</param>
        /// <param name="point2">The endint point of the segment</param>
        /// <param name="vh">The vertex helper that the segment is added to</param>
        private void CreateLineSegment(Vector3 point1, Vector3 point2, VertexHelper vh)
        {
            Vector3 offset = center ? (rectTransform.sizeDelta / 2) : Vector2.zero;

            // Create vertex template
            UIVertex vertex = UIVertex.simpleVert;
            vertex.color = color;

            // Create the start of the segment
            Quaternion point1Rotation = Quaternion.Euler(0, 0, RotatePointTowards(point1, point2) + 90);
            vertex.position = point1Rotation * new Vector3(-thickness / 2, 0);
            vertex.position += point1 - offset;
            vh.AddVert(vertex);
            vertex.position = point1Rotation * new Vector3(thickness / 2, 0);
            vertex.position += point1 - offset;
            vh.AddVert(vertex);

            // Create the end of the segment
            Quaternion point2Rotation = Quaternion.Euler(0, 0, RotatePointTowards(point2, point1) - 90);
            vertex.position = point2Rotation * new Vector3(-thickness / 2, 0);
            vertex.position += point2 - offset;
            vh.AddVert(vertex);
            vertex.position = point2Rotation * new Vector3(thickness / 2, 0);
            vertex.position += point2 - offset;
            vh.AddVert(vertex);

            // Also add the end point
            vertex.position = point2 - offset;
            vh.AddVert(vertex);
        }

        /// <summary>
        /// Gets the angle that a vertex needs to rotate to face target vertex
        /// </summary>
        /// <param name="vertex">The vertex being rotated</param>
        /// <param name="target">The vertex to rotate towards</param>
        /// <returns>The angle required to rotate vertex towards target</returns>
        private float RotatePointTowards(Vector2 vertex, Vector2 target)
        {
            return (float)(Mathf.Atan2(target.y - vertex.y, target.x - vertex.x) * (180 / Mathf.PI));
        }
    }
}
