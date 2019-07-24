using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace Common.WPF
{
    public class FormatBindableTextBlock : TextBlock
    {
        public ObservableCollection<Inline> InlineList
        {
            get { return (ObservableCollection<Inline>)GetValue(InlineListProperty); }
            set { SetValue(InlineListProperty, value); }
        }

        public static readonly DependencyProperty InlineListProperty =
            DependencyProperty.Register("InlineList", typeof(ObservableCollection<Inline>), typeof(FormatBindableTextBlock), new UIPropertyMetadata(null, OnPropertyChanged));

        private static void OnPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            FormatBindableTextBlock textBlock = sender as FormatBindableTextBlock;
            ObservableCollection<Inline> list = e.NewValue as ObservableCollection<Inline>;
            if (textBlock != null && list != null)
            {
                textBlock.Inlines.Clear();
                textBlock.Inlines.AddRange(list);
            }
        }
    }
}
