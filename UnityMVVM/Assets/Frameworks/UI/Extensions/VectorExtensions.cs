using System;
using UnityEngine;

namespace MVVMToolkit.UI
{
    public static class VectorExtensions
    {
        public static Vector2 Parse(this Vector2 v, string value)
        {
            // Remove bracers
            if (value.IndexOf('(') != -1)
                value = value.Remove(value.IndexOf('('), 1);
            if (value.IndexOf(')') != -1)
                value = value.Remove(value.IndexOf(')'), 1);

            string[] temp = value.Split(',');

            if (temp.Length == 2 && float.TryParse(temp[0].Trim(), out float x) && float.TryParse(temp[1].Trim(), out float y))
            {
                v.x = x;
                v.y = y;
                return v;
            }
            else
            {
                throw new FormatException($"Value {value} is not in valid Vector2 format!");
            }
        }

        public static Vector2 FromXZToXY(this Vector3 vector)
        {
            return new Vector2(vector.x, vector.z);
        }

        public static Vector3 FromXYToXZ(this Vector2 vector)
        {
            return new Vector3(vector.x, 0f, vector.y);
        }

        public static Vector3 DirectionTo(this Vector3 from, Vector3 to)
        {
            return (to - from).normalized;
        }

        public static Vector3 WithX(this Vector3 self, float x)
        {
            return new Vector3(x, self.y, self.z);
        }

        public static Vector3 WithY(this Vector3 self, float y)
        {
            return new Vector3(self.x, y, self.z);
        }

        public static Vector3 WithZ(this Vector3 self, float z)
        {
            return new Vector3(self.x, self.y, z);
        }

        public static Vector3 GetScreenBorderTargetPosition(Vector3 targetPosition, Vector3 center)
        {
            return ClampToCameraFrustum(targetPosition, center);
        }

        private static Vector3 ClampToCameraFrustum(Vector3 target, Vector3 center)
        {
            Camera camera = Camera.main;
            if (camera == null)
                return target;

            Vector3 dir = (target - center).normalized;

            Vector3[] corners = GetCameraFrustumCornersOnGround();
            if (corners == null || corners.Length < 4)
                return target;

            Vector3 closestPoint = target;
            float closestDist = float.MaxValue;
            bool found = false;

            for (int i = 0; i < 4; i++)
            {
                Vector3 a = corners[i];
                Vector3 b = corners[(i + 1) % 4];

                if (LineSegmentsIntersect(center, center + dir * 1000f, a, b, out Vector3 intersection))
                {
                    float dist = Vector3.Distance(center, intersection);
                    if (dist < closestDist)
                    {
                        closestDist = dist;
                        closestPoint = intersection;
                        found = true;
                    }
                }
            }

            return found ? closestPoint : center;
        }

        private static Vector3[] GetCameraFrustumCornersOnGround()
        {
            Camera camera = Camera.main;
            if (camera == null)
                return null;

            Plane groundPlane = new(Vector3.up, Vector3.zero);
            Vector3[] corners = new Vector3[4];

            Vector3[] viewportCorners = new Vector3[]
            {
                new(0,0,0),
                new(1,0,0),
                new(1,1,0),
                new(0,1,0)
            };

            for (int i = 0; i < 4; i++)
            {
                Ray ray = camera.ViewportPointToRay(viewportCorners[i]);
                if (groundPlane.Raycast(ray, out float enter))
                    corners[i] = ray.GetPoint(enter);
            }

            return corners;
        }

        private static bool LineSegmentsIntersect(Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4, out Vector3 intersection)
        {
            intersection = Vector3.zero;

            Vector2 a1 = new(p1.x, p1.z);
            Vector2 a2 = new(p2.x, p2.z);
            Vector2 b1 = new(p3.x, p3.z);
            Vector2 b2 = new(p4.x, p4.z);

            Vector2 b = a2 - a1;
            Vector2 d = b2 - b1;
            float bDotDPerp = b.x * d.y - b.y * d.x;

            if (Mathf.Abs(bDotDPerp) < 0.0001f)
                return false;

            Vector2 c = b1 - a1;
            float t = (c.x * d.y - c.y * d.x) / bDotDPerp;
            if (t < 0 || t > 1)
                return false;

            float u = (c.x * b.y - c.y * b.x) / bDotDPerp;
            if (u < 0 || u > 1)
                return false;

            Vector2 intersection2D = a1 + t * b;
            intersection = new Vector3(intersection2D.x, 0f, intersection2D.y);
            return true;
        }
    }
}
