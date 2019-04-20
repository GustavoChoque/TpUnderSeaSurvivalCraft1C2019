using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Direct3D;
using TGC.Core.Example;
using TGC.Core.Geometry;
using TGC.Core.Input;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;
using TGC.Core.Textures;
using TGC.Group.Model.Camara;
using TGC.Core.Terrain;
using TGC.Core.Sound;

namespace TGC.Group.Model
{
    class Pez
    {
        private TgcMesh meshPez;
        private float currentMoveDir;
        
        public Pez(TgcMesh meshOriginal, float currentMoveDir)
        {
            meshPez = meshOriginal;
            this.currentMoveDir = currentMoveDir;
        }        

        public void Render()
        {
            meshPez.Render();
        }

        public void Dispose()
        {
            meshPez.Dispose();
        }

        public float CurrentMoveDir { get => currentMoveDir; set => currentMoveDir = value; }
        public TGCVector3 Position { get => meshPez.Position; set => meshPez.Position = value; }
        public TGCVector3 Rotation { get => meshPez.Rotation; set => meshPez.Rotation = value; }
        public TGCMatrix Transform { get => meshPez.Transform; set => meshPez.Transform = value; }
        public TGCVector3 Scale { get => meshPez.Scale; set => meshPez.Scale = value; }
    }
}
