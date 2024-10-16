using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Graphics.OpenGL4;

class TicTacToe3D : GameWindow
{
    private int _vao;
    private int _vbo;
    private Shader _shader;

    private float[] _boardVertices = {
        // Vertical lines
        -0.66f,  1.0f, 0.0f,  -0.66f, -1.0f, 0.0f,  // Left vertical
         0.66f,  1.0f, 0.0f,   0.66f, -1.0f, 0.0f,  // Right vertical

        // Horizontal lines
        -1.0f,  0.66f, 0.0f,   1.0f,  0.66f, 0.0f,  // Top horizontal
        -1.0f, -0.66f, 0.0f,   1.0f, -0.66f, 0.0f   // Bottom horizontal
    };

    private Matrix4 _model, _view, _projection;

    public TicTacToe3D(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings)
        : base(gameWindowSettings, nativeWindowSettings)
    {
    }

    protected override void OnLoad()
    {
        base.OnLoad();

        GL.ClearColor(0.1f, 0.1f, 0.1f, 1.0f);  // Background color
        GL.Enable(EnableCap.DepthTest);  // Enable depth testing for 3D rendering

        // Load and compile shaders
        _shader = new Shader("vertexShader.glsl", "fragmentShader.glsl");

        // Generate VAO and VBO
        _vao = GL.GenVertexArray();
        _vbo = GL.GenBuffer();

        // Bind the VAO
        GL.BindVertexArray(_vao);

        // Bind the VBO and upload the board vertices
        GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
        GL.BufferData(BufferTarget.ArrayBuffer, _boardVertices.Length * sizeof(float), _boardVertices, BufferUsageHint.StaticDraw);

        // Configure vertex attributes
        GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
        GL.EnableVertexAttribArray(0);

        // Unbind VBO and VAO
        GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        GL.BindVertexArray(0);

        // Set up the projection matrix (perspective view)
        _projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(45.0f), Size.X / (float)Size.Y, 0.1f, 100.0f);

        // Set up the camera view matrix (look at the board from a specific point)
        _view = Matrix4.LookAt(new Vector3(0.0f, 0.0f, 5.0f), Vector3.Zero, Vector3.UnitY);
    }

    protected override void OnRenderFrame(FrameEventArgs e)
    {
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

        _shader.Use();

        // Update the model matrix (rotation)
        _model = Matrix4.CreateRotationY(MathHelper.DegreesToRadians(20.0f * (float)e.Time));

        // Pass the matrices to the shader
        _shader.SetMatrix4("model", _model);
        _shader.SetMatrix4("view", _view);
        _shader.SetMatrix4("projection", _projection);

        // Bind the VAO and draw the board (lines)
        GL.BindVertexArray(_vao);
        GL.DrawArrays(PrimitiveType.Lines, 0, _boardVertices.Length / 3);

        SwapBuffers();
    }

    protected override void OnUnload()
    {
        // Clean up resources
        GL.DeleteBuffer(_vbo);
        GL.DeleteVertexArray(_vao);
        _shader.Dispose();

        base.OnUnload();
    }

    static void Main(string[] args)
    {
        var gameWindowSettings = GameWindowSettings.Default;
        var nativeWindowSettings = new NativeWindowSettings()
        {
            Size = new Vector2i(800, 600),
            Title = "3D Tic-Tac-Toe (Player vs AI)"
        };

        using (TicTacToe3D game = new TicTacToe3D(gameWindowSettings, nativeWindowSettings))
        {
            game.Run();
        }
    }
}
