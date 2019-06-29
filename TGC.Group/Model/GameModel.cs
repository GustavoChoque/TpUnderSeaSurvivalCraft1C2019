using Microsoft.DirectX.DirectInput;
using System.Drawing;
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
using System.Collections.Generic;
using System;
using TGC.Group.Model.Sprites;
using LosTiburones;
using System.Linq;
using LosTiburones.Model;
using Microsoft.DirectX.Direct3D;
using TGC.Core.BoundingVolumes;
using System.Windows.Forms;
using TGC.Group.Model.Menu;
using Microsoft.DirectX;

namespace TGC.Group.Model
{
    /// <summary>
    ///     Ejemplo para implementar el TP.
    ///     Inicialmente puede ser renombrado o copiado para hacer más ejemplos chicos, en el caso de copiar para que se
    ///     ejecute el nuevo ejemplo deben cambiar el modelo que instancia GameForm <see cref="Form.GameForm.InitGraphics()" />
    ///     line 97.
    /// </summary>
    public class GameModel : TgcExample
    {
        /// <summary>
        ///     Constructor del juego.
        /// </summary>
        /// <param name="mediaDir">Ruta donde esta la carpeta con los assets</param>
        /// <param name="shadersDir">Ruta donde esta la carpeta con los shaders</param>
        public GameModel(string mediaDir, string shadersDir) : base(mediaDir, shadersDir)
        {
            Category = Game.Default.Category;
            Name = Game.Default.Name;
            Description = Game.Default.Description;
        }
        
        //DECLARO VARIABLES 'GLOBALES'
        private TgcFpsCamera camaraInterna;
        private TgcBoundingCylinder cilindroColision;
        private float leftrightRot;
        private float updownRot;

        //constantes de la camara
        private const float camaraMoveSpeed = 400f;
        private const float camaraJumpSpeed = 80f;

        private Personaje personaje = new Personaje(100, 100);

        private Escenario escenario = new Escenario();
        private InterfazDeCrafting ic = new InterfazDeCrafting();
        private InterfazDeInventario ii = new InterfazDeInventario();

        private Random rnd = new Random();

        private Drawer2D spriteDrawer = new Drawer2D();
        private CustomBitmap bitmapTitulo;
        private CustomSprite spriteTitulo = new CustomSprite();

        //-----para el menu------------
        public bool partidaActiva = false;
        private float contador;
        private bool previewActiva = true;

        public MenuPrincipal menu = new MenuPrincipal();
        public TgcFpsCameraMenu camaraMenu;
        private TgcMp3Player musicaMenu = new TgcMp3Player();
        private Boolean comenceAJugar = true;

        /// <summary>
        ///     Se llama una sola vez, al principio cuando se ejecuta el ejemplo.
        ///     Escribir aquí todo el código de inicialización: cargar modelos, texturas, estructuras de optimización, todo
        ///     procesamiento que podemos pre calcular para nuestro juego.
        ///     Borrar el codigo ejemplo no utilizado.
        /// </summary>
        /// 
        //------------------------------------------------------
        public override void Init()
        {

            D3DDevice.Instance.Device.Transform.Projection =
               Matrix.PerspectiveFovLH(D3DDevice.Instance.FieldOfView,
                   D3DDevice.Instance.AspectRatio,
                   D3DDevice.Instance.ZNearPlaneDistance,
                   D3DDevice.Instance.ZFarPlaneDistance * 2f);

            Cursor.Hide();

            menu.Init(this);
            this.configuroCamara();
            personaje.Init(this);
            escenario.Init(this);

            ic.Init(this, personaje);
            ii.Init(this, personaje);

            cilindroColision = new TgcBoundingCylinder(camaraInterna.LookAt, 0.5f, 200f);
            updownRot = Geometry.DegreeToRadian(90f) + (FastMath.PI / 10.0f);
            cilindroColision.rotateZ(updownRot);
            cilindroColision.setRenderColor(Color.LimeGreen);
            cilindroColision.updateValues();

            camaraMenu = new TgcFpsCameraMenu(new TGCVector3(3966, 25, -37), new TGCVector3(3965, 25, -38));
            Camara = camaraMenu;
            musicaMenu.FileName = MediaDir + "\\Music\\MenuTheme.mp3";

            bitmapTitulo = new CustomBitmap(this.MediaDir + "Bitmaps\\" + "Titulo.png", D3DDevice.Instance.Device);
            spriteTitulo.Bitmap = bitmapTitulo;
            
            //escalado para pantallas de otros sizes
            //pantalla que use originalmente, ancho 1366, alto 768
            var anchoOriginal = 1366f;
            var altoOriginal = 768f;

            var factorCorreccionAncho = D3DDevice.Instance.Device.Viewport.Width / anchoOriginal;
            var factorCorreccionAlto = D3DDevice.Instance.Device.Viewport.Height / altoOriginal;

            spriteTitulo.Scaling = new TGCVector2(.8f * factorCorreccionAncho, 1.6f * factorCorreccionAlto);
            spriteTitulo.Position = new TGCVector2(D3DDevice.Instance.Device.Viewport.Width / 5f, D3DDevice.Instance.Device.Viewport.Height / 8f);
        }
        /// <summary>
        ///     Se llama en cada frame.
        ///     Se debe escribir toda la lógica de computo del modelo, así como también verificar entradas del usuario y reacciones
        ///     ante ellas.
        /// </summary>
        public override void Update()
        {
            PreUpdate();

            //-------intro Juego-----------------
            if (previewActiva)
            {
                contador += ElapsedTime;
            }

            if (contador < 7)
            {
                Camara.SetCamera(new TGCVector3(1000, 1000 + contador * 100, 1000), new TGCVector3(500, 500, 500));

            }
            else if (contador < 10)
            {
                Camara.SetCamera(new TGCVector3(-700, -500, -500 + contador * 100), new TGCVector3(-200, -100, 500));

            }
            else if (contador < 15)
            {
                Camara.SetCamera(new TGCVector3(2000, 50 + contador * 100, -500), new TGCVector3(100, 1000, 500));

            }
            else if (contador < 20)
            {

                Camara.SetCamera(new TGCVector3(-700, -100, 600), new TGCVector3(-1000, -700, 200));

            }
            else if (contador < 21)
            {

                Camara.SetCamera(new TGCVector3(3966, 25, -37), new TGCVector3(3965, 25, -38));


            }
            else
            {
                previewActiva = false;
            }

            //------------------------



            if (partidaActiva) {

                if (comenceAJugar)
                {
                    musicaMenu.closeFile();
                }

                escenario.Update();
                personaje.Update();
                ic.Update();
                ii.Update();

                cilindroColision.Center = camaraInterna.LookAt;
                cilindroColision.move(personaje.Position - camaraInterna.LookAt);
                leftrightRot -= -Input.XposRelative * camaraInterna.RotationSpeed;
                updownRot -= -Input.YposRelative * camaraInterna.RotationSpeed;
                cilindroColision.Rotation = new TGCVector3(0, leftrightRot, updownRot);
                cilindroColision.updateValues();
                escenario.detectarColision(cilindroColision);

                comenceAJugar = false;
            }
            else
            {
                switch (musicaMenu.getStatus())
                {
                    case TgcMp3Player.States.Open:
                        musicaMenu.play(true);
                        break;
                    case TgcMp3Player.States.Stopped:
                        musicaMenu.play(true);
                        break;
                }

                if (Input.keyPressed(Key.P))
                {
                    switch (musicaMenu.getStatus())
                    {
                        case TgcMp3Player.States.Playing:
                            musicaMenu.pause();
                            break;
                        case TgcMp3Player.States.Paused:
                            musicaMenu.resume();
                            break;
                    }
                }
            }
            PostUpdate();
        }

        /// <summary>
        ///     Se llama cada vez que hay que refrescar la pantalla.
        ///     Escribir aquí todo el código referido al renderizado.
        ///     Borrar todo lo que no haga falta.
        /// </summary>
        public override void Render()
        {
            //Inicio el render de la escena, para ejemplos simples. Cuando tenemos postprocesado o shaders es mejor realizar las operaciones según nuestra conveniencia.
            PreRender();
            escenario.Render();
            if (partidaActiva)
            {
                
                personaje.Render();
                ic.Render();
                ii.Render();

                //cilindroColision.Render();
            }
            else if(!partidaActiva && !previewActiva){
                
                spriteDrawer.BeginDrawSprite();
                spriteDrawer.DrawSprite(spriteTitulo);
                spriteDrawer.EndDrawSprite();

                menu.Render();
            }
            //Finaliza el render y presenta en pantalla, al igual que el preRender se debe para casos puntuales es mejor utilizar a mano las operaciones de EndScene y PresentScene
            PostRender();
        }

        /// <summary>
        ///     Se llama cuando termina la ejecución del ejemplo.
        ///     Hacer Dispose() de todos los objetos creados.
        ///     Es muy importante liberar los recursos, sobretodo los gráficos ya que quedan bloqueados en el device de video.
        /// </summary>
        public override void Dispose()
        {
            menu.Dispose();
            escenario.Dispose();
            personaje.Dispose();
            ic.Dispose();
            ii.Dispose();

            cilindroColision.Dispose();

        }

        private void configuroCamara()
        {
            camaraInterna = new TgcFpsCamera(new TGCVector3(600, 10, -250), camaraMoveSpeed, camaraJumpSpeed, this);
        }

        public Random GetRandom { get => rnd; }
        public Personaje Personaje { get => personaje; set => personaje = value; }
        public Escenario Escenario { get => escenario; }
        public TgcMesh Workbench { get => Workbench; }
        public InterfazDeCrafting InterfazCrafting { get => ic; }
        public InterfazDeInventario InterfazInventario { get => ii; }
    }
}
            