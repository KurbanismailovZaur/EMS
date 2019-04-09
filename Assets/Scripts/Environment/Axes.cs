using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using static UnityEngine.Debug;
using Vectrosity;

namespace Environment
{
	public class Axes : MonoBehaviour 
	{
        [SerializeField]
        private Camera _camera;

        [SerializeField]
        private Color _xColor;

        [SerializeField]
        private Color _yColor;

        [SerializeField]
        private Color _zColor;

        [SerializeField]
        [Range(1f, 8f)]
        private float _axesMagnitudeMultiplier = 1f;

        [SerializeField]
        [Range(1f, 4f)]
        private float _linesWidth = 1f;

        private IEnumerator Start()
        {
            var xLine = CreateExis("xAxis", _xColor, Vector3.right);
            var yLine =  CreateExis("yAxis", _yColor, Vector3.up);
            var zLine = CreateExis("zAxis", _zColor, Vector3.forward);

            var lines = new List<VectorLine>() { xLine, yLine, zLine };

            while (true)
            {
                foreach (var line in lines)
                {
                    line.points3[2] = line.points3[2].normalized * _camera.orthographicSize * _axesMagnitudeMultiplier;
                    line.Draw3D();
                }

                yield return null;
            }
        }

        private VectorLine CreateExis(string name, Color color, Vector3 vector)
        {
            var line = VectorLine.SetLine3D(color, Vector3.zero, Vector3.zero, vector);
            line.name = name;
            line.rectTransform.SetParent(transform);
            line.smoothColor = true;
            line.SetColors(new List<Color32>() { color, new Color(color.r, color.g, color.b, 0f) });
            line.lineWidth = _linesWidth;

            return line;
        }
    }
}