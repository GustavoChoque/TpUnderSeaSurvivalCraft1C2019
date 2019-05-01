using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;
using TGC.Group.Model;

namespace LosTiburones.Model
{
    class Tiburon
    {
        private TGCVector3 posInicial;
        private TgcMesh mesh;
        private float currentMoveDir = -1f;

        public Tiburon(TGCVector3 posInicial, TgcMesh mesh)
        {
            this.posInicial = posInicial;
            this.mesh = mesh;
            this.mesh.Position = posInicial;
        }

        public void moverse(GameModel model)
        {
            Position += new TGCVector3(model.MovementSpeed * model.ElapsedTime * currentMoveDir, 0, 0);

            if (!((posInicial.X + 500 > Position.X) && (posInicial.X - 500 < Position.X)))
            {
                currentMoveDir *= -1;
                Rotation += new TGCVector3(0, FastMath.PI, 0);
            }

            Transform = TGCMatrix.RotationYawPitchRoll(Rotation.X, Rotation.Y, Rotation.Z) * TGCMatrix.Translation(Position);
        }

        public void Render()
        {
            this.mesh.Render();
        }

        public void Dispose()
        {
            this.mesh.Dispose();
        }

		public TGCVector3 Rotation { get => mesh.Rotation; set => mesh.Rotation = value; }
        public TGCMatrix Transform { get => mesh.Transform; set => mesh.Transform = value; }
        public TGCVector3 Position { get => mesh.Position; set => mesh.Position = value; }
        public float CurrentMoveDir { get => currentMoveDir; set => currentMoveDir = value; }
    }
}
