using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Shared.Extensions;

namespace Shared.DataStructures
{
    /// <summary>
    /// Change tracking and INotifyPropertyChanged implementation
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ObservableList<T> : List<T>, INotifyPropertyChanged, IChangeTracking
    {

        private readonly List<T> _originalList;
        private readonly List<bool> _changeTracker;

        public ObservableList(List<T> originalList)
        {
            _originalList = GenericCopier<List<T>>.DeepCopy(originalList);
            _changeTracker = new List<bool>(_originalList.Count);
        }

        public void SetItem(int index, T item)
        {
            base[index] = item;

            if (!this[index].Equals(_originalList[index]))
            {
                OnPropertyChanged();
                _changeTracker[index] = true;
            }
            else
            {
                _changeTracker[index] = false;
            }
        }

        public new T this[int index]
        {
            get
            {
                return base[index];
            }
            set
            {
                throw new InvalidOperationException("Cannot use indexer - use SetItem");
            } 
        }

        // TODO:

        //public new void Add(T item)
        //{
            
        //}

        //public new void AddRange(IEnumerable<T> collection)
        //{

        //}

        //public new void Clear()
        //{
            
        //}

        //public new void Insert(int index, T item)
        //{
           
        //}

        ///// <summary>
        ///// Inserts the elements of a collection into the <see cref="T:System.Collections.Generic.List`1"/> at the specified index.
        ///// </summary>
        ///// <param name="index">The zero-based index at which the new elements should be inserted.</param><param name="collection">The collection whose elements should be inserted into the <see cref="T:System.Collections.Generic.List`1"/>. The collection itself cannot be null, but it can contain elements that are null, if type <paramref name="T"/> is a reference type.</param><exception cref="T:System.ArgumentNullException"><paramref name="collection"/> is null.</exception><exception cref="T:System.ArgumentOutOfRangeException"><paramref name="index"/> is less than 0.-or-<paramref name="index"/> is greater than <see cref="P:System.Collections.Generic.List`1.Count"/>.</exception>
        //public void InsertRange(int index, IEnumerable<T> collection);

        //public bool Remove(T item);
      
        ///// <summary>
        ///// Removes all the elements that match the conditions defined by the specified predicate.
        ///// </summary>
        ///// 
        ///// <returns>
        ///// The number of elements removed from the <see cref="T:System.Collections.Generic.List`1"/> .
        ///// </returns>
        ///// <param name="match">The <see cref="T:System.Predicate`1"/> delegate that defines the conditions of the elements to remove.</param><exception cref="T:System.ArgumentNullException"><paramref name="match"/> is null.</exception>
        //public int RemoveAll(Predicate<T> match);
        ///// <summary>
        ///// Removes the element at the specified index of the <see cref="T:System.Collections.Generic.List`1"/>.
        ///// </summary>
        ///// <param name="index">The zero-based index of the element to remove.</param><exception cref="T:System.ArgumentOutOfRangeException"><paramref name="index"/> is less than 0.-or-<paramref name="index"/> is equal to or greater than <see cref="P:System.Collections.Generic.List`1.Count"/>.</exception>
        //public void RemoveAt(int index);
        ///// <summary>
        ///// Removes a range of elements from the <see cref="T:System.Collections.Generic.List`1"/>.
        ///// </summary>
        ///// <param name="index">The zero-based starting index of the range of elements to remove.</param><param name="count">The number of elements to remove.</param><exception cref="T:System.ArgumentOutOfRangeException"><paramref name="index"/> is less than 0.-or-<paramref name="count"/> is less than 0.</exception><exception cref="T:System.ArgumentException"><paramref name="index"/> and <paramref name="count"/> do not denote a valid range of elements in the <see cref="T:System.Collections.Generic.List`1"/>.</exception>
        //public void RemoveRange(int index, int count);


        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        #region IChangeTracking
        
        public void AcceptChanges()
        {
            throw new InvalidOperationException("AcceptChanges is not applicable to ObservableList<T>");
        }

        public bool IsChanged { 
            get
            {
                return _changeTracker.Any(x => x == true);
            }
            private set
            {
                throw new InvalidOperationException("Cannot set the IsChanged property - _changeTracker object is used to maintain change status");
            }   
        }

        #endregion
    }
}
