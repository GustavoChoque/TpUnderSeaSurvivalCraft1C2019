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
        private float radioDeteccion = 500;
        private TGCVector3 movDir = new TGCVector3(1,0,1);
        float tiempoCambioRumbo = 10;
        float contadorTiempo = 0;

        public Tiburon(TgcMesh mesh, GameModel gmodel)
        {
            this.mesh = mesh;
            this.gmodel = gmodel;
            movDir.Normalize();
        }

        public Boolean tocoBorde() {
            return Position.X >= 20000 || Position.X <= -20000 || Position.Z >= 20000 || Position.Z <= -20000;
        }

        public Boolean estoyCercaDelPersonaje(Personaje personaje)
        {
            return FastMath.Sqrt(FastMath.Pow2(personaje.Position.X - this.Position.X) + FastMath.Pow2(personaje.Position.Z - this.Position.Z)) < radioDeteccion;
        }

        public void moverse(Escenario escenario)
        {
            contadorTiempo = contadorTiempo + gmodel.ElapsedTime;

            if (!estoyCercaDelPersonaje(gmodel.GetPersonaje))
            {
                if (contadorTiempo < tiempoCambioRumbo)
                {
                    var dirPosta = new TGCVector3(movDir);
                    dirPosta.Multiply(gmodel.ElapsedTime);
                    dirPosta.Multiply(Velocidad);
                    mesh.Move(dirPosta);
                }
                else
                {
                    contadorTiempo = 0;

                    float x = (float) gmodel.GetRandom.NextDouble();
                    float y = 0;
                    float z = (float) gmodel.GetRandom.NextDouble();

                    var nuevaDir = new TGCVector3(x, y, z);
                    nuevaDir.Normalize();

                    var thetaViejaDir = FastMath.Atan(movDir.Z / movDir.X);
                    var thetaNuevaDir = FastMath.Atan(nuevaDir.Z / nuevaDir.X);

                    float anguloEntreDosVectores = thetaViejaDir - thetaNuevaDir;
                    if (anguloEntreDosVectores < 0)
                    {
                        RotateY(2 * FastMath.PI + anguloEntreDosVectores);
                    }
                    else
                    {
                        RotateY(anguloEntreDosVectores);
                    }

                    movDir = new TGCVector3(nuevaDir);
                    var dirPosta = new TGCVector3(movDir);
                    dirPosta.Multiply(gmodel.ElapsedTime);
                    dirPosta.Multiply(Velocidad);
                    mesh.Move(dirPosta);                    
                }
            }
            else
            {
                //LO PERSIGO
            }


                /*
                 * primeraRotacion = true;
                var vectorAnterior = new TGCVector3(movDir);

                if (esPrimerTramo())
                {
                    movDir = new TGCVector3(1, 0, 1);

                    if (vuelvoDePerseguir)
                    {
                        float formula = (movDir.X * vectorAnterior.X + movDir.Z * vectorAnterior.Z) / (movDir.Length() * vectorAnterior.Length());
                        float anguloEntreDosVectores = FastMath.Acos(formula);
                        RotateY(2*FastMath.PI - anguloEntreDosVectores);
                        vuelvoDePerseguir = false;
                    }

                    movDir.Multiply(velocidad * gmodel.ElapsedTime);
                    mesh.Move(movDir);
                }
                else if (esSegundoTramo())
                {
                    movDir = new TGCVector3(1, 0, -1);

                    if (vuelvoDePerseguir)
                    {
                        float formula = (movDir.X * vectorAnterior.X + movDir.Z * vectorAnterior.Z) / (movDir.Length() * vectorAnterior.Length());
                        float anguloEntreDosVectores = FastMath.Acos(formula);
                        RotateY(FastMath.PI / 2 + anguloEntreDosVectores);
                        vuelvoDePerseguir = false;
                    }

                    movDir.Multiply(velocidad * gmodel.ElapsedTime);
                    mesh.Move(movDir);
                }
                else if (esTercerTramo())
                {
                    movDir = new TGCVector3(-1, 0, -1);

                    if (vuelvoDePerseguir)
                    {
                        float formula = (movDir.X * vectorAnterior.X + movDir.Z * vectorAnterior.Z) / (movDir.Length() * vectorAnterior.Length());
                        float anguloEntreDosVectores = FastMath.Acos(formula);
                        RotateY(FastMath.PI / 2 + anguloEntreDosVectores);
                        vuelvoDePerseguir = false;
                    }

                    movDir.Multiply(velocidad * gmodel.ElapsedTime);
                    mesh.Move(movDir);
                }
                else if (esCuartoTramo())
                {
                    movDir = new TGCVector3(-1, 0, 1);

                    if (vuelvoDePerseguir)
                    {
                        float formula = (movDir.X * vectorAnterior.X + movDir.Z * vectorAnterior.Z) / (movDir.Length() * vectorAnterior.Length());
                        float anguloEntreDosVectores = FastMath.Acos(formula);
                        RotateY(FastMath.PI / 2 + anguloEntreDosVectores);
                        vuelvoDePerseguir = false;
                    }

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
                if (primeraRotacion)
                {
                    var i = new TGCVector2(Position.X, gmodel.GetPersonaje.Position.Z);
                    var j = new TGCVector2(gmodel.GetPersonaje.Position.X, gmodel.GetPersonaje.Position.Z);
                    var a = new TGCVector2(Position.X, Position.Z);

                    var z = new TGCVector2(i);
                    z.Subtract(a);

                    var x = new TGCVector2(j);
                    x.Subtract(a);

                    var v = new TGCVector2(x);
                    v.Add(z);

                    float formula = (movDir.X * v.X + movDir.Z * v.Y)/(movDir.Length() * v.Length());
                    float anguloEntreDosVectores = FastMath.Acos(formula);

                    RotateY(anguloEntreDosVectores);
                }
                primeraRotacion = false;

                //FINALMENTE LO PERSIGO
                var dir = gmodel.GetPersonaje.Position;
                dir.Normalize();
                dir.Multiply(-1f);
                dir.Multiply(velocidad * gmodel.ElapsedTime);

                mesh.Move(dir);
                movDir = new TGCVector3(dir);
                vuelvoDePerseguir = true;


                /*
                
                
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
            */
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
