using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Mathematica;
using Microsoft.DirectX.DirectInput;
using TGC.Core.SceneLoader;
using LosTiburones.Model;
using TGC.Core.Geometry;

namespace TGC.Group.Model
{
    //JUGADOR COMIENZA VIVO, CON ENERGIA Y OXIGENO AL 100
    //MUERE CUANDO VIDA U OXIGENO SON MENORES A 1
    //PUEDE MORIR O REVIVIR, SUFRIR DANIO, PERDER OXIGENO, ASI COMO RECUPERAR ENERGIA Y OXIGENO
    public class Personaje
    {
        GameModel GModel;
        TgcMesh mesh;
        TGCVector3 posicion;
        public List<ObjetoDeInventario> inventario;

       
        private float health;
        private float oxygen;
        private Boolean vivo = true;
        
        public Personaje(float health, float oxygen)
        {
            this.health = health;
            this.oxygen = oxygen;
        }

        public void Init(GameModel gmodel)
        {
            this.GModel = gmodel;
            this.posicion = this.GModel.Camara.Position;
            this.inventario = new List<ObjetoDeInventario>();
            var brainCoral = new BrainCoral();
            inventario.Add(brainCoral);
            var seaShell = new SeaShell();
            inventario.Add(seaShell);
            
        }

        public void Update()
        {
            posicion = GModel.Camara.Position;
            

            var Input = this.GModel.Input;
            this.GModel.objetosEstaticosEnArray.ForEach(objetoRecolectable =>
            {
                if (objetoCerca(objetoRecolectable))
                {
                    if (objetoRecolectable.Name.StartsWith("brain") && Input.keyPressed(Key.E))
                    {
                        objetoRecolectable.Enabled = false;
                        objetoRecolectable.Position = new TGCVector3(objetoRecolectable.Position.X, 1000, objetoRecolectable.Position.Z);
                        //despues sino buscar talvez que, que me lo encuentre por el nombre o dejarlo asi por index 
                        inventario[0].cantidad++;
                    }
                    if (objetoRecolectable.Name.StartsWith("sea") && Input.keyPressed(Key.E))
                    {
                        objetoRecolectable.Enabled = false;
                        objetoRecolectable.Position = new TGCVector3(objetoRecolectable.Position.X, 1000, objetoRecolectable.Position.Z);
                        inventario[1].cantidad++;

                    }


                    //este no lo agarra
                    if (objetoRecolectable.Name.StartsWith("tree") && Input.keyPressed(Key.E))
                    {
                        objetoRecolectable.Enabled = false;
                        objetoRecolectable.Position = new TGCVector3(objetoRecolectable.Position.X, 1000, objetoRecolectable.Position.Z);
                    }

                    //este no lo agarra
                    if (objetoRecolectable.Name.StartsWith("pillar") && Input.keyPressed(Key.E))
                    {
                        objetoRecolectable.Enabled = false;
                        objetoRecolectable.Position = new TGCVector3(objetoRecolectable.Position.X, 1000, objetoRecolectable.Position.Z);
                    }
                    //este no lo agarra
                    if (objetoRecolectable.Name.StartsWith("coral") && Input.keyPressed(Key.E))
                    {
                        objetoRecolectable.Enabled = false;
                        objetoRecolectable.Position = new TGCVector3(objetoRecolectable.Position.X, 1000, objetoRecolectable.Position.Z);
                    }
                    if (objetoRecolectable.Name.StartsWith("spiral") && Input.keyPressed(Key.E))
                    {
                        objetoRecolectable.Enabled = false;
                        objetoRecolectable.Position = new TGCVector3(objetoRecolectable.Position.X, 1000, objetoRecolectable.Position.Z);
                    }
                }
            });

            this.GModel.Metales.ForEach(metal =>
            {
                if (objetoCerca(metal))
                {
                    if (Input.keyPressed(Key.E))
                    {
                        metal.Enabled = false;
                        //metal.Position = new TGCVector3(metal.Position.X, 1000, metal.Position.Z);
                        //despues sino buscar talvez que, que me lo encuentre por el nombre o dejarlo asi por index 
                        //inventario[2].cantidad++;
                    }
                }




            });
                
            /*
                if (objetoCerca(metal))
            {
                if (metal.Name.StartsWith("brain") && Input.keyPressed(Key.E))
                {
                    objetoRecolectable.Enabled = false;
                    objetoRecolectable.Enabled = false;
                    objetoRecolectable.Position = new TGCVector3(objetoRecolectable.Position.X, 1000, objetoRecolectable.Position.Z);
                    //despues sino buscar talvez que, que me lo encuentre por el nombre o dejarlo asi por index 
                    inventario[0].cantidad++;
                }

    */

            }
        public void Render()
        {

           
        }



        public bool objetoCerca(TgcMesh objeto)
        {
            var distanciaRecoleccion = 1000 * 8;//fui probando distintos numeros
            return TGCVector3.LengthSq(objeto.Position - this.posicion) < distanciaRecoleccion;
        }

        public bool objetoCerca(TGCBox objeto)
        {
            var distanciaRecoleccion = 1000 * 8;//fui probando distintos numeros
            return TGCVector3.LengthSq(objeto.Position - this.posicion) < distanciaRecoleccion;
        }


        public void sufriDanio(float danio)
        {
            this.health = this.health - danio;
            if (this.Health <= 0) this.morite();
        }

        public void morite()
        {
            this.vivo = false;
        }

        public void revivi()
        {
            this.vivo = true;
        }

        public void recuperaVida(float vidaRecuperada)
        {
            if ((this.health + vidaRecuperada) > 100)
            {
                this.health = 100;
            }
            else
            {
                this.health = this.health + vidaRecuperada;
            }
        }

        public void perdeOxigeno(float oxigenoPerdido)
        {
            this.oxygen = this.oxygen - oxigenoPerdido;
            if (this.Oxygen <= 1) this.morite();
        }

        public void recuperaOxigeno(float oxigenoRecuperado)
        {
            if ((this.oxygen + oxigenoRecuperado) > 100)
            {
                this.oxygen = 100;
            }
            else
            {
                this.oxygen = this.oxygen + oxigenoRecuperado;
            }
        }

        public float Health { get => health; }
        public float Oxygen { get => oxygen; }
        public Boolean Vivo { get => vivo; }
    }
}
