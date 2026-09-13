using System;
using UnityEngine;

namespace AkaneUtility
{
    //https://easings.net/ja#
    public static class EasingUtility
    {
        public const float PI2 = 6.2831853f;

        public static float EaseInSine(float x) => 1 - Mathf.Cos(x * MathF.PI * 0.5f);
        public static float EaseOutSine(float x) => 1 - Mathf.Sin(x * MathF.PI * 0.5f);
        public static float EaseInOutSine(float x) => -(Mathf.Cos(Mathf.PI * x) - 1) * 0.5f;

        public static float EaseInQuad(float x) => x * x;
        public static float EaseOutQuad(float x) => 1 - (1 - x) * (1 - x);
        public static float EaseInOutQuad(float x) => x < 0.5f ? 2 * x * x : 1 - (-2 * x + 2) * (-2 * x + 2) * 0.5f;

        public static float EaseInCubic(float x) => x * x * x;
        public static float EaseOutCubic(float x) => 1 - (1 - x) * (1 - x) * (1 - x);
        public static float EaseInOutCubic(float x) => x < 0.5f ? 4 * x * x * x : 1 - (-2 * x + 2) * (-2 * x + 2) * (-2 * x + 2) * 0.5f;

        public static float EaseInQuart(float x) => x * x * x * x;
        public static float EaseOutQuart(float x) => 1 - (1 - x) * (1 - x) * (1 - x) * (1 - x);
        public static float EaseInOutQuart(float x) => x < 0.5f ? 8 * x * x * x * x : 1 - (-2 * x + 2) * (-2 * x + 2) * (-2 * x + 2) * (-2 * x + 2) * 0.5f;

        public static float EaseInQuint(float x) => x * x * x * x * x;
        public static float EaseOutQuint(float x) => 1 - (1 - x) * (1 - x) * (1 - x) * (1 - x) * (1 - x);
        public static float EaseOutQuint2(float x) => 1 - Mathf.Pow(1 - x, 5);
        public static float EaseInOutQuint(float x) => x < 0.5f ? 16 * x * x * x * x * x : 1 - (-2 * x + 2) * (-2 * x + 2) * (-2 * x + 2) * (-2 * x + 2) * (-2 * x + 2) * 0.5f;

        public static float EaseInExpo(float x) => x == 0 ? 0 : Mathf.Pow(2, 10 * x - 10);
        public static float EaseOutExpo(float x) => x == 1 ? 1 : 1 - Mathf.Pow(2, -10 * x);
        public static float EaseInOutExpo(float x)
        {
            if (x == 0) { return 0; }
            if (x == 1) { return 1; }
            if (x < 0.5) { return Mathf.Pow(2, 20 * x - 10) * 0.5f; }

            return (2 - Mathf.Pow(2, -20 * x + 10)) * 0.5f;
        }

        public static float EaseInCirc(float x) => 1 - Mathf.Sqrt(1 - x * x);
        public static float EaseOutCirc(float x) => Mathf.Sqrt(1 - (x - 1) * (x - 1));
        public static float EaseInOutCirc(float x) => x < 0.5f ? (1 - Mathf.Sqrt(1 - (2 * x) * (2 * x))) * 0.5f : (Mathf.Sqrt(1 - (-2 * x + 2) * (-2 * x + 2)) + 1) * 0.5f;

        public static float EaseInBack(float x)
        {
            const float c1 = 1.70158f;
            const float c3 = c1 + 1;

            return c3 * x * x * x - c1 * x * x;
        }
        public static float EaseOutBack(float x)
        {
            const float c1 = 1.70158f;
            const float c3 = c1 + 1;

            return 1 + c3 * (x - 1) * (x - 1) * (x - 1) + c1 * (x - 1) * (x - 1);
        }
        public static float EaseInOutBack(float x)
        {
            const float c1 = 1.70158f;
            const float c2 = c1 * 1.525f;

            return x < 0.5f
              ? ((2 * x) * (2 * x) * ((c2 + 1) * 2 * x - c2)) * 0.5f
              : ((2 * x - 2) * (2 * x - 2) * ((c2 + 1) * (x * 2 - 2) + c2) + 2) * 0.5f;
        }

        public static float EaseInElastic(float x)
        {
            const float c4 = PI2 * 0.33333f;

            if (x == 0) { return 0; }
            if (x == 1) { return 1; }

            return -Mathf.Pow(2, 10 * x - 10) * Mathf.Sin((x * 10 - 10.75f) * c4);
        }
        public static float EaseOutElastic(float x)
        {
            const float c4 = PI2 * 0.33333f;

            if (x == 0) { return 0; }
            if (x == 1) { return 1; }

            return Mathf.Pow(2, -10 * x) * Mathf.Sin((x * 10 - 0.75f) * c4) + 1;
        }

        public static float EaseInOutElastic(float x)
        {
            const float c5 = PI2 * 0.22222f;

            if (x == 0) { return 0; }
            if (x == 1) { return 1; }
            if (x < 0.5f) { return -(Mathf.Pow(2, 20 * x - 10) * Mathf.Sin((20 * x - 11.125f) * c5)) * 0.5f; }

            return (Mathf.Pow(2, -20 * x + 10) * Mathf.Sin((20 * x - 11.125f) * c5)) * 0.5f + 1;
        }

        public static float EaseInBounce(float x) => 1 - EaseOutBounce(1 - x);
        public static float EaseOutBounce(float x)
        {
            const float n1 = 7.5625f;
            const float d1 = 2.75f;

            if (x < 1 / d1)
            {
                return n1 * x * x;
            }
            else if (x < 2 / d1)
            {
                x -= 1.5f / d1;
                return n1 * x * x + 0.75f;
            }
            else if (x < 2.5 / d1)
            {
                x -= 2.25f / d1;
                return n1 * x * x + 0.9375f;
            }
            else
            {
                x -= 2.625f / d1;
                return n1 * x * x + 0.984375f;
            }
        }
        public static float EaseInOutBounce(float x)
        {
            return x < 0.5f ? (1 - EaseOutBounce(1 - 2 * x)) * 0.5f : (1 + EaseOutBounce(2 * x - 1)) * 0.5f;
        }
    }
}