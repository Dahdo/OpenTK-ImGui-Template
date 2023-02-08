using System.Runtime.InteropServices;
using NuklearSharp;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace TemplateProject.NuklearUtils;

public unsafe class NuklearController
{
    private Nuklear.nk_context Context;
    private Window* Window;
    private Shader Shader;
    
    public NuklearController()
    {
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
    }

    public void Render()
    {
    }
}