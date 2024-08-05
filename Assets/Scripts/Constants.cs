using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Constants
{
    public static class Animation
    {
        public static class Parameters
        {
            public const string Speed = "Speed";
            public const string IsClimbing = "IsClimbing";
            public const string IsGrounded = "IsGrounded";
            public const string IsJumping = "IsJumping";
            public const string IsFalling = "IsFalling";
            public const string IsSwinging = "IsSwinging";
            public const string Horizontal = "Horizontal";
            public const string Vertical = "Vertical";
            public const string YVelocity = "YVelocity";
            public const string RandomSlideAnimation = "RandomSlideAnimation";
        }
    }

    public static class Input
    {
        public const string Horizontal = "Horizontal";
        public const string Vertical = "Vertical";
        public const string Jump = "Jump";
    }

    public static class Tags
    {
        public const string Ground = "Ground";
        public const string Wall = "Wall";
    }

    public static class Layers
    {
        public const string Ground = "Ground";
        public const string Wall = "Wall";
    }
}