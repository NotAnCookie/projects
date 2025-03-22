using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bezier3D
{
    public static class ControlExtensions
    {
        // double buffering dla panelu aby nie migał
        // potrzebne bo nie można ustawić tego w prost tak jak dla formy
        public static void EnableDoubleBuffering(this Control control)
        {
            System.Reflection.PropertyInfo doubleBufferPropertyInfo = control.GetType().GetProperty("DoubleBuffered", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            doubleBufferPropertyInfo?.SetValue(control, true, null);
        }
    }
}
