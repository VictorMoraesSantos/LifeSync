using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;

namespace LifeSyncApp.Helpers
{
    // Use this instead of ObservableCollection<T> for UI-bound collections that may outlive some subscribers.
    public class SafeObservableCollection<T> : ObservableCollection<T>
    {
        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            try
            {
                base.OnCollectionChanged(e);
            }
            catch (ObjectDisposedException)
            {
                // Subscriber (likely UI) has been disposed — ignore to avoid crashing.
            }
        }

        protected override void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            try
            {
                base.OnPropertyChanged(e);
            }
            catch (ObjectDisposedException)
            {
                // Subscriber (likely UI) has been disposed — ignore.
            }
        }

        /// <summary>
        /// Replaces all items silently (no per-item events) and fires a single Reset.
        /// Prevents BindableLayout duplication glitches caused by Clear+Add event interleaving.
        /// </summary>
        public void ReplaceAll(IEnumerable<T> items)
        {
            var list = items?.ToList() ?? new List<T>();

            Items.Clear();
            foreach (var item in list)
                Items.Add(item);

            OnPropertyChanged(new PropertyChangedEventArgs("Count"));
            OnPropertyChanged(new PropertyChangedEventArgs("Item[]"));
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }
    }

    public static class ObservableCollectionExtensions
    {
        public static void ReplaceAll<T>(this ObservableCollection<T> collection, IEnumerable<T> items)
        {
            if (collection is SafeObservableCollection<T> safe)
            {
                safe.ReplaceAll(items);
                return;
            }

            if (collection == null) throw new ArgumentNullException(nameof(collection));
            var list = items?.ToList() ?? new List<T>();
            collection.Clear();
            foreach (var item in list)
                collection.Add(item);
        }
    }
}
