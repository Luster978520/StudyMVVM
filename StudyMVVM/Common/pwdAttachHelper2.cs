using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace StudyMVVM.Common
{
    public class pwdAttachHelper2
    {


        #region  CustPwd改变了，Password跟着改变
        public static string GetCustPwd(DependencyObject obj)  //附加属性DependencyObject obj对象直接作为参数进行传递，obj是附加属性的宿主对象
        {
            return (string)obj.GetValue(CustPwdProperty);
        }

        public static void SetCustPwd(DependencyObject obj, string value)
        {
            obj.SetValue(CustPwdProperty, value);
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CustPwdProperty =
            DependencyProperty.RegisterAttached("CustPwd", typeof(string), typeof(pwdAttachHelper2), new PropertyMetadata("",OnCustPwdChangedCallback));

        private static void OnCustPwdChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            string newPwd = (string)e.NewValue;

            if (newPwd != null)
            {
                if (d is PasswordBox pwdBox)
                {
                    if (e.NewValue.ToString() != pwdBox.Password)  //界面输入的密码和绑定的密码不一样，才去赋值，避免死循环
                    {
                        pwdBox.Password = newPwd;
                    }
                }
            }
        }
        #endregion


        #region  password 改变了，CustPwd跟着改变,想办法订阅PasswordChanged事件




        public static bool GetIsOpen(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsOpenProperty);
        }

        public static void SetIsOpen(DependencyObject obj, bool value)
        {
            obj.SetValue(IsOpenProperty, value);
        }

        // Using a DependencyProperty as the backing store for IsOpen.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsOpenProperty =
            DependencyProperty.RegisterAttached("IsOpen", typeof(bool), typeof(pwdAttachHelper2), new PropertyMetadata(false, OnIsOpenChangedCallback));

        private static void OnIsOpenChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is PasswordBox pwdBox)
            {
                pwdBox.PasswordChanged -= OnPasswordChanged;
                pwdBox.PasswordChanged += OnPasswordChanged;
            }
        }

        private static void OnPasswordChanged(object sender, RoutedEventArgs e)
        {
            //前端的密码发生变换，需要给我们的自定义的密码赋值   
            if (sender is PasswordBox pwdBox)
            {
                SetCustPwd(pwdBox, pwdBox.Password);
            }
            
        }










        #endregion
    }
}
