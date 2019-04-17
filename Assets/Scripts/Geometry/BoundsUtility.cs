using Geometry.Exceptions;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Geometry
{
    public static class BoundsUtility
    {
        public enum BoundsCreateOption
        {
            Transform,
            Mesh,
            TransformAndMesh
        }
        
        public static RotatableBounds GetLocalBound(GameObject gameObject, BoundsCreateOption option)
        {
            RotatableBounds rotatableBounds = new RotatableBounds();

            bool isCenterSettedByTransform = false;

            if (option == BoundsCreateOption.Transform || option == BoundsCreateOption.TransformAndMesh)
            {
                Transform[] transforms = gameObject.GetComponentsInChildren<Transform>();

                rotatableBounds = new RotatableBounds(transforms[0].position, transforms[0].rotation);
                isCenterSettedByTransform = true;

                for (int i = 1; i < transforms.Length; i++)
                {
                    rotatableBounds.Encapsulate(transforms[i].position);
                }
            }
            
            if (option == BoundsCreateOption.Mesh || option == BoundsCreateOption.TransformAndMesh)
            {
                MeshFilter[] meshFilters = gameObject.GetComponentsInChildren<MeshFilter>();

                if (!isCenterSettedByTransform)
                {
                    if (meshFilters.Length == 0)
                    {
                        throw new BoundsCreationException("Object and it's childs not have MeshFilter component");
                    }
                    else
                    {
                        Vector3 boundsCenter = meshFilters[0].transform.TransformPoint(meshFilters[0].sharedMesh.bounds.center);
                        rotatableBounds = new RotatableBounds(boundsCenter, gameObject.transform.rotation);
                    }
                }

                for (int i = 0; i < meshFilters.Length; i++)
                {
                    Bounds meshBounds = meshFilters[i].sharedMesh.bounds;
                    meshBounds.center = meshFilters[i].transform.TransformPoint(meshBounds.center);
                    meshBounds.size = Vector3.Scale(meshBounds.size, meshFilters[i].transform.lossyScale);

                    rotatableBounds.Encapsulate(new RotatableBounds(meshBounds, meshFilters[i].transform.rotation));
                }
            }

            return rotatableBounds;
        }

        public static Bounds GetGlobalBounds(GameObject gameObject, BoundsCreateOption option)
        {
            Bounds bounds = new Bounds();

            bool isCenterSettedByTransform = false;

            if (option == BoundsCreateOption.Transform || option == BoundsCreateOption.TransformAndMesh)
            {
                Transform[] transforms = gameObject.GetComponentsInChildren<Transform>();

                bounds.center = transforms[0].position;
                isCenterSettedByTransform = true;

                for (int i = 1; i < transforms.Length; i++)
                {
                    bounds.Encapsulate(transforms[i].position);
                }
            }
            
            if (option == BoundsCreateOption.Mesh || option == BoundsCreateOption.TransformAndMesh)
            {
                MeshRenderer[] meshRenderers = gameObject.GetComponentsInChildren<MeshRenderer>();

                if (!isCenterSettedByTransform)
                {
                    if (meshRenderers.Length == 0)
                    {
                        throw new BoundsCreationException("Object and it's childs not have MeshFilter component");
                    }
                    else
                    {
                        bounds = meshRenderers[0].bounds;
                    }
                }

                for (int i = isCenterSettedByTransform ? 0 : 1; i < meshRenderers.Length; i++)
                {
                    bounds.Encapsulate(meshRenderers[i].bounds);
                }
            }

            return bounds;
        }
    }
}