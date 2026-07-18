using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.SharedKernel.Security
{
    public static class XssDetect
    {
        public static bool IsXssAttack(byte[] Content)
        {
            try
            {
                if (Content == null)
                {
                    return false;
                }
                var str = System.Text.Encoding.Default.GetString(Content);
                if (str.Contains("<script>"))
                {
                    return true;
                }
                if (str.Contains("<applet>"))
                {
                    return true;
                }   
                if (str.Contains("<html>"))
                {
                    return true;
                }
                if (str.Contains("<frame>"))
                {
                    return true;
                }
                if (str.Contains("<img>"))
                {
                    return true;
                } 
                if (str.Contains("<embed>"))
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return false;
        }
    }
}
