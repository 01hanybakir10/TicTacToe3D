using System;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

public class Shader : IDisposable
{
    public readonly int Handle;
    private bool disposedValue = false;  // To detect redundant calls

    public Shader(string vertexPath, string fragmentPath)
    {
        // Load shader source code
        string vertexShaderSource = System.IO.File.ReadAllText(vertexPath);
        string fragmentShaderSource = System.IO.File.ReadAllText(fragmentPath);

        // Compile vertex shader
        int vertexShader = GL.CreateShader(ShaderType.VertexShader);
        GL.ShaderSource(vertexShader, vertexShaderSource);
        GL.CompileShader(vertexShader);
        CheckShaderCompileErrors(vertexShader);

        // Compile fragment shader
        int fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
        GL.ShaderSource(fragmentShader, fragmentShaderSource);
        GL.CompileShader(fragmentShader);
        CheckShaderCompileErrors(fragmentShader);

        // Link shaders into a program
        Handle = GL.CreateProgram();
        GL.AttachShader(Handle, vertexShader);
        GL.AttachShader(Handle, fragmentShader);
        GL.LinkProgram(Handle);

        // Clean up shaders as they're no longer needed once linked
        GL.DeleteShader(vertexShader);
        GL.DeleteShader(fragmentShader);
    }

    public void Use()
    {
        GL.UseProgram(Handle);
    }

    public void SetMatrix4(string name, Matrix4 matrix)
    {
        int location = GL.GetUniformLocation(Handle, name);
        GL.UniformMatrix4(location, false, ref matrix);
    }

    // Check for shader compile errors
    private void CheckShaderCompileErrors(int shader)
    {
        GL.GetShader(shader, ShaderParameter.CompileStatus, out int success);
        if (success == 0)
        {
            string infoLog = GL.GetShaderInfoLog(shader);
            throw new Exception($"Error compiling shader: {infoLog}");
        }
    }

    // Implementation of the Dispose method
    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                // Dispose managed state (managed objects) if any
            }

            // Delete the OpenGL shader program
            GL.DeleteProgram(Handle);

            disposedValue = true;
        }
    }

    // Public Dispose method to be called by user code
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    // Destructor/finalizer to call Dispose in case it's not explicitly called
    ~Shader()
    {
        Dispose(false);
    }
}
