using System.Windows.Forms;

namespace WorkTimer
{
    /// <summary>
    /// A custom Panel control with double buffering enabled to prevent flicker
    /// during frequent repainting operations.
    /// </summary>
    public class DoubleBufferedPanel : Panel
    {
        public DoubleBufferedPanel()
        {
            // Activate double buffering to prevent flickering.
            this.SetStyle(ControlStyles.AllPaintingInWmPaint |
                          ControlStyles.UserPaint |
                          ControlStyles.OptimizedDoubleBuffer,
                          true);

            // Ensure the control is redrawn when it is resized.
            this.ResizeRedraw = true;
        }
    }
}
