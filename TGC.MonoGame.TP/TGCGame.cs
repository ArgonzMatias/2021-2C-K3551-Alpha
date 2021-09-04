﻿using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace TGC.MonoGame.TP
{
    /// <summary>
    ///     Esta es la clase principal  del juego.
    ///     Inicialmente puede ser renombrado o copiado para hacer más ejemplos chicos, en el caso de copiar para que se
    ///     ejecute el nuevo ejemplo deben cambiar la clase que ejecuta Program <see cref="Program.Main()" /> linea 10.
    /// </summary>
    public class TGCGame : Game
    {
        public const string ContentFolder3D = "Models/";
        public const string ContentFolderEffects = "Effects/";
        public const string ContentFolderMusic = "Music/";
        public const string ContentFolderSounds = "Sounds/";
        public const string ContentFolderSpriteFonts = "SpriteFonts/";
        public const string ContentFolderTextures = "Textures/";

        /// <summary>
        ///     Constructor del juego.
        /// </summary>
        public TGCGame()
        {
            // Maneja la configuracion y la administracion del dispositivo grafico.
            Graphics = new GraphicsDeviceManager(this);
            // Descomentar para que el juego sea pantalla completa.
            // Graphics.IsFullScreen = true;
            // Carpeta raiz donde va a estar toda la Media.
            Content.RootDirectory = "Content";
            // Hace que el mouse sea visible.
            IsMouseVisible = true;
        }

        private GraphicsDeviceManager Graphics { get; }
        private SpriteBatch SpriteBatch { get; set; }
        private Model Cartel { get; set; }
        private Model Esfera { get; set; }
        private Effect Effect { get; set; }
        private BasicEffect BasicEffect { get; set; }
        private float Rotation { get; set; }
        private Matrix World { get; set; }
        private Matrix View { get; set; }
        private Matrix Projection { get; set; }
        public VertexBuffer Vertices { get; private set; }
        public IndexBuffer Indices { get; private set; }

        /// <summary>
        ///     Se llama una sola vez, al principio cuando se ejecuta el ejemplo.
        ///     Escribir aqui el codigo de inicializacion: el procesamiento que podemos pre calcular para nuestro juego.
        /// </summary>
        protected override void Initialize()
        {
            // La logica de inicializacion que no depende del contenido se recomienda poner en este metodo.
            // Seria hasta aca.

            // Configuramos nuestras matrices de la escena.
            World = Matrix.Identity;
            View = Matrix.CreateLookAt(Vector3.UnitZ * 150, Vector3.Zero, Vector3.Up);
            Projection =
                Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, GraphicsDevice.Viewport.AspectRatio, 1, 250);

            // Setup our basic effect
            BasicEffect = new BasicEffect(GraphicsDevice)
            {
                World = World,
                View = View,
                Projection = Projection,
                VertexColorEnabled = true
            };

            float yPositionFloor = -20f;
            float xScaleFloor = 50f;
            float zScaleFloor = 50f;

            // Array of vertex positions and colors.
            var triangleVertices = new[]
            {
                new VertexPositionColor(new Vector3(-1f * xScaleFloor, yPositionFloor, 1f * zScaleFloor), Color.Blue),
                new VertexPositionColor(new Vector3(-1f * xScaleFloor, yPositionFloor, -1f * zScaleFloor), Color.Red),
                new VertexPositionColor(new Vector3(1f * xScaleFloor, yPositionFloor, -1f * zScaleFloor), Color.Green),
                new VertexPositionColor(new Vector3(1f * xScaleFloor, yPositionFloor, 1f * zScaleFloor), Color.Yellow)
            };

            Vertices = new VertexBuffer(GraphicsDevice, VertexPositionColor.VertexDeclaration, triangleVertices.Length,
                BufferUsage.WriteOnly);
            Vertices.SetData(triangleVertices);

            // Array of indices
            var triangleIndices = new ushort[]
            {
                0, 1, 2, 0, 2, 3
            };

            Indices = new IndexBuffer(GraphicsDevice, IndexElementSize.SixteenBits, triangleIndices.Length, BufferUsage.None);
            Indices.SetData(triangleIndices);

            base.Initialize();
        }

        /// <summary>
        ///     Se llama una sola vez, al principio cuando se ejecuta el ejemplo, despues de Initialize.
        ///     Escribir aqui el codigo de inicializacion: cargar modelos, texturas, estructuras de optimizacion, el procesamiento
        ///     que podemos pre calcular para nuestro juego.
        /// </summary>
        protected override void LoadContent()
        {
            // Aca es donde deberiamos cargar todos los contenido necesarios antes de iniciar el juego.
            SpriteBatch = new SpriteBatch(GraphicsDevice);

            // Cargo el Cartel
            Cartel = Content.Load<Model>(ContentFolder3D + "Marbel/Sign/StreetSign");
            //Cargo la esfera
            Esfera = Content.Load<Model>(ContentFolder3D + "Marbel/Pelota/pelota");

            // Cargo un efecto basico propio declarado en el Content pipeline.
            // En el juego no pueden usar BasicEffect de MG, deben usar siempre efectos propios.
            Effect = Content.Load<Effect>(ContentFolderEffects + "BasicShader");

            // Asigno el efecto que cargue a cada parte del mesh.
            // Un modelo puede tener mas de 1 mesh internamente.
            // Un mesh puede tener mas de 1 mesh part (cada 1 puede tener su propio efecto).
            //mesh Cartel
            foreach (var mesh in Cartel.Meshes)            
            foreach (var meshPart in mesh.MeshParts)
                meshPart.Effect = Effect;
            //mesh esfera
            foreach (var mesh in Esfera.Meshes)
                foreach (var meshPart in mesh.MeshParts)
                    meshPart.Effect = Effect;
            base.LoadContent();
        }

        /// <summary>
        ///     Se llama en cada frame.
        ///     Se debe escribir toda la logica de computo del modelo, asi como tambien verificar entradas del usuario y reacciones
        ///     ante ellas.
        /// </summary>
        protected override void Update(GameTime gameTime)
        {
            // Aca deberiamos poner toda la logica de actualizacion del juego.

            // Capturar Input teclado
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                //Salgo del juego.
                Exit();
            // Basado en el tiempo que paso se va generando una rotacion.
            Rotation += Convert.ToSingle(gameTime.ElapsedGameTime.TotalSeconds);

            base.Update(gameTime);
        }

        /// <summary>
        ///     Se llama cada vez que hay que refrescar la pantalla.
        ///     Escribir aqui el codigo referido al renderizado.
        /// </summary>
        protected override void Draw(GameTime gameTime)
        {
            // Aca deberiamos poner toda la logia de renderizado del juego.
            GraphicsDevice.Clear(Color.Black);

            // Para dibujar le modelo necesitamos pasarle informacion que el efecto esta esperando.
            Effect.Parameters["View"].SetValue(View);
            Effect.Parameters["Projection"].SetValue(Projection);
            
            var rotationMatrix = Matrix.CreateRotationY(Rotation);

            //Se agrega el cartel
            foreach (var mesh in Cartel.Meshes)
            {
                World =mesh.ParentBone.Transform * Matrix.CreateScale(0.1f)  * Matrix.CreateRotationY(Rotation) * Matrix.CreateTranslation(new Vector3(50f, 0f, 0f));
                    //asigno colo verde amarillo 
                Effect.Parameters["DiffuseColor"].SetValue(Color.GreenYellow.ToVector3());
                Effect.Parameters["World"].SetValue(World);
                mesh.Draw();
            }
            //Se agrega la esfera
            foreach (var mesh in Esfera.Meshes)
            {
                World = mesh.ParentBone.Transform * Matrix.CreateScale(0.1f) * Matrix.CreateTranslation(new Vector3(-50f, 0f, 0f));
                Effect.Parameters["World"].SetValue(World);
                   //asigno colo rojo
                Effect.Parameters["DiffuseColor"].SetValue(Color.Red.ToVector3());
                mesh.Draw();
            }

            // Para el piso
            // Set our vertex buffer.
            GraphicsDevice.SetVertexBuffer(Vertices);

            // Set our index buffer
            GraphicsDevice.Indices = Indices;

            foreach (var pass in BasicEffect.CurrentTechnique.Passes)
            {
                pass.Apply();

                GraphicsDevice.DrawIndexedPrimitives(
                    // We’ll be rendering one triangles.
                    PrimitiveType.TriangleList,
                    // The offset, which is 0 since we want to start at the beginning of the Vertices array.
                    0,
                    // The start index in the Vertices array.
                    0,
                    // The number of triangles to draw.
                    2);
            }
        }

        /// <summary>
        ///     Libero los recursos que se cargaron en el juego.
        /// </summary>
        protected override void UnloadContent()
        {
            // Libero los recursos.
            Content.Unload();

            base.UnloadContent();
        }
    }
}

// idea obstaculo: El cartel puede ser un obstaculo que si lo hacemos rotar el jugador tendria que evitarlo