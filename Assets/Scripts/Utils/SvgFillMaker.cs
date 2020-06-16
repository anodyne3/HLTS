using System.Collections.Generic;
using Core;
using Unity.VectorGraphics;
using UnityEngine;

namespace Utils
{
    [ExecuteInEditMode]
    public class SvgFillMaker : GlobalAccess
    {
        private Scene _mScene;
        private VectorUtils.TessellationOptions _mOptions;
        public float svgFillAmount;
        public Color fillColor;
        public Vector2[] bezierPoints = new Vector2[10];

        private void MakeShape()
        {
            var shape = new Shape
            {
                Contours = new[]
                {
                    new BezierContour
                    {
                        Segments = new[]
                        {
                            new BezierPathSegment
                            {
                                P0 = bezierPoints[0],
                                P1 = bezierPoints[1],
                                P2 = bezierPoints[2]
                            },
                            new BezierPathSegment
                            {
                                P0 = bezierPoints[3],
                                P1 = bezierPoints[4],
                                P2 = bezierPoints[5]
                            },
                            new BezierPathSegment
                            {
                                P0 = bezierPoints[6],
                                P1 = bezierPoints[7],
                                P2 = bezierPoints[8]
                            },
                            new BezierPathSegment
                            {
                                P0 = bezierPoints[9]
                            }
                        }
                    }
                },
                Fill = new SolidFill
                {
                    Color = fillColor
                }
            };

            var viewPort = new Shape
            {
                Contours = new[]
                {
                    new BezierContour
                    {
                        Segments = new []
                        {
                            new BezierPathSegment(),
                            new BezierPathSegment
                            {
                                P0 = new Vector2(0,1),
                                P1 = new Vector2(0, 1),
                                P2 = new Vector2( 1, 1)
                            },
                            new BezierPathSegment
                            {
                                P0 = new Vector2(1,1),
                                P1 = new Vector2(1, 1),
                                P2 = new Vector2( 1, 0)
                            },
                            new BezierPathSegment
                            {
                                P0 = new Vector2(1,0),
                            }
                        },
                        Closed = true
                    }
                },
                Fill = new SolidFill()
            };

            _mScene = new Scene
            {
                Root = new SceneNode {Shapes = new List<Shape> {shape, viewPort}}
            };

            _mOptions = new VectorUtils.TessellationOptions
            {
                StepDistance = 1000.0f,
                MaxCordDeviation = 0.05f,
                MaxTanAngleDeviation = 0.05f,
                SamplingStepSize = 0.1f
            };
        }

        public VectorUtils.Alignment alignment = VectorUtils.Alignment.Custom;

        public Sprite ShowShape()
        {
            MakeShape();

            var geometry = VectorUtils.TessellateScene(_mScene, _mOptions);

            return VectorUtils.BuildSprite(geometry, 100.0f, alignment, Vector2.zero,  128);
        }
    }
}