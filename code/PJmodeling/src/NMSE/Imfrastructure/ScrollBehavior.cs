using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using ListBox = System.Windows.Controls.ListBox;
using TextBox = System.Windows.Controls.TextBox;

namespace NMSE.Imfrastructure;

public static class ScrollBehavior {
  public enum Kind {
    None,
    AutoScrollToEnd
  }

  #region attached Kind Kind

  public static readonly DependencyProperty KindProperty = DependencyProperty.RegisterAttached("Kind", typeof(Kind), typeof(ScrollBehavior), new FrameworkPropertyMetadata(default(Kind), KindPropertyChanged));

  public static void SetKind(DependencyObject d, Kind value) => d.SetValue(KindProperty, value);
  public static Kind GetKind(DependencyObject d) => (Kind)d.GetValue(KindProperty);

  static void KindPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
    switch (d) {
      case ListBox listBox: {
        if (listBox.Items.SourceCollection is not INotifyCollectionChanged ncc)
          throw new NotSupportedException("INotifyCollectionChanged required.");
        ncc.CollectionChanged -= listBoxAutoScrollToEnd;
        if (e.NewValue is Kind.AutoScrollToEnd)
          ncc.CollectionChanged += listBoxAutoScrollToEnd;
        break;

        void listBoxAutoScrollToEnd(object sender, NotifyCollectionChangedEventArgs args) {
          if (args.NewItems?.OfType<object>().Any() is true)
            listBox.Dispatcher.InvokeAsync(() => listBox.ScrollIntoView(args.NewItems.OfType<object>().Last()), DispatcherPriority.Background);
        }
      } case TextBox textBox: {
        textBox.TextChanged -= textBoxAutoScrollToEnd;
        if (e.NewValue is Kind.AutoScrollToEnd)
          textBox.TextChanged += textBoxAutoScrollToEnd;
        break;

        void textBoxAutoScrollToEnd(object sender, TextChangedEventArgs args) { textBox.Dispatcher.InvokeAsync(() => textBox.ScrollToEnd(), DispatcherPriority.Background); }
      }
    }
  }

  #endregion // attached Kind Kind
}
