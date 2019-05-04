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
        private float velocidad = 100f;
        private int tramo = 0;
        private float anguloRebote = FastMath.PI / 4;
        private float radioDeteccion = 500;
        private Boolean primeraRotacion = true;
        private TGCVector3 movDir = new TGCVector3(0,0,0);
        private TGCVector3 versorDir = new TGCVector3(0, 0, 0);
        private float anguloAnterior = 0;

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
                primeraRotacion = true;

                if (esPrimerTramo())
                {
                    movDir = new TGCVector3(1, 0, 1);
                    versorDir = new TGCVector3(movDir);
                    movDir.Multiply(velocidad * gmodel.ElapsedTime);
                    mesh.Move(movDir);
                }
                else if (esSegundoTramo())
                {
                    movDir = new TGCVector3(1, 0, -1);
                    versorDir = new TGCVector3(movDir);
                    movDir.Multiply(velocidad * gmodel.ElapsedTime);
                    mesh.Move(movDir);
                }
                else if (esTercerTramo())
                {
                    movDir = new TGCVector3(-1, 0, -1);
                    versorDir = new TGCVector3(movDir);
                    movDir.Multiply(velocidad * gmodel.ElapsedTime);
                    mesh.Move(movDir);
                }
                else if (esCuartoTramo())
                {
                    movDir = new TGCVector3(-1, 0, 1);
                    versorDir = new TGCVector3(movDir);
                    movDir.Multiply(velocidad * gmodel.ElapsedTime);
                    mesh.Move(movDir);
                }

                if (tocoBorde())
                {
                    RotateY(FastMath.PI / 2);
                    tramo = (tramo + 1) % 4;
                }
            }
            else //LO PERSIGO
            {
                //FUNCIONA SOLO SI LO PERSIGO DESDE ATRAS A LA DERECHA... 
                //SEPARAR CASOS SEGUN POSICION??? SON 4 CASOS, UNO POR CUADRANTE
                if (versorDir.Equals(new TGCVector3(1, 0, 1)))
                {
                    //PRIMER TRAMO
                    if (true)
                    {

                    }
                }
                else if (versorDir.Equals(new TGCVector3(1, 0, -1)))
                {
                    //SEGUNDO TRAMO
                    if (true)
                    {

                    }
                }
                else if (versorDir.Equals(new TGCVector3(-1, 0, -1)))
                {
                    //TERCER TRAMO
                    if (true)
                    {

                    }
                }
                else if (versorDir.Equals(new TGCVector3(-1, 0, 1)))
                {
                    //CUARTO TRAMO
                    if (true)
                    {

                    }
                }

                //SI RECIEN LO DESCUBRO TENGO QUE GIRAR UN ANGULO ADICIONAL
                if (primeraRotacion)
                {
                    var primerAngulo = FastMath.Atan(movDir.Z / movDir.X);
                    var x = Position.X - gmodel.GetPersonaje.Position.X;
                    var z = gmodel.GetPersonaje.Position.Z - Position.Z;                    
                    var segundoAngulo = FastMath.Atan(z / x) + FastMath.PI;

                    RotateY(primerAngulo + segundoAngulo);
                }
                primeraRotacion = false;

                //SINO TENGO QUE ROTAR LA DIFERENCIA UNICAMENTE
                //HACER

                //FINALMENTE ME MUEVO
                var dir = gmodel.GetPersonaje.Position;
                dir.Normalize();
                dir.Multiply(-1f);
                dir.Multiply(velocidad * gmodel.ElapsedTime);
                
                mesh.Move(dir);
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

        public Boolean estaAtrasDerecha(Personaje personaje)
        {
            return Position.X > personaje.Position.X
                && Position.Z < personaje.Position.Z;
        }

        public Boolean estaAdelanteDerecha(Personaje personaje)
        {
            return Position.X < personaje.Position.X
                && Position.Z < personaje.Position.Z;
        }

        public Boolean estaAtrasIzquierda(Personaje personaje)
        {
            return Position.X > personaje.Position.X
                && Position.Z > personaje.Position.Z;
        }

        public Boolean estaAdelanteIzquierda(Personaje personaje)
        {
            return Position.X < personaje.Position.X
                && Position.Z > personaje.Position.Z;
        }
    }
}
