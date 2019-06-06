using BulletSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;

namespace LosTiburones.Model.CraftingInventario
{
    public class Arpon
    {
        private TgcMesh meshArpon;
        private RigidBody rigidBodyArpon;

        public Arpon(TgcMesh meshArpon, RigidBody rigidBodyArpon, TGCVector3 posicion)
        {
            this.meshArpon = meshArpon;
            this.rigidBodyArpon = rigidBodyArpon;
            this.meshArpon.Position = posicion;
        }

        public void Dispose()
        {
            this.meshArpon.Dispose();
            this.rigidBodyArpon.Dispose();
        }

        public RigidBody RigidBody { get => rigidBodyArpon; }

        public TgcMesh Mesh { get => meshArpon; }

        public TGCVector3 Position { get => meshArpon.Position; set => meshArpon.Position = value; }

        public void Update()
        {
            Position = new TGCVector3(RigidBody.CenterOfMassPosition.X, RigidBody.CenterOfMassPosition.Y, RigidBody.CenterOfMassPosition.Z);
            Mesh.Scale = new TGCVector3(1f,1f,1f);
            Mesh.Transform = TGCMatrix.Scaling(Mesh.Scale) * TGCMatrix.Translation(Mesh.Position);
        }
    }
}
