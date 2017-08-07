using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace MVVMToolkit.ViewModels
{
    /// <summary>
    /// Add two properties one for Items collection and other for SelectedItem
    /// </summary>
    /// <typeparam name="T">Type of item</typeparam>
    public abstract class ViewModelCollectionBase<T> : ViewModelBase
    {
        /// <summary>
        /// Items of the collection
        /// </summary>
        public ObservableCollection<T> Items { get; } = new ObservableCollection<T>();

        /// <summary>
        /// Item selected
        /// </summary>
        public T SelectedItem { get => Get(); set => Set(value); }

        /// <summary>
        /// Check if an item is selected
        /// </summary>
        public bool HasSelectedItem => SelectedItem != null;

        /// <summary>
        /// Reset the list of items
        /// </summary>
        /// <param name="items">Items to add to Items Observable Collection</param>
        protected void ResetItems(IEnumerable<T> items)
        {
            Items.Clear();
            Items.Concat(items);
        }

        /// <summary>
        /// Replace the current item by another
        /// </summary>
        /// <param name="oldItem">The current item</param>
        /// <param name="newItem">The new item</param>
        /// <param name="samePlace">If is true the new item is on the same place that the old item otherwise the new item is add to the end</param>
        protected void ReplaceItem(T oldItem, T newItem, bool samePlace = true)
        {
            if (samePlace)
            {
                int index = Items.IndexOf(oldItem);
                Items.RemoveAt(index);
                Items.Insert(index, newItem);
            }
            else
            {
                Items.Remove(oldItem);
                Items.Add(newItem);
            }
        }
    }
}
