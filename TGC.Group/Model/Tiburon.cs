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
        private GameModel gmodel;
        private TgcMesh mesh;
        private float velocidad = 1f;
        private int tramo = 0;
        private float anguloRebote = FastMath.PI / 4;
        private float radioDeteccion = 500;

        public Tiburon(TgcMesh mesh, GameModel gmodel)
        {
            this.mesh = mesh;
            this.gmodel = gmodel;
        }

        public Boolean tocoBorde() {
            return Position.X >= 20000 || Position.X <= -20000 || Position.Z >= 20000 || Position.Z <= -20000;
        }

        private Boolean esPrimerTramo()
        {
            return tramo.Equals(0);
        }

        private Boolean esSegundoTramo()
        {
            return tramo.Equals(1);
        }

        private Boolean esTercerTramo()
        {
            return tramo.Equals(2);
        }

        private Boolean esCuartoTramo()
        {
            return tramo.Equals(3);
        }

        public Boolean estoyCercaDelPersonaje(Personaje personaje)
        {
            return FastMath.Sqrt(FastMath.Pow2(personaje.Position.X - this.Position.X) + FastMath.Pow2(personaje.Position.Z - this.Position.Z)) < radioDeteccion;
        }

        public void moverse(Escenario escenario)
        {
            if (!estoyCercaDelPersonaje(gmodel.GetPersonaje))
            {
                if (esPrimerTramo())
                {
                    var dir = new TGCVector3(1, 0, 1);
                    dir.Multiply(velocidad);
                    mesh.Move(dir);
                }
                else if (esSegundoTramo())
                {
                    var dir = new TGCVector3(1, 0, -1);
                    dir.Multiply(velocidad);
                    mesh.Move(dir);
                }
                else if (esTercerTramo())
                {
                    var dir = new TGCVector3(-1, 0, -1);
                    dir.Multiply(velocidad);
                    mesh.Move(dir);
                }
                else if (esCuartoTramo())
                {
                    var dir = new TGCVector3(-1, 0, 1);
                    dir.Multiply(velocidad);
                    mesh.Move(dir);
                }

                if (tocoBorde())
                {
                    RotateY(FastMath.PI / 2);
                    tramo = (tramo + 1) % 4;
                }
            }
            else //LO PERSIGO
            {
                //
            }            
        }

        public void Render()
        {
            this.mesh.Render();
        }

        public void Dispose()
        {
            this.mesh.Dispose();
        }

        public TGCMatrix Transform { get => mesh.Transform; set => mesh.Transform = value; }
        public TGCVector3 Position { get => mesh.Position; set => mesh.Position = value; }
        public float Velocidad { get => velocidad; set => velocidad = value; }
        public void RotateX(float angle)
        {
            mesh.RotateX(angle);
        }

        public void RotateY(float angle)
        {
            mesh.RotateY(angle);
        }

        public void RotateZ(float angle)
        {
            mesh.RotateZ(angle);
        }

        public void Move(TGCVector3 vector)
        {
            mesh.Move(vector);
        }
    }
}
