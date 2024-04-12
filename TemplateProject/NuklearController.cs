using System.Runtime.InteropServices;
using NuklearSharp;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace TemplateProject;

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
        Context.clip.copy = (handle, text, length) =>
        {
            string clipboard = Marshal.PtrToStringAnsi(new IntPtr(text), length);
            GLFW.SetClipboardString(Window, clipboard);
        };
        Context.clip.paste = (handle, edit) =>
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

    public void Update(KeyboardState keyboard, MouseState mouse)
    {
        Nuklear.nk_input_begin(Context);
        
        Nuklear.nk_input_key(Context, Nuklear.NK_KEY_DEL, Convert.ToInt32(keyboard.IsKeyDown(Keys.Delete)));
        Nuklear.nk_input_key(Context, Nuklear.NK_KEY_ENTER, Convert.ToInt32(keyboard.IsKeyDown(Keys.Enter)));
        Nuklear.nk_input_key(Context, Nuklear.NK_KEY_TAB, Convert.ToInt32(keyboard.IsKeyDown(Keys.Tab)));
        Nuklear.nk_input_key(Context, Nuklear.NK_KEY_BACKSPACE, Convert.ToInt32(keyboard.IsKeyDown(Keys.Backspace)));
        Nuklear.nk_input_key(Context, Nuklear.NK_KEY_UP, Convert.ToInt32(keyboard.IsKeyDown(Keys.Up)));
        Nuklear.nk_input_key(Context, Nuklear.NK_KEY_DOWN, Convert.ToInt32(keyboard.IsKeyDown(Keys.Down)));
        Nuklear.nk_input_key(Context, Nuklear.NK_KEY_TEXT_START, Convert.ToInt32(keyboard.IsKeyDown(Keys.Home)));
        Nuklear.nk_input_key(Context, Nuklear.NK_KEY_TEXT_END, Convert.ToInt32(keyboard.IsKeyDown(Keys.End)));
        Nuklear.nk_input_key(Context, Nuklear.NK_KEY_SCROLL_START, Convert.ToInt32(keyboard.IsKeyDown(Keys.Home)));
        Nuklear.nk_input_key(Context, Nuklear.NK_KEY_SCROLL_END, Convert.ToInt32(keyboard.IsKeyDown(Keys.End)));
        Nuklear.nk_input_key(Context, Nuklear.NK_KEY_SCROLL_DOWN, Convert.ToInt32(keyboard.IsKeyDown(Keys.PageDown)));
        Nuklear.nk_input_key(Context, Nuklear.NK_KEY_SCROLL_UP, Convert.ToInt32(keyboard.IsKeyDown(Keys.PageUp)));
        Nuklear.nk_input_key(Context, Nuklear.NK_KEY_SHIFT, Convert.ToInt32(keyboard.IsKeyDown(Keys.LeftShift)||
                                                                                 keyboard.IsKeyDown(Keys.RightShift)));

    if (keyboard.IsKeyDown(Keys.LeftControl) ||
        keyboard.IsKeyDown(Keys.RightControl)) {
        Nuklear.nk_input_key(Context, Nuklear.NK_KEY_COPY, Convert.ToInt32(keyboard.IsKeyDown(Keys.C)));
        Nuklear.nk_input_key(Context, Nuklear.NK_KEY_PASTE, Convert.ToInt32(keyboard.IsKeyDown(Keys.V)));
        Nuklear.nk_input_key(Context, Nuklear.NK_KEY_CUT, Convert.ToInt32(keyboard.IsKeyDown(Keys.X)));
        Nuklear.nk_input_key(Context, Nuklear.NK_KEY_TEXT_UNDO, Convert.ToInt32(keyboard.IsKeyDown(Keys.Z)));
        Nuklear.nk_input_key(Context, Nuklear.NK_KEY_TEXT_REDO, Convert.ToInt32(keyboard.IsKeyDown(Keys.Y)));
        Nuklear.nk_input_key(Context, Nuklear.NK_KEY_TEXT_WORD_LEFT, Convert.ToInt32(keyboard.IsKeyDown(Keys.Left)));
        Nuklear.nk_input_key(Context, Nuklear.NK_KEY_TEXT_WORD_RIGHT, Convert.ToInt32(keyboard.IsKeyDown(Keys.Right)));
        Nuklear.nk_input_key(Context, Nuklear.NK_KEY_TEXT_LINE_START, Convert.ToInt32(keyboard.IsKeyDown(Keys.B)));
        Nuklear.nk_input_key(Context, Nuklear.NK_KEY_TEXT_LINE_END, Convert.ToInt32(keyboard.IsKeyDown(Keys.E)));
        Nuklear.nk_input_key(Context, Nuklear.NK_KEY_TEXT_SELECT_ALL, Convert.ToInt32(keyboard.IsKeyDown(Keys.A)));
    } else {
        Nuklear.nk_input_key(Context, Nuklear.NK_KEY_LEFT, Convert.ToInt32(keyboard.IsKeyDown(Keys.Left)));
        Nuklear.nk_input_key(Context, Nuklear.NK_KEY_RIGHT, Convert.ToInt32(keyboard.IsKeyDown(Keys.Right)));
        Nuklear.nk_input_key(Context, Nuklear.NK_KEY_COPY, 0);
        Nuklear.nk_input_key(Context, Nuklear.NK_KEY_PASTE, 0);
        Nuklear.nk_input_key(Context, Nuklear.NK_KEY_CUT, 0);
        Nuklear.nk_input_key(Context, Nuklear.NK_KEY_SHIFT, 0);
    }
        
        Nuklear.nk_input_motion(Context, (int)mouse.X, (int)mouse.Y);
        
        Nuklear.nk_input_button(Context, Nuklear.NK_BUTTON_LEFT, (int)mouse.X, (int)mouse.Y, mouse.IsButtonDown(MouseButton.Left).CompareTo(false));
        Nuklear.nk_input_button(Context, Nuklear.NK_BUTTON_MIDDLE, (int)mouse.X, (int)mouse.Y, mouse.IsButtonDown(MouseButton.Left).CompareTo(false));
        Nuklear.nk_input_button(Context, Nuklear.NK_BUTTON_RIGHT, (int)mouse.X, (int)mouse.Y, mouse.IsButtonDown(MouseButton.Left).CompareTo(false));
        // TODO: NK_BUTTON_DOUBLE
        
        Nuklear.nk_input_scroll(Context, new Nuklear.nk_vec2{x = mouse.Scroll.X, y = mouse.Scroll.Y});
        
        Nuklear.nk_input_end(Context);
    }
    
    public void PressChar(char keyChar)
    {
        Nuklear.nk_input_char(Context, keyChar);
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