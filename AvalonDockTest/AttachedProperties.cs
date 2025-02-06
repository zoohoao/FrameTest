using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace AvalonDockTest
{
    public static class VisibilityAttachedProperties
    {
        // 获取 IsSpecialVisible 属性
        public static string GetIsSpecialVisible(DependencyObject element)
        {
            return (string)element.GetValue(IsSpecialVisibleProperty);
        }

        // 设置 IsSpecialVisible 属性
        public static void SetIsSpecialVisible(DependencyObject element, string value)
        {
            element.SetValue(IsSpecialVisibleProperty, value);
        }

        // 注册附加属性 IsSpecialVisible
        public static readonly DependencyProperty IsSpecialVisibleProperty =
            DependencyProperty.RegisterAttached(
                "IsSpecialVisible",
                typeof(string),
                typeof(VisibilityAttachedProperties),
                new PropertyMetadata("dddd", OnCallBack, OnChanged));

        private static object OnChanged(DependencyObject d, object baseValue)
        {
            return baseValue;
        }

        private static void OnCallBack(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }
    }
}