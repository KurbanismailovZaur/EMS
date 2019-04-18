using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using static UnityEngine.Debug;
using Vectrosity;

namespace Environments
{
    public class Axes : MonoBehaviour
    {
        [SerializeField]
        private Transform _axes;

        [SerializeField]
        private Transform _grid;

        [SerializeField]
        private Transform _minor;

        [SerializeField]
        private Transform _major;

        [SerializeField]
        private Transform _perimeter;

        [SerializeField]
        private Camera _camera;

        [SerializeField]
        private Color _xColor;

        [SerializeField]
        private Color _yColor;

        [SerializeField]
        private Color _zColor;

        [SerializeField]
        private Color _lineColor;

        [SerializeField]
        private Color _majorLineColor;

        [SerializeField]
        private Color _perimeterColor;

        [SerializeField]
        [Range(1f, 8f)]
        private float _axesMagnitudeMultiplier = 1f;

        [SerializeField]
        [Range(1f, 4f)]
        private float _linesWidth = 1f;

        [SerializeField]
        [Range(4, 384)]
        private int _linesCountPerAxis = 4;

        [SerializeField]
        [Range(8f, 256f)]
        private float _lineMagnitude = 8f;

        [SerializeField]
        [Range(8f, 32f)]
        private float _invisibleAngle = 8f;

        [SerializeField]
        [Range(8f, 40f)]
        private float _visibleAngleOffset = 8f;

        public bool AxesVisibility
        {
            get => _axes.gameObject.activeSelf;
            set => _axes.gameObject.SetActive(value);
        }

        public bool GridVisibility
        {
            get => _grid.gameObject.activeSelf;
            set => _grid.gameObject.SetActive(value);
        }

        private void Start()
        {
            var xLine = CreateAxis("XAxis", _xColor, Vector3.right);
            var yLine = CreateAxis("YAxis", _yColor, Vector3.up);
            var zLine = CreateAxis("ZAxis", _zColor, Vector3.forward);

            var axes = new List<VectorLine>() { xLine, yLine, zLine };
            var lines = new List<VectorLine>();
            var majorLines = new List<VectorLine>();
            var perimeterLines = new List<VectorLine>();

            var halfLinesCountPerAxis = _linesCountPerAxis / 2;

            for (int i = -halfLinesCountPerAxis; i < halfLinesCountPerAxis - 1; i++)
            {
                if ((i + 1) % 10 != 0)
                {
                    lines.Add(CreateLine("GridLine", _minor, _lineColor, new Vector3(i + 1f, 0f, -_lineMagnitude / 2f), new Vector3(i + 1f, 0f, _lineMagnitude / 2f)));
                    lines.Add(CreateLine("GridLine", _minor, _lineColor, new Vector3(-_lineMagnitude / 2f, 0f, i + 1f), new Vector3(_lineMagnitude / 2f, 0f, i + 1f)));
                }
                else
                {
                    majorLines.Add(CreateLine("GridLine", _major, _majorLineColor, new Vector3(i + 1f, 0f, -_lineMagnitude / 2f), new Vector3(i + 1f, 0f, _lineMagnitude / 2f)));
                    majorLines.Add(CreateLine("GridLine", _major, _majorLineColor, new Vector3(-_lineMagnitude / 2f, 0f, i + 1f), new Vector3(_lineMagnitude / 2f, 0f, i + 1f)));
                }
            }

            var halfMagnitude = _lineMagnitude / 2f;

            perimeterLines.Add(CreatePerimeterLine(_perimeterColor, -halfMagnitude, -halfMagnitude, -halfMagnitude, halfMagnitude));
            perimeterLines.Add(CreatePerimeterLine(_perimeterColor, -halfMagnitude, halfMagnitude, halfMagnitude, halfMagnitude));
            perimeterLines.Add(CreatePerimeterLine(_perimeterColor, halfMagnitude, halfMagnitude, halfMagnitude, -halfMagnitude));
            perimeterLines.Add(CreatePerimeterLine(_perimeterColor, halfMagnitude, -halfMagnitude, -halfMagnitude, -halfMagnitude));
            
            StartCoroutine(UpdateLines(axes, lines, majorLines, perimeterLines));
        }

        private IEnumerator UpdateLines(List<VectorLine> axes, List<VectorLine> lines, List<VectorLine> majorLines, List<VectorLine> perimeterLines)
        {
            while (true)
            {
                foreach (var axis in axes)
                {
                    axis.points3[2] = axis.points3[2].normalized * _camera.orthographicSize * _axesMagnitudeMultiplier;
                    axis.Draw3D();
                }

                var angle = Vector3.Angle(_camera.transform.position, new Vector3(_camera.transform.position.x, 0f, _camera.transform.position.z));

                float alpha = Mathf.Min(Mathf.Max(angle - _invisibleAngle, 0f) / _visibleAngleOffset, 1f);

                for (int i = 0; i < lines.Count; i++)
                    lines[i].color = new Color(_lineColor.r, _lineColor.g, _lineColor.b, _lineColor.a * alpha);

                for (int i = 0; i < majorLines.Count; i++)
                    majorLines[i].color = new Color(_majorLineColor.r, _majorLineColor.g, _majorLineColor.b, _majorLineColor.a * alpha);

                for (int i = 0; i < perimeterLines.Count; i++)
                    perimeterLines[i].color = new Color(_perimeterColor.r, _perimeterColor.g, _perimeterColor.b, _perimeterColor.a * alpha);

                yield return null;
            }
        }

        private VectorLine CreateAxis(string name, Color color, Vector3 vector)
        {
            var line = VectorLine.SetLine3D(color, Vector3.zero, Vector3.zero, vector);
            line.name = name;
            line.rectTransform.SetParent(_axes);
            line.smoothColor = true;
            line.SetColors(new List<Color32>() { color, new Color(color.r, color.g, color.b, 0f) });
            line.lineWidth = _linesWidth;

            return line;
        }

        private VectorLine CreateLine(string name, Transform parent, Color color, Vector3 pointA, Vector3 pointB)
        {
            var line = VectorLine.SetLine3D(color, pointA, pointB);
            line.name = name;
            line.lineWidth = _linesWidth;
            line.rectTransform.SetParent(parent);

            return line;
        }

        private VectorLine CreatePerimeterLine(Color color, float x1, float z1, float x2, float z2)
        {
            return CreateLine("PerimeterLine", _perimeter, color, new Vector3(x1, 0f, z1), new Vector3(x2, 0f, z2));
        }
    }
}