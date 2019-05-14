using BulletSharp;
using BulletSharp.Math;
using System.Drawing;
using TGC.Core.Geometry;
using TGC.Core.Mathematica;

namespace LosTiburones.Model
{
    class ADebugDrawer : DebugDraw
    {
        public ADebugDrawer(DebugDrawModes debugModePar)
        {
            DebugMode = debugModePar;
        }

        private DebugDrawModes debugMode;

        public override DebugDrawModes DebugMode { get => debugMode; set => debugMode = value; }

        public override void Draw3DText(ref Vector3 location, string textString)
        {

        }

        public override void DrawLine(ref Vector3 from, ref Vector3 to, ref Vector3 color)
        {
            var line = TgcLine.fromExtremes(new TGCVector3(from.X, from.Y, from.Z), new TGCVector3(to.X, to.Y, to.Z), Color.Red);
            line.Render();
        }

        public override void ReportErrorWarning(string warningString)
        {

        }
    }
}
