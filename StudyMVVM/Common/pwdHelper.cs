using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace StudyMVVM.Common
{
    public class pwdHelper: DependencyObject
    {


        public static string GetpwdTextProperty(DependencyObject obj)
        {
            return (string)obj.GetValue(PwdTextProperty);
        }

        public static void SetpwdTextProperty(DependencyObject obj, string value)
        {
            obj.SetValue(PwdTextProperty, value);
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PwdTextProperty =
            DependencyProperty.RegisterAttached("pwdTextProperty", typeof(string), typeof(pwdHelper), new PropertyMetadata("",onCustPwdChanged));

        private static void onCustPwdChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            string newPwd = (string)e.NewValue;
            if (d is PasswordBox pwdBox)
            {
                if (pwdBox.Password!=newPwd)
                {
                    pwdBox.Password = newPwd;
                }
                
            }
        }

        public static bool GetisOpen(DependencyObject obj)
        {
            return (bool)obj.GetValue(isOpenProperty);
        }

        public static void SetisOpen(DependencyObject obj, bool value)
        {
            obj.SetValue(isOpenProperty, value);
        }

        // Using a DependencyProperty as the backing store for isOpen.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty isOpenProperty =
            DependencyProperty.RegisterAttached("isOpen", typeof(bool), typeof(pwdHelper), new PropertyMetadata(false,OnIsOpenChanged));

        private static void OnIsOpenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is PasswordBox pwdBox)
            {
               pwdBox.PasswordChanged -= PwdBox_PasswordChanged;
              pwdBox.PasswordChanged += PwdBox_PasswordChanged;
            }
        }

        private static void PwdBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (sender is PasswordBox pwdBox)
            {
                
            }
        }
    }
}
