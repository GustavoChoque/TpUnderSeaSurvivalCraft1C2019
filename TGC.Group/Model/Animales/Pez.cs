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
using LosTiburones.Model;

namespace TGC.Group.Model
{
    public class Pez
    {
        private TgcMesh meshPez;
        private float currentMoveDir;
        private float moveSpeed;
        private MovimientoPez movimiento;
        private float radioCercaniaPersonaje = 200f;


        public Pez(TgcMesh meshOriginal, float currentMoveDir, float moveSpeed)
        {
            meshPez = meshOriginal;
            this.currentMoveDir = currentMoveDir;
            this.moveSpeed = moveSpeed;
        }

        //Para crear un pez es necesario definirle un mesh, la posicion inicial, la rotacion inicial y el movimiento que tendra
        public Pez(TgcMesh meshPezPar, TGCVector3 posicion, TGCVector3 rotacion, MovimientoPez movimientoPar)
        {
            meshPez = meshPezPar;
            meshPez.Rotation = rotacion;
            meshPez.Transform = TGCMatrix.RotationYawPitchRoll(meshPez.Rotation.Y, meshPez.Rotation.X, meshPez.Rotation.Z) * TGCMatrix.Translation(posicion.X, posicion.Y, posicion.Z);
            this.Movimiento = movimientoPar;
        }

        //El pez delega en el objeto movimiento el como moverse
        public void mover(float ElapsedTime)
        {
            this.Movimiento.mover(this.meshPez, ElapsedTime);
        }

        //Por el momento lo unico que puede hacer un pez es moverse
        public void Update(float ElapsedTime)
        {
            this.mover(ElapsedTime);
        }

        public void Render()
        {
            meshPez.Render();
        }

        public void Dispose()
        {
            meshPez.Dispose();
        }

        public Boolean estoyCercaDePersonaje(Personaje personaje)
        {
            return FastMath.Sqrt(TGCVector3.LengthSq(personaje.Position - this.Position)) < radioCercaniaPersonaje;
        }

        public void disable()
        {
            meshPez.Enabled = false;
        }

        public float CurrentMoveDir { get => currentMoveDir; set => currentMoveDir = value; }
        public float MoveSpeed { get => moveSpeed; set => moveSpeed = value; }
        public MovimientoPez Movimiento { get => movimiento; set => movimiento = value; }
        public TGCVector3 Position { get => meshPez.Position; set => meshPez.Position = value; }
        public TGCVector3 Rotation { get => meshPez.Rotation; set => meshPez.Rotation = value; }
        public TGCMatrix Transform { get => meshPez.Transform; set => meshPez.Transform = value; }
        public TGCVector3 Scale { get => meshPez.Scale; set => meshPez.Scale = value; }
    }
}
