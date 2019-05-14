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

        //Boleano para ver si dibujamos el boundingbox
        //private bool BoundingBox { get; set; }

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

        private Random rnd = new Random();

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
            this.configuroCamara();
            personaje.Init(this);
            escenario.Init(this);

            ic.Init(this, personaje);

            cilindroColision = new TgcBoundingCylinder(camaraInterna.Position, 0.08f, 150f);
            leftrightRot = 0;
            updownRot = Geometry.DegreeToRadian(90f) + (FastMath.PI / 10.0f);
            cilindroColision.rotateZ(updownRot);
            cilindroColision.rotateY(leftrightRot);
            cilindroColision.setRenderColor(Color.LimeGreen);
            cilindroColision.updateValues();
        }
        /// <summary>
        ///     Se llama en cada frame.
        ///     Se debe escribir toda la lógica de computo del modelo, así como también verificar entradas del usuario y reacciones
        ///     ante ellas.
        /// </summary>
        public override void Update()
        {
            PreUpdate();

            escenario.Update();
            personaje.Update();
            ic.Update();

            cilindroColision.Center = camaraInterna.Position;
            if (camaraInterna.LockCam || Input.buttonDown(TgcD3dInput.MouseButtons.BUTTON_LEFT))
            {
                leftrightRot -= -Input.XposRelative * camaraInterna.RotationSpeed;
                updownRot -= -Input.YposRelative * camaraInterna.RotationSpeed;
                cilindroColision.Rotation = new TGCVector3(0, leftrightRot, updownRot);
            }

            cilindroColision.updateValues();
            escenario.detectarColision(cilindroColision);

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
            personaje.Render();
            ic.Render();

            //cilindroColision.Render();

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
            escenario.Dispose();
            personaje.Dispose();
            ic.Dispose();

            cilindroColision.Dispose();

        }

        private void configuroCamara()
        {
            camaraInterna = new TgcFpsCamera(new TGCVector3(600, 60, -250), camaraMoveSpeed, camaraJumpSpeed, Input);
            Camara = camaraInterna;
        }

        public Random GetRandom { get => rnd; }
        public Personaje Personaje { get => personaje; set => personaje = value; }
        public Escenario Escenario { get => escenario; }
        public TgcMesh Workbench { get => Workbench; }
        public InterfazDeCrafting InterfazCrafting { get => ic; }
    }
}
            