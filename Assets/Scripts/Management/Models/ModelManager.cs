using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using static UnityEngine.Debug;
using Dummiesman;
using Geometry;
using UnityEngine.Events;
using System;
using System.IO;
using System.Globalization;
using System.Linq;
using System.Collections.ObjectModel;
using System.Threading;
using UI.Panels;
using UI.Dialogs;
using UI.Popups;

namespace Management.Models
{
    public class ModelManager : MonoSingleton<ModelManager>
    {
        public struct Plane
        {
            public Vector3 b;
            public Vector3 a;
            public Vector3 c;

            public Plane(Vector3 a, Vector3 b, Vector3 c) => (this.a, this.b, this.c) = (a, b, c);
        }

        [SerializeField]
        [Range(0f, 100f)]
        private float _allowedMaxSize;

        private List<(int materialID, List<Plane>)> _materialsPlanesPairs;

        public string PathToCahchedModelFile { get; private set; }
        public string PathToCahchedMaterialsFile { get; private set; }

        public UnityEvent ModelImported;

        public UnityEvent VisibilityChanged;

        public UnityEvent FadeChanged;

        public UnityEvent ModelRemoved;

        public UnityEvent PlanesImported;

        public UnityEvent PlanesRemoved;

        public Model Model { get; private set; }

        public ReadOnlyCollection<(int materialID, List<Plane> planes)> MaterialPlanesPairs => _materialsPlanesPairs == null ? null : new ReadOnlyCollection<(int materialID, List<Plane>)>(_materialsPlanesPairs);

        public void ImportModel(string path)
        {
            GameObject go = null;

            try
            {
                go = new OBJLoader().Load(path);
            }
            catch (Exception ex)
            {
                ErrorDialog.Instance.ShowError("Модель вида имела неверный формат.", ex);
                return;
            }

            if (go.GetComponentsInChildren<MeshFilter>().Length == 0)
            {
                ErrorDialog.Instance.ShowError("Модель вида пуста.");
                return;
            }

            RemoveModel();

            var bounds = GetBounds(go);

            Clamp(go, ref bounds);

            Model = Model.Factory.MakeModel(go, bounds);
            Model.transform.SetParent(transform);

            PathToCahchedModelFile = Path.Combine(Application.temporaryCachePath, "Model.obj");
            File.Copy(path, PathToCahchedModelFile, true);

            PathToCahchedMaterialsFile = null;

            var localPathToMaterialsFile = OBJLoader.LocalPathToLastOBJMaterialFile;
            if (localPathToMaterialsFile != null)
            {
                PathToCahchedMaterialsFile = Path.Combine(Application.temporaryCachePath, "Materials.mtl");
                File.Copy(Path.Combine(Path.GetDirectoryName(path), localPathToMaterialsFile), PathToCahchedMaterialsFile, true);

                var tempModelPath = Path.Combine(Application.temporaryCachePath, "TempModel.obj");

                using (var reader = new StreamReader(File.Open(PathToCahchedModelFile, FileMode.Open, FileAccess.Read)))
                {
                    using (var writer = new StreamWriter(File.Create(tempModelPath)))
                    {
                        string line;
                        while (!(line = reader.ReadLine()).Contains("mtllib")) writer.WriteLine(line);

                        writer.WriteLine("mtllib Materials.mtl");
                        writer.Write(reader.ReadToEnd());
                    }
                }

                File.Delete(PathToCahchedModelFile);
                File.Move(tempModelPath, PathToCahchedModelFile);
            }

            ModelImported.Invoke();
            PopupManager.Instance.PopSuccess("Модель вида импортирована");
        }

        private Bounds GetBounds(GameObject go)
        {
            return BoundsUtility.GetGlobalBounds(go, BoundsUtility.BoundsCreateOption.Mesh);
        }

        private void Clamp(GameObject go, ref Bounds bounds)
        {
            var maxSize = Mathf.Max(bounds.size.x, bounds.size.y, bounds.size.z);

            if (maxSize <= _allowedMaxSize) return;

            float ratio = _allowedMaxSize / maxSize;

            go.transform.localScale *= ratio;
            bounds.center *= ratio;
            bounds.size *= ratio;
        }

        private void Center(GameObject go, Bounds bounds)
        {
            go.transform.position -= bounds.center;
        }

        public void ToggleVisibility()
        {
            Model.gameObject.SetActive(!Model.gameObject.activeSelf);

            VisibilityChanged.Invoke();
        }

        public void ToggleFade()
        {
            Model.SwitchFading();

            FadeChanged.Invoke();
        }

        public void RemoveModel()
        {
            if (!Model) return;

            Destroy(Model.gameObject);
            Model = null;

            File.Delete(PathToCahchedModelFile);
            PathToCahchedModelFile = null;

            if (PathToCahchedMaterialsFile != null)
            {
                File.Delete(PathToCahchedMaterialsFile);
                PathToCahchedMaterialsFile = null;
            }

            ModelRemoved.Invoke();
        }

        public async Task ImportPlanesAsync(string path)
        {
            DatabaseManager.Instance.ResetProgress();
            ProgressDialog.Instance.Show("Формирование математической модели");

            RemovePlanes();

            await new WaitForBackgroundThread();

            try
            {
                var info = GetVerticesInfoFromOBJ(path);

                _materialsPlanesPairs = new List<(int materialID, List<Plane>)>(info.Count);

                int index = 0;
                foreach (var pair in info)
                    _materialsPlanesPairs.Add((index++, pair.Value));
                
            }
            catch (Exception ex)
            {
                await new WaitForUpdate();

                ProgressDialog.Instance.Hide();
                ErrorDialog.Instance.ShowError("Модель плоскостей имела неверный формат.", ex);

                return;
            }

            await new WaitForUpdate();
            
            PlanesImported.Invoke();
        }

        public void ImportPlanes(List<(int materialID, List<Plane>)> planes)
        {
            _materialsPlanesPairs = planes;

            PlanesImported.Invoke();
        }

        public Dictionary<string, List<Plane>> GetVerticesInfoFromOBJ(string pathToOBJ)
        {
            Dictionary<string, List<Plane>> materialsPlanes = new Dictionary<string, List<Plane>>();

            long perPercentCount = 0;
            using (StreamReader sr = new StreamReader(pathToOBJ))
            {
                while (sr.ReadLine() != null) perPercentCount += 1;
            }

            perPercentCount = Mathf.RoundToInt(perPercentCount / 100f);

            long currentRow = 0;
            using (StreamReader sr = new StreamReader(pathToOBJ))
            {
                List<Vector3> vertices = new List<Vector3>();

                string materialName = null;
                List<int> materialVerticesIndexes = new List<int>();

                bool faceWasUsed = false;

                while (!sr.EndOfStream)
                {
                    string line = sr.ReadLine();
                    currentRow += 1;

                    if (currentRow % perPercentCount == 0)
                        DatabaseManager.Instance.SetProgress($"{((int)(currentRow / perPercentCount)).Remap(0, 100, 0, 60).ToString()}%");

                    if (line.StartsWith("v "))
                    {
                        if (faceWasUsed)
                        {
                            AddVerticesAsPlanes(materialsPlanes, materialName, IndexesToVertices(vertices, materialVerticesIndexes));

                            materialName = null;
                            materialVerticesIndexes.Clear();
                            faceWasUsed = false;
                        }

                        string vertexString = line.Substring(2);
                        vertices.Add(GetVector3FromObjString(vertexString));
                    }
                    else if (line.StartsWith("usemtl "))
                    {
                        if (materialName != null)
                            AddVerticesAsPlanes(materialsPlanes, materialName, IndexesToVertices(vertices, materialVerticesIndexes));

                        materialName = line.Substring(7);
                        materialVerticesIndexes.Clear();
                    }
                    else if (line.StartsWith("f "))
                    {
                        faceWasUsed = true;

                        int[] indexes = line.Substring(2).Split(' ').Select(p => GetFirstNumber(p)).ToArray();

                        if (indexes.Length < 3) continue;

                        indexes = indexes.Skip(2).SelectMany((i, index) => new int[] { indexes[0], indexes[index + 1], indexes[index + 2] }).ToArray();

                        for (int i = 0; i < indexes.Length; i++)
                            materialVerticesIndexes.Add(indexes[i]);
                    }
                }

                if (materialVerticesIndexes.Count != 0)
                    AddVerticesAsPlanes(materialsPlanes, materialName, IndexesToVertices(vertices, materialVerticesIndexes));
            }

            return materialsPlanes;
        }

        private void AddVerticesAsPlanes(Dictionary<string, List<Plane>> materialsPlanes, string materialName, List<Vector3> vertices)
        {
            var planes = new List<Plane>(vertices.Count / 3);
            for (int i = 0; i < vertices.Count; i += 3)
                planes.Add(new Plane(vertices[i], vertices[i + 1], vertices[i + 2]));

            materialName = materialName ?? "null";

            if (!materialsPlanes.ContainsKey(materialName))
                materialsPlanes.Add(materialName, planes);
            else
                materialsPlanes[materialName].AddRange(planes);
        }

        private List<Vector3> IndexesToVertices(List<Vector3> vertices, List<int> indexes)
        {
            List<Vector3> indexesVertices = new List<Vector3>(indexes.Count);

            for (int i = 0; i < indexes.Count; i++)
                indexesVertices.Add(vertices[indexes[i] - 1]);

            return indexesVertices;
        }

        private Vector3 GetVector3FromObjString(string str)
        {
            Vector3 vec = new Vector3(0, 0, 0);

            int i = 0;

            for (int elem = 0; elem < 3; elem++)
            {
                int e = str.IndexOf(' ', i);
                if (e < 0) e = str.Length;

                vec[elem] = float.TryParse(str.Substring(i, e - i), NumberStyles.Float, new NumberFormatInfo { NumberDecimalSeparator = "." }, out float f) ? f : 0f;
                i = EndOfCharRepetition(str, e);
            }

            return vec;
        }

        private float ParseFloat(string s) => float.TryParse(s, out float f) ? f : 0f;

        public int EndOfCharRepetition(string str, int startAt)
        {
            if (startAt < str.Length)
            {
                int i = startAt;
                char c = str[i];
                while (i < str.Length - 1)
                {
                    i++;
                    if (str[i] != c) return i;
                }
            }
            return str.Length;
        }

        private int GetFirstNumber(string str)
        {
            int i = 0;
            while (char.IsNumber(str[i])) ++i;

            return int.Parse(str.Substring(0, i));
        }

        public void RemovePlanes()
        {
            _materialsPlanesPairs = null;

            PlanesRemoved.Invoke();
        }

        private void OnDestroy() => RemoveModel();
    }
}