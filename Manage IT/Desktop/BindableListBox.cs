using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;

namespace Desktop
{
    using System.Collections;
    using System.Collections.Specialized;
    using System.Windows;
    using System.Windows.Controls;

    public class BindableListBox : ListBox
    {
        public static readonly DependencyProperty BindableSelectedItemsProperty =
            DependencyProperty.Register("BindableSelectedItems", typeof(IList), typeof(BindableListBox), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnBindableSelectedItemsChanged));

        public IList BindableSelectedItems
        {
            get { return (IList)GetValue(BindableSelectedItemsProperty); }
            set { SetValue(BindableSelectedItemsProperty, value); }
        }

        private static void OnBindableSelectedItemsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var listBox = d as BindableListBox;
            if (listBox == null) return;

            listBox.SelectionChanged -= listBox.OnSelectionChangedInternal;

            if (e.OldValue is INotifyCollectionChanged oldCollection)
            {
                oldCollection.CollectionChanged -= listBox.OnCollectionChanged;
            }

            if (e.NewValue is INotifyCollectionChanged newCollection)
            {
                newCollection.CollectionChanged += listBox.OnCollectionChanged;
            }

            listBox.SelectedItems.Clear();

            if (e.NewValue == null)
            {
                return;
            }

            foreach (var item in e.NewValue as IList)
            {
                listBox.SelectedItems.Add(item);
            }

            listBox.SelectionChanged += listBox.OnSelectionChangedInternal;
        }

        private void OnSelectionChangedInternal(object sender, SelectionChangedEventArgs e)
        {
            if (BindableSelectedItems == null) return;

            foreach (var item in e.RemovedItems)
            {
                BindableSelectedItems.Remove(item);
            }

            foreach (var item in e.AddedItems)
            {
                BindableSelectedItems.Add(item);
            }
        }

        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            SelectionChanged -= OnSelectionChangedInternal;

            if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                SelectedItems.Clear();
            }
            else
            {
                if (e.OldItems != null)
                {
                    foreach (var item in e.OldItems)
                    {
                        SelectedItems.Remove(item);
                    }
                }

                if (e.NewItems != null)
                {
                    foreach (var item in e.NewItems)
                    {
                        SelectedItems.Add(item);
                    }
                }
            }

            SelectionChanged += OnSelectionChangedInternal;
        }
    }
}
