using BulletSharp;
using Microsoft.DirectX.DirectInput;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.BoundingVolumes;
using TGC.Core.BulletPhysics;
using TGC.Core.Collision;
using TGC.Core.Direct3D;
using TGC.Core.Geometry;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;
using TGC.Core.Sound;
using TGC.Core.Terrain;
using TGC.Core.Textures;
using TGC.Group.Model;
using TGC.Group.Model.Camara;
using TGC.Group.Model.Sprites;
using TGC.Core.Shaders;
using Microsoft.DirectX.Direct3D;
using Effect = Microsoft.DirectX.Direct3D.Effect;

namespace LosTiburones.Model
{
    public class Escenario
    {   //DECLARO VARIABLES 'GLOBALES'
        GameModel GModel;
        
        private TgcPlane piso, agua;
        private TgcMesh coralBrain, coral, meshTiburon, fish, pillarCoral, seaShell, spiralWireCoral, treeCoral, yellowFish;
        private TgcMesh arbusto, arbusto2, pasto, planta, planta2, planta3, roca;

        private TgcMesh workbench;

        private TgcScene barco;
        private TgcSkyBox skybox;
        private ObjetoRecolectable objetoAMostrar;
        private List<ObjetoRecolectable> objetosRecolectables = new List<ObjetoRecolectable>();

        private TgcSimpleTerrain terreno;
        private float currentScaleXZ;
        private float currentScaleY;

        private TgcMp3Player musica = new TgcMp3Player();
        private TgcStaticSound sonidoTiburonCerca = new TgcStaticSound();

        private List<TgcMesh> objetosEstaticosEnArray = new List<TgcMesh>();
        private List<Pez> pecesAmarillos = new List<Pez>();
        private List<Pez> pecesAzules = new List<Pez>();

        private Pez pezCircular;

        private Tiburon tiburon;

        //Constantes para velocidades de movimiento
        private const float ROTATION_SPEED = 50f;
        private const float MOVEMENT_SPEED = 50f;

        private CustomBitmap bitmapCursor;
        private CustomBitmap bitmapCorazon, bitmapTanqueOxigeno, bitmapBarra00;
        private CustomBitmap bitmapBarraVida10, bitmapBarraVida20, bitmapBarraVida30, bitmapBarraVida40, bitmapBarraVida50,
                bitmapBarraVida60, bitmapBarraVida70, bitmapBarraVida80, bitmapBarraVida90, bitmapBarraVida100;
        private CustomBitmap bitmapBarraOxigeno10, bitmapBarraOxigeno20, bitmapBarraOxigeno30, bitmapBarraOxigeno40, bitmapBarraOxigeno50,
                bitmapBarraOxigeno60, bitmapBarraOxigeno70, bitmapBarraOxigeno80, bitmapBarraOxigeno90, bitmapBarraOxigeno100;
        private Drawer2D spriteDrawer = new Drawer2D();

        private List<CustomSprite> sprites = new List<CustomSprite>();

        private CustomSprite spriteCursor = new CustomSprite();
        private CustomSprite spriteCorazon = new CustomSprite();
        private CustomSprite spriteBarraVida = new CustomSprite();
        private CustomSprite spriteBarraOxigeno = new CustomSprite();
        private CustomSprite spriteTanqueOxigeno = new CustomSprite();

        private int ScreenWidth = D3DDevice.Instance.Device.Viewport.Width;
        private int ScreenHeight = D3DDevice.Instance.Device.Viewport.Height;

        //private Personaje personaje = new Personaje(100, 100);

        //-----------Fisica-------------------
        private RigidBody rigidCamera;

        DiscreteDynamicsWorld dynamicsWorld;
        CollisionDispatcher dispatcher;
        DefaultCollisionConfiguration collisionConfiguration;
        SequentialImpulseConstraintSolver constraintSolver;
        BroadphaseInterface broadphaseInterface;

        TGCVector3 posicionInicial = new TGCVector3(600, 60, -250);
        TgcFpsCamera camaraInterna;

        //----------BULLET DEBUG: DebugDrawer para Bullet
        private DebugDraw debugDrawer;
        
        //-------------------

        //-------Shaders----------
        private Effect efectoSuperficieAgua;
        private float time;
        //---------------------

        public void Init(GameModel gmodel)
        {
            this.GModel = gmodel;

            //----------------Fisica------------

            //----configuracion del Mundo fisico--------------
            collisionConfiguration = new DefaultCollisionConfiguration();
            dispatcher = new CollisionDispatcher(collisionConfiguration);
            GImpactCollisionAlgorithm.RegisterAlgorithm(dispatcher);
            constraintSolver = new SequentialImpulseConstraintSolver();
            broadphaseInterface = new DbvtBroadphase();
            dynamicsWorld = new DiscreteDynamicsWorld(dispatcher, broadphaseInterface, constraintSolver, collisionConfiguration);
            //dynamicsWorld.Gravity=new TGCVector3(0,-20f,0).ToBulletVector3();
            //dynamicsWorld.Gravity=new TGCVector3(0,0,0).ToBulletVector3();
            
            //-------------creacion rigidbodies-------------------
            var ballShape = new SphereShape(10);
            var ballTransform = TGCMatrix.Identity;
            ballTransform.Origin = posicionInicial;//GModel.Camara.Position;//new TGCVector3(100, 50, 0);
            var ballMotionState = new DefaultMotionState(ballTransform.ToBsMatrix);
            var ballInertia = ballShape.CalculateLocalInertia(1f);
            var ballInfo = new RigidBodyConstructionInfo(1, ballMotionState, ballShape, ballInertia);

            rigidCamera = new RigidBody(ballInfo);
            rigidCamera.SetDamping(0.9f, 0.9f);
            //esto es para que no le afecte la gravedad al inicio de la partida
            rigidCamera.ActivationState = ActivationState.IslandSleeping;
            dynamicsWorld.AddRigidBody(rigidCamera);

            //BULLET DEBUG: Desabilitar el Bullet Debug de la camara. Solo si se va a Debugear Bullet
            //rigidCamera.CollisionFlags = CollisionFlags.DisableVisualizeObject; //Comentar esta linea si se quiere debugear la camara

            camaraInterna = new TgcFpsCamera(posicionInicial, GModel.Input);

            //Device de DirectX para crear primitivas.
            var d3dDevice = D3DDevice.Instance.Device;
            
            this.cargoPisos();

            this.cargoSkybox();

            this.cargoHeightmap();

            this.cargoMusica();

            this.cargoMeshes();

            this.generoObjetosEstaticosSueltos();

            this.generoObjetosEstaticosEnArray();

            this.generoPecesAmarillos();

            this.generoPecesAzules();

            this.generoMetales();

            this.generoHUD();


            //-----------Fisica---------
            //para el heighmap

            var heighmapRigid = BulletRigidBodyFactory.Instance.CreateSurfaceFromHeighMap(terreno.getData());
            dynamicsWorld.AddRigidBody(heighmapRigid);

            //BULLET DEBUG: Desabilitar el Bullet Debug del heighmap. Solo si se va a Debugear Bullet
            //heighmapRigid.CollisionFlags = CollisionFlags.DisableVisualizeObject; //Comentar esta linea si se quiere debugear el Heighmap

            //----------------------

            //-----------cargar shaders----
            efectoSuperficieAgua = TGCShaders.Instance.LoadEffect(GModel.ShadersDir + "SuperficieDeAgua.fx");
            agua.Effect = efectoSuperficieAgua;
            agua.Technique = "OleajeNormal";
            time = 0;
            //-----------

            workbench = new TgcSceneLoader().loadSceneFromFile(GModel.MediaDir + "ModelosTgc\\Workbench\\Workbench-TgcScene.xml").Meshes[0];
            workbench.Scale = new TGCVector3(0.2f, 0.2f, 0.2f);
            workbench.Position = new TGCVector3(150f, -60, -50f);
            workbench.RotateY(-FastMath.PI/2);

            //BULLET DEBUG: Debug para Bullet (Commentar estas lineas si NO se desea Debugear)
            //-------------Start Bullet Debug Config----------------
            //debugDrawer = new ADebugDrawer(DebugDrawModes.DrawWireframe);
            //dynamicsWorld.DebugDrawer = debugDrawer;
            //-------------End Bullet Debug Config------------------

        }
        public void Update()
        {
            //Capturar Input teclado
            //if (Input.keyPressed(Key.F))
            //{
            //    BoundingBox = !BoundingBox;
            //}

            var Input=this.GModel.Input;

            //-------Fisica----------
            dynamicsWorld.StepSimulation(1 / 60f, 100);

            //----------------
            var director = GModel.Camara.LookAt - GModel.Camara.Position; //new TGCVector3(1,0,0);
            director.Normalize();
            var strength = 50f;
            //if (GModel.Input.keyDown(Key.UpArrow))
            if (GModel.Input.keyDown(Key.W))
            {

                //muevo el rigidsegun a donde miro

                rigidCamera.ActivationState = ActivationState.ActiveTag;
               
                rigidCamera.ApplyCentralImpulse(strength * director.ToBulletVector3());

            }
            if (GModel.Input.keyUp(Key.W))
            {
                //para detener el rigid
                //op1
                rigidCamera.ActivationState = ActivationState.IslandSleeping;
                //op2
                //rigidCamera.ActivationState = ActivationState.DisableSimulation;
            }

            if (GModel.Input.keyDown(Key.A))
            {

                //muevo el rigidsegun a donde miro

                rigidCamera.ActivationState = ActivationState.ActiveTag;

                var miIzquierda = new TGCVector3();
                miIzquierda.X = director.X * FastMath.Cos(FastMath.PI_HALF) - director.Z * FastMath.Sin(FastMath.PI_HALF);
                miIzquierda.Z = director.X * FastMath.Sin(FastMath.PI_HALF) + director.Z * FastMath.Cos(FastMath.PI_HALF);

                rigidCamera.ApplyCentralImpulse(strength * miIzquierda.ToBulletVector3());

            }
            if (GModel.Input.keyUp(Key.A))
            {
                //para detener el rigid
                //op1
                rigidCamera.ActivationState = ActivationState.IslandSleeping;
                //op2
                //rigidCamera.ActivationState = ActivationState.DisableSimulation;
            }

            if (GModel.Input.keyDown(Key.D))
            {

                //muevo el rigidsegun a donde miro

                rigidCamera.ActivationState = ActivationState.ActiveTag;

                var miDerecha = new TGCVector3();
                miDerecha.X = director.X * FastMath.Cos(FastMath.PI + FastMath.PI_HALF) - director.Z * FastMath.Sin(FastMath.PI + FastMath.PI_HALF);
                miDerecha.Z = director.X * FastMath.Sin(FastMath.PI + FastMath.PI_HALF) + director.Z * FastMath.Cos(FastMath.PI + FastMath.PI_HALF);

                rigidCamera.ApplyCentralImpulse(strength * miDerecha.ToBulletVector3());

            }
            if (GModel.Input.keyUp(Key.D))
            {
                //para detener el rigid
                //op1
                rigidCamera.ActivationState = ActivationState.IslandSleeping;
                //op2
                //rigidCamera.ActivationState = ActivationState.DisableSimulation;
            }

            //if (GModel.Input.keyDown(Key.DownArrow))
            if (GModel.Input.keyDown(Key.S))
            {

                //muevo el rigidsegun a donde miro
                rigidCamera.ActivationState = ActivationState.ActiveTag;
                rigidCamera.ApplyCentralImpulse(-strength * director.ToBulletVector3());

            }
            if (GModel.Input.keyUp(Key.S))
            {
                //para detener el rigid
                //op1
                rigidCamera.ActivationState = ActivationState.IslandSleeping;
                //op2
                //rigidCamera.ActivationState = ActivationState.DisableSimulation;
            }
            //-----------------------

            //Capturar Input teclado
            if (Input.keyPressed(Key.P))
            {
                if (musica.getStatus().Equals(TgcMp3Player.States.Playing))
                {
                    musica.pause();
                }
                else if (musica.getStatus().Equals(TgcMp3Player.States.Paused))
                {
                    musica.resume();
                } else if (musica.getStatus().Equals(TgcMp3Player.States.Open))
                {
                    musica.play(true);
                }
            }

            //Reseteo el juego si apreto R
            //if (Input.keyPressed(Key.R))
            //{
            //this.Dispose();
            //this.Init();
            //}

            //-----------movimientos-------------
            tiburon.moverse(this);

            //-----------
            //Muevo los peces amarillos

            pezCircular.Update(GModel.ElapsedTime);

            pecesAmarillos.ForEach(pez =>
            {
                pez.Position += new TGCVector3(5f * pez.MoveSpeed * GModel.ElapsedTime * pez.CurrentMoveDir, 0, 0);

                if (GModel.GetRandom.Next(0, 2000) > GModel.GetRandom.Next(1998, 1999) //cada tanto
                    || this.fueraDelMapa(pez)) //si toco los bordes
                {
                    //reboto
                    pez.CurrentMoveDir *= -1;
                    pez.Rotation += new TGCVector3(FastMath.PI, 0, 0);
                }

                pez.Transform = TGCMatrix.Scaling(pez.Scale) * TGCMatrix.RotationYawPitchRoll(pez.Rotation.X, pez.Rotation.Y, pez.Rotation.Z) * TGCMatrix.Translation(pez.Position);
            }
            );

            //-----------
            //Muevo los peces azules

            pecesAzules.ForEach(pez =>
            {
                pez.Position += new TGCVector3(0, 0, 2f * pez.MoveSpeed * GModel.ElapsedTime * pez.CurrentMoveDir);

                if (GModel.GetRandom.Next(0, 2000) > GModel.GetRandom.Next(1998, 1999) //cada tanto
                    || this.fueraDelMapa(pez)) //si toco los bordes
                {
                    //reboto
                    pez.CurrentMoveDir *= -1;
                    pez.Rotation += new TGCVector3(FastMath.PI, 0, 0);
                }

                pez.Transform = TGCMatrix.Scaling(pez.Scale) * TGCMatrix.RotationYawPitchRoll(pez.Rotation.X, pez.Rotation.Y, pez.Rotation.Z) * TGCMatrix.Translation(pez.Position);
            }
            );

            //-----Skybox
            skybox.Center = GModel.Personaje.Position; //GModel.Camara.Position;
            //----------

            //REFRESCO EL TAMAÑO DE LA PANTALLA
            ScreenWidth = D3DDevice.Instance.Device.Viewport.Width;
            ScreenHeight = D3DDevice.Instance.Device.Viewport.Height;

            this.actualizoValoresSaludOxigeno(GModel.Personaje);

        }

        public void Render()
        {
            //Dibuja un texto por pantalla
            //DrawText.drawText("Con la tecla F se dibuja el bounding box.", 0, 20, Color.OrangeRed);
            GModel.DrawText.drawText("Con la tecla P se activa o desactiva la música.", 0, 30, Color.OrangeRed);
            GModel.DrawText.drawText("Con clic izquierdo subimos la camara [Actual]: " + TGCVector3.PrintVector3(GModel.Camara.Position), 0, 40, Color.OrangeRed);

            //BULLET DEBUG: Le indico a bullet que Dibuje las lineas de debug.

            //dynamicsWorld.DebugDrawWorld(); //ATENCION: COMENTAR ESTA LINEA SI NO SE DESEA DEBUGEAR BULLET

            //--------------------------------

            //----Fisica----------

            camaraInterna.setPosicion(new TGCVector3(rigidCamera.CenterOfMassPosition));
            GModel.Camara = camaraInterna;

            //----------------

            //----------Shaders----------
            time += GModel.ElapsedTime;
            efectoSuperficieAgua.SetValue("time", time);
            
            //--------------

            if (GModel.Personaje.Vivo)
            {
                if (bajoElAgua(GModel.Personaje))
                {
                    GModel.DrawText.drawText("Sufriendo daño por falta de oxigeno", 0, 50, Color.Red);
                }

                if ((this.fueraDelMapa(GModel.Personaje)))
                {
                    GModel.DrawText.drawText("Sufriendo daño por estar fuera del mapa", 0, 60, Color.Red);
                }

                /*
                if ((tiburon.estoyCercaDelPersonaje(GModel.Personaje)))
                {
                    GModel.DrawText.drawText("Estás cerca del tiburón", 0, 70, Color.Red);
                }
                */
            }
            else
            {
                GModel.DrawText.drawText("Te moriste", ScreenWidth / 2, ScreenHeight / 2, Color.Red);
            }

            if (objetoAMostrar != null)
            {
                GModel.DrawText.drawText("Recolectar: " + objetoAMostrar.Nombre, Convert.ToInt32(Math.Round((double)ScreenWidth / 2.2)), Convert.ToInt32(Math.Round((double)ScreenHeight / 2.2)), Color.Red);
            }

            //--------skybox---------
            skybox.Render();
            //---------------
            //----heighmap
            terreno.Render();

            //------------------------------------
            agua.Render();
            piso.Render();
            coral.Render();
            tiburon.Render();
            coralBrain.Render();
            barco.RenderAll();
            fish.Render();
            pillarCoral.Render();
            seaShell.Render();
            spiralWireCoral.Render();
            treeCoral.Render();
            yellowFish.Render();
            arbusto.Render();
            arbusto2.Render();
            pasto.Render();
            planta.Render();
            planta2.Render();
            planta3.Render();
            roca.Render();

            objetosEstaticosEnArray.ForEach(obj => obj.Render());

            pecesAmarillos.ForEach(obj => obj.Render());
            pezCircular.Render();
            pecesAzules.ForEach(obj => obj.Render());

            objetosRecolectables.ForEach(obj => obj.Render());

            //SPRITES
            spriteDrawer.BeginDrawSprite();
            sprites.ForEach(sprite => spriteDrawer.DrawSprite(sprite));
            spriteDrawer.EndDrawSprite();


            workbench.Render();

        }

        public void Dispose()
        {

            //---------fisica------
            dynamicsWorld.Dispose();
            dispatcher.Dispose();
            collisionConfiguration.Dispose();
            constraintSolver.Dispose();
            broadphaseInterface.Dispose();
            rigidCamera.Dispose();

            //------------
            //----------Shaders-------
            efectoSuperficieAgua.Dispose();

            //-------------

            //------------------------
            skybox.Dispose();
            terreno.Dispose();
            agua.Dispose();
            piso.Dispose();
            coral.Dispose();
            tiburon.Dispose();
            coralBrain.Dispose();
            barco.DisposeAll();
            fish.Dispose();
            pillarCoral.Dispose();
            seaShell.Dispose();
            spiralWireCoral.Dispose();
            treeCoral.Dispose();
            yellowFish.Dispose();
            arbusto.Dispose();
            arbusto2.Dispose();
            pasto.Dispose();
            planta.Dispose();
            planta2.Dispose();
            planta3.Dispose();
            roca.Dispose();

            objetosEstaticosEnArray.ForEach(obj => obj.Dispose());

            pecesAmarillos.ForEach(obj => obj.Dispose());
            pezCircular.Dispose();

            pecesAzules.ForEach(obj => obj.Dispose());

            objetosRecolectables.ForEach(obj => obj.Dispose());

            //BULLET DEBUG: Libero el DebugDrawer
            //debugDrawer.Dispose(); //Comentar esta linea si no se desea debugear


            workbench.Dispose();
        }


        private Boolean fueraDelMapa(Personaje personaje)
        {
            return personaje.Position.X > 20000 || personaje.Position.X < -20000 || personaje.Position.Z > 20000 || personaje.Position.Z < -20000;
        }

        private Boolean fueraDelMapa(Pez pez)
        {
            return pez.Position.X > 20000 || pez.Position.X < -20000 || pez.Position.Z > 20000 || pez.Position.Z < -20000;
        }

        private Boolean dentroDelMapa(Personaje personaje)
        {
            return personaje.Position.X <= 20000 && personaje.Position.X >= -20000 && personaje.Position.Z <= 20000 && personaje.Position.Z >= -20000;
        }

        private Boolean bajoElAgua(Personaje personaje)
        {
            return personaje.Position.Y < 0;
        }

        private Boolean sobreElAgua(Personaje personaje)
        {
            return personaje.Position.Y >= 0;
        }

        private void actualizoValoresSaludOxigeno(Personaje personaje)
        {
            //ACTUALIZO LOS VALORES DE SALUD Y OXIGENO
            if (this.fueraDelMapa(personaje))
            {
                personaje.sufriDanio(7.5f * GModel.ElapsedTime) ;
            }

            if (this.dentroDelMapa(personaje))
            {
                personaje.recuperaVida(3f * GModel.ElapsedTime);
            }

            if (this.bajoElAgua(personaje))
            {
                personaje.perdeOxigeno(7.5f * GModel.ElapsedTime);
            }

            if (this.sobreElAgua(personaje))
            {
                personaje.recuperaOxigeno(3f * GModel.ElapsedTime);
            }

            //ACTUALIZO LOS SPRITES DE ENERGIA Y OXIGENO
            if (personaje.Vivo)
            {
                switch (personaje.Health)
                {
                    case float n when (n <= 0):
                        spriteBarraVida.Bitmap = bitmapBarra00;
                        break;
                    case float n when (n >= 1 && n <= 10):
                        spriteBarraVida.Bitmap = bitmapBarraVida10;
                        break;
                    case float n when (n >= 11 && n <= 20):
                        spriteBarraVida.Bitmap = bitmapBarraVida20;
                        break;
                    case float n when (n >= 21 && n <= 30):
                        spriteBarraVida.Bitmap = bitmapBarraVida30;
                        break;
                    case float n when (n >= 31 && n <= 40):
                        spriteBarraVida.Bitmap = bitmapBarraVida40;
                        break;
                    case float n when (n >= 41 && n <= 50):
                        spriteBarraVida.Bitmap = bitmapBarraVida50;
                        break;
                    case float n when (n >= 51 && n <= 60):
                        spriteBarraVida.Bitmap = bitmapBarraVida60;
                        break;
                    case float n when (n >= 61 && n <= 70):
                        spriteBarraVida.Bitmap = bitmapBarraVida70;
                        break;
                    case float n when (n >= 71 && n <= 80):
                        spriteBarraVida.Bitmap = bitmapBarraVida80;
                        break;
                    case float n when (n >= 81 && n <= 90):
                        spriteBarraVida.Bitmap = bitmapBarraVida90;
                        break;
                    case float n when (n >= 91 && n <= 100):
                        spriteBarraVida.Bitmap = bitmapBarraVida100;
                        break;
                }

                switch (personaje.Oxygen)
                {
                    case float n when (n <= 0):
                        spriteBarraOxigeno.Bitmap = bitmapBarra00;
                        break;
                    case float n when (n >= 1 && n <= 10):
                        spriteBarraOxigeno.Bitmap = bitmapBarraOxigeno10;
                        break;
                    case float n when (n >= 11 && n <= 20):
                        spriteBarraOxigeno.Bitmap = bitmapBarraOxigeno20;
                        break;
                    case float n when (n >= 21 && n <= 30):
                        spriteBarraOxigeno.Bitmap = bitmapBarraOxigeno30;
                        break;
                    case float n when (n >= 31 && n <= 40):
                        spriteBarraOxigeno.Bitmap = bitmapBarraOxigeno40;
                        break;
                    case float n when (n >= 41 && n <= 50):
                        spriteBarraOxigeno.Bitmap = bitmapBarraOxigeno50;
                        break;
                    case float n when (n >= 51 && n <= 60):
                        spriteBarraOxigeno.Bitmap = bitmapBarraOxigeno60;
                        break;
                    case float n when (n >= 61 && n <= 70):
                        spriteBarraOxigeno.Bitmap = bitmapBarraOxigeno70;
                        break;
                    case float n when (n >= 71 && n <= 80):
                        spriteBarraOxigeno.Bitmap = bitmapBarraOxigeno80;
                        break;
                    case float n when (n >= 81 && n <= 90):
                        spriteBarraOxigeno.Bitmap = bitmapBarraOxigeno90;
                        break;
                    case float n when (n >= 91 && n <= 100):
                        spriteBarraOxigeno.Bitmap = bitmapBarraOxigeno100;
                        break;
                }
            }
            else
            {
                if (personaje.Health < 0)
                {
                    spriteBarraVida.Bitmap = bitmapBarra00;
                }

                if (personaje.Oxygen < 0)
                {
                    spriteBarraOxigeno.Bitmap = bitmapBarra00;
                }
            }
        }

        private void generoPecesAmarillos()
        {
            pezCircular = new Pez(yellowFish.createMeshInstance("pezPrueba"), new TGCVector3(0, -100, 0), new TGCVector3(0, 1.57f, 0), new MovimientoCircular(new TGCVector3(0, -100, 0), 150, 0.25f));
            //----------------
            //PECES AMARILLOS
            //Se mueven en X
            //Se autoescalan entre 1 y 5
            //Son 64
            //Tienen una velocidad de entre 25 y 75
            //----------------
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    int currentMoveDirection = GModel.GetRandom.Next(0, 2) * 2 - 1; //Devuelve aleatoriamente una direccion de movimiento inicial (-1 o 1)
                    float moveSpeed = (float)(GModel.GetRandom.NextDouble() * 75) + 25;

                    Pez pez = new Pez(yellowFish.createMeshInstance(yellowFish.Name + i + "_" + j), currentMoveDirection, moveSpeed);
                    pez.Position = new TGCVector3(GModel.GetRandom.Next(-4000, 4000), GModel.GetRandom.Next(-900, -50), GModel.GetRandom.Next(-4000, 4000));

                    int scale = GModel.GetRandom.Next(1, 5);
                    pez.Scale = new TGCVector3(scale, scale, scale);

                    //Corrijo que los peces vayan para atras
                    if (pez.CurrentMoveDir.Equals(1))
                    {
                        pez.Rotation += new TGCVector3(FastMath.PI, 0, 0);
                        pez.Transform = TGCMatrix.Scaling(pez.Scale) * TGCMatrix.RotationYawPitchRoll(pez.Rotation.X, pez.Rotation.Y, pez.Rotation.Z) * TGCMatrix.Translation(pez.Position);
                    }
                    else
                    {
                        pez.Transform = TGCMatrix.Scaling(pez.Scale) * TGCMatrix.Translation(pez.Position);
                    }

                    pecesAmarillos.Add(pez);
                }

            }
        }

        private void generoPecesAzules()
        {
            //----------------
            //PECES AZULES
            //Se mueven en Z
            //Se autoescalan entre 10 y 20
            //Son 64
            //Tienen una velocidad de entre 40 y 90
            //----------------
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    int currentMoveDirection = GModel.GetRandom.Next(0, 2) * 2 - 1; //Devuelve aleatoriamente una direccion de movimiento inicial (-1 o 1)
                    float moveSpeed = (float)(GModel.GetRandom.NextDouble() * 90) + 40;

                    Pez pez = new Pez(fish.createMeshInstance(fish.Name + i + "_" + j), currentMoveDirection, moveSpeed);
                    pez.Position = new TGCVector3(GModel.GetRandom.Next(-4000, 4000), GModel.GetRandom.Next(-900, -50), GModel.GetRandom.Next(-4000, 4000));

                    int scale = GModel.GetRandom.Next(10, 20);
                    pez.Scale = new TGCVector3(scale, scale, scale);

                    //Corrijo que los peces vayan para atras
                    if (pez.CurrentMoveDir.Equals(1))
                    {
                        pez.Rotation += new TGCVector3(FastMath.PI, 0, 0);
                        pez.Transform = TGCMatrix.Scaling(pez.Scale) * TGCMatrix.RotationYawPitchRoll(pez.Rotation.X, pez.Rotation.Y, pez.Rotation.Z) * TGCMatrix.Translation(pez.Position);
                    }
                    else
                    {
                        pez.Transform = TGCMatrix.Scaling(pez.Scale) * TGCMatrix.Translation(pez.Position);
                    }

                    pecesAzules.Add(pez);
                }

            }
        }

        private void generoHUD()
        {
            //Cargo cursor
            Microsoft.DirectX.Direct3D.Device d3dDevice = D3DDevice.Instance.Device;
            bitmapCursor = new CustomBitmap(GModel.MediaDir + "Bitmaps\\" + "cursor_default.png", d3dDevice);
            spriteCursor.Bitmap = bitmapCursor;
            spriteCursor.Scaling = new TGCVector2(1f, 1f);
            spriteCursor.Position = new TGCVector2(ScreenWidth / 2.093f, ScreenHeight / 2.17f);
            sprites.Add(spriteCursor);
            //HUD
            bitmapCorazon = new CustomBitmap(GModel.MediaDir + "Bitmaps\\" + "Vida.png", D3DDevice.Instance.Device);
            spriteCorazon.Bitmap = bitmapCorazon;
            spriteCorazon.Scaling = new TGCVector2(0.15f, 0.15f);
            spriteCorazon.Position = new TGCVector2(ScreenWidth / 1.1f, ScreenHeight / 1.15f);
            sprites.Add(spriteCorazon);

            bitmapBarraVida100 = new CustomBitmap(GModel.MediaDir + "Bitmaps\\" + "BarraVida1.0.png", D3DDevice.Instance.Device);
            spriteBarraVida.Bitmap = bitmapBarraVida100;
            spriteBarraVida.Scaling = new TGCVector2(.75f, 1f);
            spriteBarraVida.Position = new TGCVector2(ScreenWidth / 1.7f, ScreenHeight / 1.13f);
            sprites.Add(spriteBarraVida);

            bitmapTanqueOxigeno = new CustomBitmap(GModel.MediaDir + "Bitmaps\\" + "Oxygen.png", D3DDevice.Instance.Device);
            spriteTanqueOxigeno.Bitmap = bitmapTanqueOxigeno;
            spriteTanqueOxigeno.Scaling = new TGCVector2(0.25f, 0.2f);
            spriteTanqueOxigeno.Position = new TGCVector2(ScreenWidth / 1.125f, ScreenHeight / 1.4f);
            sprites.Add(spriteTanqueOxigeno);

            bitmapBarraOxigeno100 = new CustomBitmap(GModel.MediaDir + "Bitmaps\\" + "BarraOxigeno1.0.png", D3DDevice.Instance.Device);
            spriteBarraOxigeno.Bitmap = bitmapBarraOxigeno100;
            spriteBarraOxigeno.Scaling = new TGCVector2(.75f, 1f);
            spriteBarraOxigeno.Position = new TGCVector2(ScreenWidth / 1.7f, ScreenHeight / 1.325f);
            sprites.Add(spriteBarraOxigeno);

            //ANIMACION DE LAS BARRAS DE ENERGIA Y OXIGENO
            bitmapBarra00 = new CustomBitmap(GModel.MediaDir + "Bitmaps\\" + "Barra0.0.png", D3DDevice.Instance.Device);

            bitmapBarraVida10 = new CustomBitmap(GModel.MediaDir + "Bitmaps\\" + "BarraVida0.1.png", D3DDevice.Instance.Device);
            bitmapBarraVida20 = new CustomBitmap(GModel.MediaDir + "Bitmaps\\" + "BarraVida0.2.png", D3DDevice.Instance.Device);
            bitmapBarraVida30 = new CustomBitmap(GModel.MediaDir + "Bitmaps\\" + "BarraVida0.3.png", D3DDevice.Instance.Device);
            bitmapBarraVida40 = new CustomBitmap(GModel.MediaDir + "Bitmaps\\" + "BarraVida0.4.png", D3DDevice.Instance.Device);
            bitmapBarraVida50 = new CustomBitmap(GModel.MediaDir + "Bitmaps\\" + "BarraVida0.5.png", D3DDevice.Instance.Device);
            bitmapBarraVida60 = new CustomBitmap(GModel.MediaDir + "Bitmaps\\" + "BarraVida0.6.png", D3DDevice.Instance.Device);
            bitmapBarraVida70 = new CustomBitmap(GModel.MediaDir + "Bitmaps\\" + "BarraVida0.7.png", D3DDevice.Instance.Device);
            bitmapBarraVida80 = new CustomBitmap(GModel.MediaDir + "Bitmaps\\" + "BarraVida0.8.png", D3DDevice.Instance.Device);
            bitmapBarraVida90 = new CustomBitmap(GModel.MediaDir + "Bitmaps\\" + "BarraVida0.9.png", D3DDevice.Instance.Device);
            bitmapBarraVida100 = new CustomBitmap(GModel.MediaDir + "Bitmaps\\" + "BarraVida1.0.png", D3DDevice.Instance.Device);

            bitmapBarraOxigeno10 = new CustomBitmap(GModel.MediaDir + "Bitmaps\\" + "BarraOxigeno0.1.png", D3DDevice.Instance.Device);
            bitmapBarraOxigeno20 = new CustomBitmap(GModel.MediaDir + "Bitmaps\\" + "BarraOxigeno0.2.png", D3DDevice.Instance.Device);
            bitmapBarraOxigeno30 = new CustomBitmap(GModel.MediaDir + "Bitmaps\\" + "BarraOxigeno0.3.png", D3DDevice.Instance.Device);
            bitmapBarraOxigeno40 = new CustomBitmap(GModel.MediaDir + "Bitmaps\\" + "BarraOxigeno0.4.png", D3DDevice.Instance.Device);
            bitmapBarraOxigeno50 = new CustomBitmap(GModel.MediaDir + "Bitmaps\\" + "BarraOxigeno0.5.png", D3DDevice.Instance.Device);
            bitmapBarraOxigeno60 = new CustomBitmap(GModel.MediaDir + "Bitmaps\\" + "BarraOxigeno0.6.png", D3DDevice.Instance.Device);
            bitmapBarraOxigeno70 = new CustomBitmap(GModel.MediaDir + "Bitmaps\\" + "BarraOxigeno0.7.png", D3DDevice.Instance.Device);
            bitmapBarraOxigeno80 = new CustomBitmap(GModel.MediaDir + "Bitmaps\\" + "BarraOxigeno0.8.png", D3DDevice.Instance.Device);
            bitmapBarraOxigeno90 = new CustomBitmap(GModel.MediaDir + "Bitmaps\\" + "BarraOxigeno0.9.png", D3DDevice.Instance.Device);
            bitmapBarraOxigeno100 = new CustomBitmap(GModel.MediaDir + "Bitmaps\\" + "BarraOxigeno1.0.png", D3DDevice.Instance.Device);
        }

        private void cargoMeshes()
        {
            coral = new TgcSceneLoader().loadSceneFromFile(GModel.MediaDir + "\\Aquatic\\Meshes\\coral-TgcScene.xml").Meshes[0];

            meshTiburon = new TgcSceneLoader().loadSceneFromFile(GModel.MediaDir + "\\Aquatic\\Meshes\\shark-TgcScene.xml").Meshes[0];

            coralBrain = new TgcSceneLoader().loadSceneFromFile(GModel.MediaDir + "\\Aquatic\\Meshes\\brain_coral-TgcScene.xml").Meshes[0];

            barco = new TgcSceneLoader().loadSceneFromFile(GModel.MediaDir + "\\Aquatic\\Meshes\\ship-TgcScene.xml");

            fish = new TgcSceneLoader().loadSceneFromFile(GModel.MediaDir + "\\Aquatic\\Meshes\\fish-TgcScene.xml").Meshes[0];

            pillarCoral = new TgcSceneLoader().loadSceneFromFile(GModel.MediaDir + "\\Aquatic\\Meshes\\pillar_coral-TgcScene.xml").Meshes[0];

            seaShell = new TgcSceneLoader().loadSceneFromFile(GModel.MediaDir + "\\Aquatic\\Meshes\\sea_shell-TgcScene.xml").Meshes[0];

            spiralWireCoral = new TgcSceneLoader().loadSceneFromFile(GModel.MediaDir + "\\Aquatic\\Meshes\\spiral_wire_coral-TgcScene.xml").Meshes[0];

            treeCoral = new TgcSceneLoader().loadSceneFromFile(GModel.MediaDir + "\\Aquatic\\Meshes\\tree_coral-TgcScene.xml").Meshes[0];

            yellowFish = new TgcSceneLoader().loadSceneFromFile(GModel.MediaDir + "\\Aquatic\\Meshes\\yellow_fish-TgcScene.xml").Meshes[0];

            arbusto = new TgcSceneLoader().loadSceneFromFile(GModel.MediaDir + "\\MeshCreator\\Meshes\\Vegetacion\\Arbusto\\Arbusto-TgcScene.xml").Meshes[0];

            arbusto2 = new TgcSceneLoader().loadSceneFromFile(GModel.MediaDir + "\\MeshCreator\\Meshes\\Vegetacion\\Arbusto2\\Arbusto2-TgcScene.xml").Meshes[0];

            pasto = new TgcSceneLoader().loadSceneFromFile(GModel.MediaDir + "\\MeshCreator\\Meshes\\Vegetacion\\Pasto\\Pasto-TgcScene.xml").Meshes[0];

            planta = new TgcSceneLoader().loadSceneFromFile(GModel.MediaDir + "\\MeshCreator\\Meshes\\Vegetacion\\Planta\\Planta-TgcScene.xml").Meshes[0];

            planta2 = new TgcSceneLoader().loadSceneFromFile(GModel.MediaDir + "\\MeshCreator\\Meshes\\Vegetacion\\Planta2\\Planta2-TgcScene.xml").Meshes[0];

            planta3 = new TgcSceneLoader().loadSceneFromFile(GModel.MediaDir + "\\MeshCreator\\Meshes\\Vegetacion\\Planta3\\Planta3-TgcScene.xml").Meshes[0];

            roca = new TgcSceneLoader().loadSceneFromFile(GModel.MediaDir + "\\MeshCreator\\Meshes\\Vegetacion\\Roca\\Roca-TgcScene.xml").Meshes[0];
        }

        private void generoObjetosEstaticosSueltos()
        {
            barco.Meshes.ForEach(meshBarco => meshBarco.Scale = new TGCVector3(10,10,10));
            barco.Meshes.ForEach(meshBarco => meshBarco.Position = new TGCVector3(-100,0,-100));

            //--------------objetos---------
            coral.Position = new TGCVector3(100, -1000, -435);
            coral.Scale = new TGCVector3(15, 15, 15);

            tiburon = new Tiburon(meshTiburon, GModel);
            //tiburon.Rotation += new TGCVector3(0, FastMath.PI / 2 + FastMath.PI / 4, 0);
            //tiburon.Transform = TGCMatrix.Translation(tiburon.Position) * TGCMatrix.RotationYawPitchRoll(tiburon.Rotation.X, tiburon.Rotation.Y, tiburon.Rotation.Z);
            tiburon.RotateY(FastMath.PI / 2 + FastMath.PI / 4);
            tiburon.Move(new TGCVector3(-650, -100, 1000));

            coralBrain.Position = new TGCVector3(-200, -1000, 340);
            coralBrain.Transform = TGCMatrix.Translation(coralBrain.Position);

            //fish.Position = new TGCVector3(0, -1000, 0);
            //fish.Transform = TGCMatrix.Translation(fish.Position);

            pillarCoral.Position = new TGCVector3(0, -1000, 40);
            pillarCoral.Transform = TGCMatrix.Translation(pillarCoral.Position);

            seaShell.Position = new TGCVector3(500, -1000, 40);
            seaShell.Transform = TGCMatrix.Translation(seaShell.Position);

            spiralWireCoral.Position = new TGCVector3(-50, -1000, 40);
            spiralWireCoral.Transform = TGCMatrix.Translation(spiralWireCoral.Position);

            TGCVector3 escalaTreeCoral = new TGCVector3(10f, 10f, 10f);
            treeCoral.Position = new TGCVector3(-70, -1000, 200);
            treeCoral.Transform = TGCMatrix.Scaling(escalaTreeCoral) * TGCMatrix.Translation(treeCoral.Position);

            //yellowFish.Position = new TGCVector3(50, -500, -20);
            //yellowFish.Transform = TGCMatrix.Translation(yellowFish.Position);

            arbusto.Position = new TGCVector3(70, -1000, -30);
            arbusto.AlphaBlendEnable = true;
            arbusto.Transform = TGCMatrix.Translation(arbusto.Position);

            arbusto2.Position = new TGCVector3(60, -1000, -20);
            arbusto2.Transform = TGCMatrix.Translation(arbusto2.Position);

            pasto.Position = new TGCVector3(50, -1000, -20);
            pasto.Transform = TGCMatrix.Translation(pasto.Position);

            planta.Position = new TGCVector3(40, -1000, -20);
            planta.Transform = TGCMatrix.Translation(planta.Position);

            planta2.Position = new TGCVector3(30, -1000, -20);
            planta2.Transform = TGCMatrix.Translation(planta2.Position);

            planta3.Position = new TGCVector3(20, -1000, -20);
            planta3.Transform = TGCMatrix.Translation(planta3.Position);

            roca.Position = new TGCVector3(10, -1000, -20);
            roca.Transform = TGCMatrix.Translation(roca.Position);
        }

        private void generoObjetosEstaticosEnArray()
        {
            var rows = 6;
            var cols = 6;
            //----------------
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    /*var instance = coralBrain.createMeshInstance(coralBrain.Name + i + "_" + j);
                    instance.Position = new TGCVector3(GModel.GetRandom.Next(-10000, 10000), -1000, GModel.GetRandom.Next(-10000, 10000));
                    instance.Transform = TGCMatrix.Translation(instance.Position);
                    objetosEstaticosEnArray.Add(instance);*/

                    var posicion = new TGCVector3(GModel.GetRandom.Next(-10000, 10000), -1000, GModel.GetRandom.Next(-10000, 10000));
                    var instance = new RecolectableConMesh(coralBrain, new TGCVector3(67, 0, 0), posicion, "Coral Brain");
                    instance.Mesh.Scale = new TGCVector3(10, 10, 10);
                    instance.Mesh.Transform = TGCMatrix.Scaling(instance.Mesh.Scale) * TGCMatrix.Translation(instance.Mesh.Position);
                    objetosRecolectables.Add(instance);

                }

            }
            //----------------
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    var instance = coral.createMeshInstance(coral.Name + i + "_" + j);

                    instance.Position = new TGCVector3(GModel.GetRandom.Next(-10000, 10000), -1000, GModel.GetRandom.Next(-10000, 10000));
                    //escalado random
                    var escalaObjeto = GModel.GetRandom.Next(5, 15);
                    instance.Scale = new TGCVector3(escalaObjeto, escalaObjeto, escalaObjeto);
                    instance.Transform = TGCMatrix.Scaling(instance.Scale) * TGCMatrix.Translation(instance.Position);
                    objetosEstaticosEnArray.Add(instance);
                }

            }
            //-----------------
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    var instance = pillarCoral.createMeshInstance(pillarCoral.Name + i + "_" + j);
                    instance.Position = new TGCVector3(GModel.GetRandom.Next(-10000, 10000), -1000, GModel.GetRandom.Next(-10000, 10000));
                    var escalaObjeto = GModel.GetRandom.Next(5, 15);
                    instance.Scale = new TGCVector3(escalaObjeto, escalaObjeto, escalaObjeto);
                    instance.Transform = TGCMatrix.Scaling(instance.Scale) * TGCMatrix.Translation(instance.Position);
                    objetosEstaticosEnArray.Add(instance);
                }

            }

            //-----------------
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    /*var instance = seaShell.createMeshInstance(seaShell.Name + i + "_" + j);
                    instance.Position = new TGCVector3(GModel.GetRandom.Next(-10000, 10000), -1000, GModel.GetRandom.Next(-10000, 10000));
                    instance.Transform = TGCMatrix.Translation(instance.Position);
                    objetosEstaticosEnArray.Add(instance);*/

                    var posicion = new TGCVector3(GModel.GetRandom.Next(-10000, 10000), -1000, GModel.GetRandom.Next(-10000, 10000));
                    var instance = new RecolectableConMesh(seaShell, new TGCVector3(67, 0, 0), posicion, "Sea Shell");
                    instance.Mesh.Scale = new TGCVector3(2, 2, 2);
                    instance.Mesh.Transform = TGCMatrix.Scaling(instance.Mesh.Scale) * TGCMatrix.Translation(instance.Mesh.Position);
                    objetosRecolectables.Add(instance);
                }

            }
            //-----------------
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    var instance = treeCoral.createMeshInstance(treeCoral.Name + i + "_" + j);
                    instance.Position = new TGCVector3(GModel.GetRandom.Next(-10000, 10000), -1000, GModel.GetRandom.Next(-10000, 10000));
                    var escalaObjeto = GModel.GetRandom.Next(5, 15);
                    instance.Scale = new TGCVector3(escalaObjeto, escalaObjeto, escalaObjeto);
                    instance.Transform = TGCMatrix.Scaling(instance.Scale) * TGCMatrix.Translation(instance.Position);
                    objetosEstaticosEnArray.Add(instance);
                }

            }
            //-----------------
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    var instance = spiralWireCoral.createMeshInstance(spiralWireCoral.Name + i + "_" + j);
                    instance.Position = new TGCVector3(GModel.GetRandom.Next(-10000, 10000), -1000, GModel.GetRandom.Next(-10000, 10000));
                    var escalaObjeto = GModel.GetRandom.Next(5, 15);
                    instance.Scale = new TGCVector3(escalaObjeto, escalaObjeto, escalaObjeto);
                    instance.Transform = TGCMatrix.Scaling(instance.Scale) * TGCMatrix.Translation(instance.Position);
                    objetosEstaticosEnArray.Add(instance);
                }

            }
            //-----------------
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    var instance = arbusto.createMeshInstance(arbusto.Name + i + "_" + j);
                    instance.Position = new TGCVector3(GModel.GetRandom.Next(-10000, 10000), -1000, GModel.GetRandom.Next(-10000, 10000));
                    instance.AlphaBlendEnable = true;
                    var escalaObjeto = GModel.GetRandom.Next(5, 15);
                    instance.Scale = new TGCVector3(escalaObjeto, escalaObjeto, escalaObjeto);
                    instance.Transform = TGCMatrix.Scaling(instance.Scale) * TGCMatrix.Translation(instance.Position);
                    objetosEstaticosEnArray.Add(instance);
                }

            }
            //-----------------
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    var instance = arbusto2.createMeshInstance(arbusto2.Name + i + "_" + j);
                    instance.Position = new TGCVector3(GModel.GetRandom.Next(-10000, 10000), -1000, GModel.GetRandom.Next(-10000, 10000));
                    instance.AlphaBlendEnable = true;
                    var escalaObjeto = GModel.GetRandom.Next(5, 15);
                    instance.Scale = new TGCVector3(escalaObjeto, escalaObjeto, escalaObjeto);
                    instance.Transform = TGCMatrix.Scaling(instance.Scale) * TGCMatrix.Translation(instance.Position);
                    objetosEstaticosEnArray.Add(instance);
                }

            }
            //-----------------
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    var instance = pasto.createMeshInstance(pasto.Name + i + "_" + j);
                    instance.Position = new TGCVector3(GModel.GetRandom.Next(-10000, 10000), -1000, GModel.GetRandom.Next(-10000, 10000));
                    instance.AlphaBlendEnable = true;
                    var escalaObjeto = GModel.GetRandom.Next(5, 15);
                    instance.Scale = new TGCVector3(escalaObjeto, escalaObjeto, escalaObjeto);
                    instance.Transform = TGCMatrix.Scaling(instance.Scale) * TGCMatrix.Translation(instance.Position);
                    objetosEstaticosEnArray.Add(instance);
                }

            }
            //-----------------
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    var instance = planta.createMeshInstance(planta.Name + i + "_" + j);
                    instance.Position = new TGCVector3(GModel.GetRandom.Next(-10000, 10000), -1000, GModel.GetRandom.Next(-10000, 10000));
                    var escalaObjeto = GModel.GetRandom.Next(5, 15);
                    instance.Scale = new TGCVector3(escalaObjeto, escalaObjeto, escalaObjeto);
                    instance.Transform = TGCMatrix.Scaling(instance.Scale) * TGCMatrix.Translation(instance.Position);
                    objetosEstaticosEnArray.Add(instance);
                }

            }
            //-----------------
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    var instance = planta2.createMeshInstance(planta2.Name + i + "_" + j);
                    instance.Position = new TGCVector3(GModel.GetRandom.Next(-10000, 10000), -1000, GModel.GetRandom.Next(-10000, 10000));
                    var escalaObjeto = GModel.GetRandom.Next(5, 15);
                    instance.Scale = new TGCVector3(escalaObjeto, escalaObjeto, escalaObjeto);
                    instance.Transform = TGCMatrix.Scaling(instance.Scale) * TGCMatrix.Translation(instance.Position);
                    objetosEstaticosEnArray.Add(instance);
                }

            }
            //-----------------
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    var instance = planta3.createMeshInstance(planta3.Name + i + "_" + j);
                    instance.Position = new TGCVector3(GModel.GetRandom.Next(-10000, 10000), -1000, GModel.GetRandom.Next(-10000, 10000));
                    var escalaObjeto = GModel.GetRandom.Next(5, 15);
                    instance.Scale = new TGCVector3(escalaObjeto, escalaObjeto, escalaObjeto);
                    instance.Transform = TGCMatrix.Scaling(instance.Scale) * TGCMatrix.Translation(instance.Position);
                    objetosEstaticosEnArray.Add(instance);
                }

            }
            //-----------------
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    var instance = roca.createMeshInstance(roca.Name + i + "_" + j);
                    instance.Position = new TGCVector3(GModel.GetRandom.Next(-10000, 10000), -1000, GModel.GetRandom.Next(-10000, 10000));
                    var escalaObjeto = GModel.GetRandom.Next(5, 15);
                    instance.Scale = new TGCVector3(escalaObjeto, escalaObjeto, escalaObjeto);
                    instance.Transform = TGCMatrix.Scaling(instance.Scale) * TGCMatrix.Translation(instance.Position);
                    objetosEstaticosEnArray.Add(instance);
                }

            }
        }

        private void cargoSkybox()
        {
            skybox = new TgcSkyBox();
            skybox.Center = TGCVector3.Empty;
            skybox.Size = new TGCVector3(10000, 10000, 10000);

            var texturesPath = GModel.MediaDir + "Texturas\\SkyBox\\";

            skybox.setFaceTexture(TgcSkyBox.SkyFaces.Up, texturesPath + "lostatseaday_up.jpg");
            skybox.setFaceTexture(TgcSkyBox.SkyFaces.Down, texturesPath + "lostatseaday_dn.jpg");
            skybox.setFaceTexture(TgcSkyBox.SkyFaces.Left, texturesPath + "lostatseaday_lf.jpg");
            skybox.setFaceTexture(TgcSkyBox.SkyFaces.Right, texturesPath + "lostatseaday_rt.jpg");
            skybox.setFaceTexture(TgcSkyBox.SkyFaces.Back, texturesPath + "lostatseaday_ft.jpg");
            skybox.setFaceTexture(TgcSkyBox.SkyFaces.Front, texturesPath + "lostatseaday_bk.jpg");
            skybox.SkyEpsilon = 25f;
            skybox.Init();
        }

        private void cargoHeightmap()
        {
            terreno = new TgcSimpleTerrain();
            //var path = MediaDir + "Texturas\\Heighmaps\\heighmap.jpg";
            var path = GModel.MediaDir + "Texturas\\Heighmaps\\heighmap1.jpg";
            //var textu = MediaDir + "Texturas\\Grass.jpg";
            var textu = GModel.MediaDir + "Texturas\\mountain.jpg";
            currentScaleXZ = 100f;
            currentScaleY = 160f;
            //terreno.loadHeightmap(path, currentScaleXZ, currentScaleY, new TGCVector3(0, -130, 0));
            terreno.loadHeightmap(path, currentScaleXZ, currentScaleY, new TGCVector3(0, -195, 0));
            terreno.loadTexture(textu);
            terreno.AlphaBlendEnable = true;
        }

        
        private void cargoPisos()
        {
            var aguaTextura = TgcTexture.createTexture(D3DDevice.Instance.Device, GModel.MediaDir + "Texturas\\agua20.jpg");
            agua = new TgcPlane(new TGCVector3(-20000, 0, -20000), new TGCVector3(40000, 0, 40000), TgcPlane.Orientations.XZplane, aguaTextura);

            var pisoTextura = TgcTexture.createTexture(D3DDevice.Instance.Device, GModel.MediaDir + "Texturas\\seabed.jpg");
            piso = new TgcPlane(new TGCVector3(-20000, -1000, -20000), new TGCVector3(40000, 0, 40000), TgcPlane.Orientations.XZplane, pisoTextura);
        }

        private void cargoMusica()
        {
            musica.FileName = GModel.MediaDir + "\\Music\\AbandonShip.mp3";
            sonidoTiburonCerca.loadSound(GModel.MediaDir + "\\Music\\SharkNear.wav", GModel.DirectSound.DsDevice);
        }

        private void generoMetales()
        {
            var texturaOro = TgcTexture.createTexture(GModel.MediaDir + "\\Texturas\\oro.jpg");
            var texturaRubi = TgcTexture.createTexture(GModel.MediaDir + "\\Texturas\\ruby.jpg");
            var texturaPlatino = TgcTexture.createTexture(GModel.MediaDir + "\\Texturas\\platinum.jpg");

            for (int i = 0; i < 16; i++)
            {
                for (int j = 0; j < 22; j++)
                {
                    var side = GModel.GetRandom.Next(50, 75);
                    var tamanio = new TGCVector3(side, side / 4, side / 2);
                    var posicion = new TGCVector3(GModel.GetRandom.Next(-10000, 10000), -1000 + side / 8, GModel.GetRandom.Next(-10000, 10000));
                    var instance = new RecolectableConTextura(texturaOro, tamanio, posicion, "Oro");
                    objetosRecolectables.Add(instance);
                }

            }

            for (int i = 0; i < 12; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    var side = GModel.GetRandom.Next(50, 75);
                    var tamanio = new TGCVector3(side, side, side);
                    var posicion = new TGCVector3(GModel.GetRandom.Next(-10000, 10000), -1000 + side / 2, GModel.GetRandom.Next(-10000, 10000));
                    var instance = new RecolectableConTextura(texturaRubi, tamanio, posicion, "Ruby");
                    objetosRecolectables.Add(instance);
                }

            }

            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    var side = GModel.GetRandom.Next(50, 75);
                    var tamanio = new TGCVector3(side, side, side);
                    var posicion = new TGCVector3(GModel.GetRandom.Next(-10000, 10000), -1000 + side / 2, GModel.GetRandom.Next(-10000, 10000));
                    var instance = new RecolectableConTextura(texturaPlatino, tamanio, posicion, "Platino");
                    objetosRecolectables.Add(instance);
                }
            }

        }

        public void detectarColision(TgcBoundingCylinder cilindro)
        {
            List<ObjetoRecolectable> objetosEnColision = objetosRecolectables.Where(objeto => objeto.colisionaCon(cilindro)).ToList();
            if (objetosEnColision.Any())
            {
                cilindro.setRenderColor(Color.Red);
                objetoAMostrar = objetosEnColision.First();
                if (GModel.Input.keyPressed(Key.E))
                {
                    GModel.Personaje.Inventario.agregaObjeto(new ObjetoInventario(objetoAMostrar.Nombre, 1));
                    objetosRecolectables.Remove(objetoAMostrar);
                    objetoAMostrar = null;
                }
            }
            else
            {
                cilindro.setRenderColor(Color.LimeGreen);
                objetoAMostrar = null;
            }
        }

        public void hacerSonarTiburonCerca()
        {
            if (musica.getStatus().Equals(TgcMp3Player.States.Playing))
            {
                musica.pause();
            }

            sonidoTiburonCerca.play(true);
        }

        public void detenerSonidoTiburonCerca()
        {
            if (musica.getStatus().Equals(TgcMp3Player.States.Paused))
            {
                musica.resume();
            }

            sonidoTiburonCerca.stop();
        }

        public float MovementSpeed { get => MOVEMENT_SPEED; }

        public TgcScene Barco { get => barco; }

        public TgcMesh Workbench { get => workbench; }
    }
}
