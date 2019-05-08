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
        private GameModel gmodel;
        //TgcMesh mesh;
        //public List<InterfazRecolectable> inventario;   
        private Inventario inventario;
        private float health;
        private float oxygen;
        private Boolean vivo = true;
        private float radioDeteccionNave = 450f;
        private float radioDeteccionWorkbench = 250f;

        public Personaje(float health, float oxygen)
        {
            this.health = health;
            this.oxygen = oxygen;
        }

        public void Init(GameModel gmodel)
        {
            this.gmodel = gmodel;
            inventario = new Inventario(gmodel, this);
            Inventario.agregaObjeto(new ObjetoInventario("BrainCoral", 4));
            Inventario.agregaObjeto(new ObjetoInventario("SeaShell", 1));
        }

        public void Update()
        {
            var Input = this.gmodel.Input;
            
            inventario.Update();
            //-----------crafteo------------
            if (workbenchCerca(gmodel.Escenario.Workbench) && Input.keyPressed(Key.C))
            {
                gmodel.InterfazCrafting.activar();
            }
        }
        public void Render()
        {
            inventario.Render();           
        }

        public void Dispose()
        {
            inventario.Dispose();
        }

        private Boolean workbenchCerca(TgcMesh workbench)
        {            
            return FastMath.Sqrt(TGCVector3.LengthSq(workbench.Position - Position)) < radioDeteccionWorkbench;
        }

        private Boolean cercaDeNave(TgcScene nave)
        {
            return FastMath.Sqrt(TGCVector3.LengthSq(nave.Meshes[4].Position - Position)) < radioDeteccionNave;
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
        public TGCVector3 Position { get => gmodel.Camara.Position; }
        public Inventario Inventario { get => inventario; }

        public Boolean estaCercaDeNave()
        {
            return cercaDeNave(gmodel.Escenario.Barco);
        }

    }
}
