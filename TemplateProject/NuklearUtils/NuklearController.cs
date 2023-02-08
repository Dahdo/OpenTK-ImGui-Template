using System.Runtime.InteropServices;
using NuklearSharp;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace TemplateProject.NuklearUtils;

public unsafe class NuklearController : IDisposable
{
    private Nuklear.nk_context Context;
    private Window* Window;
    private Shader Shader;
    
    public NuklearController(GameWindow window)
    {
        Window = window.WindowPtr;
        Context = new Nuklear.nk_context();
        HandleClipboard();
        Shader = new Shader(("nuklear.vert", ShaderType.VertexShader), ("nuklear.frag", ShaderType.FragmentShader));
    }

    private void HandleClipboard()
    {
        Context.clip.copy = (Nuklear.nk_handle handle, char* text, int length) =>
        {
            string clipboard = Marshal.PtrToStringAnsi(new IntPtr(text), length);
            GLFW.SetClipboardString(Window, clipboard);
        };
        Context.clip.paste = (Nuklear.nk_handle handle, Nuklear.nk_text_edit edit) =>
        {
            string clipboard = GLFW.GetClipboardString(Window);
            if (clipboard != null)
            {
                IntPtr ptr = Marshal.StringToHGlobalAnsi(clipboard);
                Nuklear.nk_textedit_paste(edit, (char*)ptr.ToPointer(), clipboard.Length);
            }  
        };
        Context.clip.userdata = new Nuklear.nk_handle();
    }

    public void Update(KeyboardState keyboardState, MouseState mouse)
    {
        Nuklear.nk_input_begin(Context);
        
        Nuklear.nk_input_button(Context, Nuklear.NK_BUTTON_LEFT, (int)mouse.X, (int)mouse.Y, mouse.IsButtonDown(MouseButton.Left).CompareTo(false));
        Nuklear.nk_input_button(Context, Nuklear.NK_BUTTON_MIDDLE, (int)mouse.X, (int)mouse.Y, mouse.IsButtonDown(MouseButton.Left).CompareTo(false));
        Nuklear.nk_input_button(Context, Nuklear.NK_BUTTON_RIGHT, (int)mouse.X, (int)mouse.Y, mouse.IsButtonDown(MouseButton.Left).CompareTo(false));
        // TODO: NK_BUTTON_DOUBLE
        
        Nuklear.nk_input_end(Context);
    }

    public void Render()
    {
        
    }

    public void Dispose()
    {
        Shader.Dispose();
        Nuklear.nk_free(Context);
    }
}