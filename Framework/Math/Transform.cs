﻿using System;

namespace Foster.Framework
{
    public interface ITransform
    {
        Vector2 Position { get; set; }
        Vector2 Scale { get; set; }
        Vector2 Origin { get; set; }
        float Rotation { get; set; }
    }

    public class Transform : ITransform
    {
        public event Action? OnChanged;

        private Transform? parent;
        private Vector2 position;
        private Vector2 origin;
        private Vector2 scale = Vector2.One;
        private float rotation;
        private Matrix3x2 matrix;
        private Matrix3x2 inverse;
        private bool dirty = true;
        
        public Transform? Parent
        {
            get => parent;
            set
            {
                if (parent != value)
                {
                    if (parent != null)
                        parent.OnChanged -= MakeDirty;

                    parent = value;

                    if (parent != null)
                        parent.OnChanged += MakeDirty;

                    MakeDirty();
                }

            }
        }

        public float X
        {
            get => Position.X;
            set => Position = new Vector2(value, Position.Y);
        }

        public float Y
        {
            get => Position.Y;
            set => Position = new Vector2(Position.X, value);
        }

        public float ScaleX
        {
            get => Scale.X;
            set => Scale = new Vector2(value, Scale.Y);
        }

        public float ScaleY
        {
            get => Scale.Y;
            set => Scale = new Vector2(Scale.X, value);
        }

        public float OriginX
        {
            get => Origin.X;
            set => Origin = new Vector2(value, Origin.Y);
        }

        public float OriginY
        {
            get => Origin.Y;
            set => Origin = new Vector2(Origin.X, value);
        }

        public Vector2 Position
        {
            get => position;
            set
            {
                if (position != value)
                {
                    position = value;
                    MakeDirty();
                }
            }
        }

        public Vector2 Origin
        {
            get => origin;
            set
            {
                if (origin != value)
                {
                    origin = value;
                    MakeDirty();
                }
            }
        }

        public Vector2 Scale
        {
            get => scale;
            set
            {
                if (scale != value)
                {
                    scale = value;
                    MakeDirty();
                }
            }
        }

        public float Rotation
        {
            get => rotation;
            set
            {
                if (rotation != value)
                {
                    rotation = value;
                    MakeDirty();
                }
            }
        }

        public Matrix3x2 Matrix
        {
            get
            {
                if (dirty)
                    Update();

                return matrix;
            }
        }

        public Matrix3x2 Inverse
        {
            get
            {
                if (dirty)
                    Update();

                return inverse;
            }
        }

        public Vector2 GlobalPosition
        {
            get
            {
                if (parent != null)
                    return Vector2.Transform(position, parent.Matrix);
                return position;
            }
            set
            {
                if (parent != null)
                    Position = Vector2.Transform(value, parent.Matrix.Invert());
                else
                    Position = value;
            }
        }

        private void Update()
        {
            matrix = Matrix3x2.CreateTranslation(-origin.X, -origin.Y) *
                             Matrix3x2.CreateScale(scale.X, scale.Y) *
                             Matrix3x2.CreateRotation(rotation) *
                             Matrix3x2.CreateTranslation(position.X, position.Y);

            if (parent != null)
                matrix = matrix * parent.Matrix;
            inverse = matrix.Invert();
            dirty = false;
        }

        private void MakeDirty()
        {
            dirty = true;
            OnChanged?.Invoke();
        }
    }
}
