using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using static UnityEngine.Debug;
using System.Linq;

namespace Visuals
{
    public class Model : MonoBehaviour
    {
        #region Classes
        public static class Factory
        {
            public static Model MakeModel(GameObject go)
            {
                var model = go.AddComponent<Model>();

                model.Initialize();

                return model;
            }
        }
        #endregion

        #region Memory only
        private List<Texture> _textures = new List<Texture>();

        private List<Mesh> _meshes = new List<Mesh>();
        #endregion

        private Dictionary<Renderer, (Material[] materials, Material[] fadeMaterials)> _pairs = new Dictionary<Renderer, (Material[] materials, Material[] fadeMaterials)>();

        private bool _isFaded;

        private void Initialize()
        {
            var renderers = GetComponentsInChildren<Renderer>();

            foreach (var renderer in renderers)
            {
                var materials = renderer.sharedMaterials;
                var fadeMaterials = new Material[materials.Length];

                for (int i = 0; i < materials.Length; i++)
                {
                    fadeMaterials[i] = new Material(materials[i]);
                    fadeMaterials[i].shader = Shader.Find("Unlit/Pseudo/Fade");

                    var color = fadeMaterials[i].color;
                    color.a = 0.25f;
                    fadeMaterials[i].color = color;
                }

                _pairs.Add(renderer, (materials, fadeMaterials));
            }

            #region Memory only
            foreach (var filter in GetComponentsInChildren<MeshFilter>())
            {
                if (!_meshes.Contains(filter.sharedMesh))
                    _meshes.Add(filter.sharedMesh);
            }

            foreach (var pair in _pairs)
                foreach (var material in pair.Value.materials)
                    if (material.mainTexture && !_textures.Contains(material.mainTexture))
                        _textures.Add(material.mainTexture);
            #endregion
        }

        private void SetFade(bool state)
        {
            foreach (var pair in _pairs)
                pair.Key.sharedMaterials = state ? pair.Value.fadeMaterials : pair.Value.materials;

            _isFaded = state;
        }

        public void SwitchFading() => SetFade(!_isFaded);

        private void OnDestroy()
        {
            foreach (var texture in _textures)
                Destroy(texture);

            foreach (var pair in _pairs)
            {
                foreach (var material in pair.Value.materials)
                    Destroy(material);

                foreach (var material in pair.Value.fadeMaterials)
                    Destroy(material);
            }

            foreach (var mesh in _meshes)
                Destroy(mesh);
        }
    }
}