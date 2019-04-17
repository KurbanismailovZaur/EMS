using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Geometry
{
    [Serializable]
    public struct RotatableBounds
    {
        [SerializeField]
        private Bounds _bounds;

        [SerializeField]
        private Vector3 _center;

        #region Properties
        public Quaternion Rotation;

        public Bounds Bounds
        {
            get { return new Bounds(_center, _bounds.size); }
            set
            {
                _bounds = new Bounds(Vector3.zero, value.size);
                _center = value.center;
            }
        }

        public Vector3 Center
        {
            get { return _center; }
            set
            {
                Bounds bounds = Bounds;
                bounds.center = value;
                Bounds = bounds;
            }
        }

        public Vector3 Size
        {
            get { return _bounds.size; }
            set
            {
                _bounds.size = value;
            }
        }

        public Vector3 Extents
        {
            get { return _bounds.extents; }
            set
            {
                _bounds.extents = value;
            }
        }

        #region Points
        #region Rights
        public Vector3 RightTopFront { get { return _center + (Rotation * _bounds.extents); } }

        public Vector3 RightTopBack { get { return _center + (Rotation * new Vector3(_bounds.extents.x, _bounds.extents.y, -_bounds.extents.z)); } }

        public Vector3 RightBottomFront { get { return _center + (Rotation * new Vector3(_bounds.extents.x, -_bounds.extents.y, _bounds.extents.z)); } }

        public Vector3 RightBottomBack { get { return _center + (Rotation * new Vector3(_bounds.extents.x, -_bounds.extents.y, -_bounds.extents.z)); } }
        #endregion

        #region Lefts
        public Vector3 LeftTopFront { get { return _center + (Rotation * new Vector3(-_bounds.extents.x, _bounds.extents.y, _bounds.extents.z)); } }

        public Vector3 LeftTopBack { get { return _center + (Rotation * new Vector3(-_bounds.extents.x, _bounds.extents.y, -_bounds.extents.z)); } }

        public Vector3 LeftBottomFront { get { return _center + (Rotation * new Vector3(-_bounds.extents.x, -_bounds.extents.y, _bounds.extents.z)); } }

        public Vector3 LeftBottomBack { get { return _center + (Rotation * -_bounds.extents); } }
        #endregion
        #endregion
        #endregion

        #region Methods
        #region Constructors
        public RotatableBounds(Vector3 center) : this(center, Vector3.zero, Quaternion.identity) { }

        public RotatableBounds(Vector3 center, Vector3 size) : this(center, size, Quaternion.identity) { }

        public RotatableBounds(Vector3 center, Quaternion rotation) : this(center, Vector3.zero, rotation) { }

        public RotatableBounds(Vector3 center, Vector3 size, Quaternion rotation) : this(new Bounds(center, size), rotation) { }

        public RotatableBounds(Bounds bounds, Quaternion rotation)
        {
            _bounds = new Bounds(Vector3.zero, bounds.size);
            _center = bounds.center;
            Rotation = rotation;
        }
        #endregion

        public void Encapsulate(Vector3 point, bool inWorldSpace = true)
        {
            #region Calculate point position in defferent spaces
            Vector3 pointWorldPosition = point;
            Vector3 pointLocalPosition = point;

            if (inWorldSpace)
            {
                pointLocalPosition = InverseTransformPoint(point);
            }
            else
            {
                pointWorldPosition = TransformPoint(point);
            }
            #endregion

            #region Encapsulation
            Vector3 oldCenter = _bounds.center;
            _bounds.Encapsulate(pointLocalPosition + oldCenter);
            Vector3 vectorToNewCenter = _bounds.center - oldCenter;
            _center += (Rotation * vectorToNewCenter);
            #endregion
        }

        public void Encapsulate(Bounds bounds)
        {
            Encapsulate(bounds.min);
            Encapsulate(bounds.max);
        }

        public void Encapsulate(RotatableBounds rotatableBounds)
        {
            Encapsulate(rotatableBounds.RightTopFront);
            Encapsulate(rotatableBounds.RightTopBack);
            Encapsulate(rotatableBounds.RightBottomFront);
            Encapsulate(rotatableBounds.RightBottomBack);
            Encapsulate(rotatableBounds.LeftTopFront);
            Encapsulate(rotatableBounds.LeftTopBack);
            Encapsulate(rotatableBounds.LeftBottomFront);
            Encapsulate(rotatableBounds.LeftBottomBack);
        }

        public Vector3 TransformPoint(Vector3 point)
        {
            return _center + (Rotation * point);
        }

        public Vector3 InverseTransformPoint(Vector3 point)
        {
            return Quaternion.Inverse(Rotation) * (point - _center);
        }

        public Bounds ToBounds()
        {
            Bounds bounds = new Bounds(_center, Vector3.zero);

            bounds.Encapsulate(RightTopFront);
            bounds.Encapsulate(RightTopBack);
            bounds.Encapsulate(RightBottomFront);
            bounds.Encapsulate(RightBottomBack);
            bounds.Encapsulate(LeftTopFront);
            bounds.Encapsulate(LeftTopBack);
            bounds.Encapsulate(LeftBottomFront);
            bounds.Encapsulate(LeftBottomBack);

            return bounds;
        }

        public override string ToString()
        {
            return string.Format("Rotation: {0}, {1}", Rotation, Bounds);
        }
        #endregion
    }
}