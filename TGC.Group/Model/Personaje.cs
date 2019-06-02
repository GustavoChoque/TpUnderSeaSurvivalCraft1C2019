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
        private Inventario inventario;
        private float health;
        private float oxygen;
        private float maxHealth;
        private float maxOxygen;
        private Boolean vivo = true;
        private float radioDeteccionNave = 450f;
        private float radioDeteccionWorkbench = 250f;
        private bool usoArma = false;
        private bool usoRedPesca = false;

        public Personaje(float health, float oxygen)
        {
            this.health = health;
            this.oxygen = oxygen;
            maxHealth = health;
            maxOxygen = oxygen;
        }

        public void aumentoOxigeno(float newMaxOxygen)
        {
            if (newMaxOxygen > maxOxygen)
            {
                maxOxygen = newMaxOxygen;
                oxygen = newMaxOxygen;
            }
        }

        public void aumentoSalud(float newMaxHealth)
        {
            if (newMaxHealth > maxHealth)
            {
                maxHealth = newMaxHealth;
                health = newMaxHealth;
            }
        }

        public void Init(GameModel gmodel)
        {
            this.gmodel = gmodel;
            inventario = new Inventario();
            //Para probar +100 oxigeno y +100 salud
            Inventario.agregaObjeto(new BrainCoral());
            Inventario.agregaObjeto(new BrainCoral());
            Inventario.agregaObjeto(new BrainCoral());
            Inventario.agregaObjeto(new BrainCoral());
            Inventario.agregaObjeto(new BrainCoral());
            Inventario.agregaObjeto(new SeaShell());
            Inventario.agregaObjeto(new SeaShell());
            Inventario.agregaObjeto(new SeaShell());
            Inventario.agregaObjeto(new SeaShell());
            //Para probar arma
            Inventario.agregaObjeto(new ObjetoInventario("Oro", 3));
            //Para probar red de pesca
            Inventario.agregaObjeto(new ObjetoInventario("Platino", 4));
        }

        public void Update()
        {
            var Input = this.gmodel.Input;

            //-----------crafteo------------
            if (workbenchCerca(gmodel.Escenario.Workbench) && Input.keyPressed(Key.C) && !gmodel.InterfazInventario.Activo)
            {
                gmodel.InterfazCrafting.activar();
            }

            //inventario
            if (Input.keyPressed(Key.I) && !gmodel.InterfazCrafting.Activo)
            {
                gmodel.InterfazInventario.activar();
            }
        }
        public void Render()
        {
            
        }

        public void Dispose()
        {

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
            if ((this.health + vidaRecuperada) > maxHealth)
            {
                this.health = maxHealth;
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
            if ((this.oxygen + oxigenoRecuperado) > maxOxygen)
            {
                this.oxygen = maxOxygen;
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
        public float MaxHealth { get => maxHealth; }
        public float MaxOxygen { get => maxOxygen; }
        public bool UsoArma { get => usoArma; set => usoArma = value; }
        public bool UsoRedPesca { get => usoRedPesca; set => usoRedPesca = value; }
    }
}
