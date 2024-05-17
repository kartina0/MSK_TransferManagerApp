// ----------------------------------------------
// Copyright © 2017 DATALINK
// ----------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace DL_CommonLibrary
{
 #if false  
    public class SortableBindingList<T> : BindingList<T>
    {

        private PropertyDescriptor _sortProp = null;
        private ListSortDirection _sortDir = ListSortDirection.Ascending;
        private bool _isSorted = false;

        public SortableBindingList() { }
        public SortableBindingList(IList<T> list) : base(list) { }
        public void AddRange(T[] array)
        {
            for (int i = 0; i < array.Length; i++)
                base.Add(array[i]);
        }

        protected override void ApplySortCore(PropertyDescriptor property, ListSortDirection direction)
        {
            List<T> list = this.Items as List<T>;
            if (list != null)
            {
                list.Sort(PropertyComparerFactory.Factory<T>(property, direction));

                this._isSorted = true;
                this._sortProp = property;
                this._sortDir = direction;

                this.OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
            }
        }

        protected override bool SupportsSortingCore { get { return true; } }
        protected override void RemoveSortCore() { }
        protected override bool IsSortedCore { get { return this._isSorted; } }
        protected override PropertyDescriptor SortPropertyCore { get { return this._sortProp; } }
        protected override ListSortDirection SortDirectionCore { get { return this._sortDir; } }
    }

    public static class PropertyComparerFactory
    {
        public static IComparer<T> Factory<T>(PropertyDescriptor property, ListSortDirection direction)
        {
            Type seed = typeof(PropertyComparer<,>);
            Type[] typeArgs = { typeof(T), property.PropertyType };
            Type pcType = seed.MakeGenericType(typeArgs);

            IComparer<T> comparer = (IComparer<T>)Activator.CreateInstance(pcType, new object[] { property, direction });
            return comparer;
        }
    }

    public sealed class PropertyComparer<T, U> : IComparer<T>
    {
        private PropertyDescriptor _property;
        private ListSortDirection _direction;
        private Comparer<U> _comparer;

        public PropertyComparer(PropertyDescriptor property, ListSortDirection direction)
        {
            this._property = property;
            this._direction = direction;
            this._comparer = Comparer<U>.Default;
        }

        public int Compare(T x, T y)
        {
            U xValue = (U)this._property.GetValue(x);
            U yValue = (U)this._property.GetValue(y);

            if (this._direction == ListSortDirection.Ascending)
                return this._comparer.Compare(xValue, yValue);
            else
                return this._comparer.Compare(yValue, xValue);
        }
    }
#else
    //IBindingListのソート機能を実装したジェネリックリストクラス
    public class SortableBindingList<T> : BindingList<T>
    {
        /// <summary>
        /// ソート項目
        /// </summary>
        private PropertyDescriptor _SortProperty;

        /// ソート方向(昇順・降順)の保持を行います。
        /// </summary>
        /// 
        private ListSortDirection _direction = ListSortDirection.Ascending;

        /// <summary>
        /// ソート済みかを示す値
        /// </summary>
        private bool _isSorted;

        /// <summary>
        /// 最優先比較オブジェクト
        /// </summary>
        private readonly Dictionary<string, IComparer<T>> _prioritySort;
        /// <summary>
        /// 第２キーの追加比較オブジェクト
        /// </summary>
        private readonly IComparer<T> _baseComparer;


        public SortableBindingList() { }
        public SortableBindingList(IList<T> list) : base(list) { }

        public static SortableBindingList<T> ToSortableBindingList(List<T> list)
        {
            return new SortableBindingList<T>(list);
        }

        public SortableBindingList(List<T> list) : base(list) { }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public SortableBindingList(IList<T> list, IComparer<T> baseComparer)
            : this(list, baseComparer, null)
        {
            _baseComparer = baseComparer;
        }
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public SortableBindingList(IList<T> list, IComparer<T> baseComparer, Dictionary<string, IComparer<T>> prioritySort)
            : base(list)
        {
            _baseComparer = baseComparer;
            _prioritySort = prioritySort;
        }

        public void ResetSort()
        {
            _SortProperty = null;
            _direction = ListSortDirection.Ascending;
            _isSorted = false;
        }

        protected override bool SupportsSortingCore
        {
            get
            {
                return true;
            }
        }

        public void AddRange(T[] array)
        {
            for (int i = 0; i < array.Length; i++)
                base.Add(array[i]);
        }

        protected override void ApplySortCore(PropertyDescriptor prop, ListSortDirection direction)
        {
            List<T> items = this.Items as List<T>;

            if (items != null)
            {
                PropertyComparer<T> pc = new PropertyComparer<T>(prop, direction);
                items.Sort(pc);
                _isSorted = true;
            }
            else
                _isSorted = false;

            _direction = direction;
            _SortProperty = prop;
            this.OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
        }

        protected override bool IsSortedCore
        {
            get { return _isSorted; }
        }

        protected override ListSortDirection SortDirectionCore
        {
            get
            {
                return _direction;
            }
        }

        protected override PropertyDescriptor SortPropertyCore
        {
            get
            {
                return _SortProperty;
            }
        }
    }

    //汎用コンペアラークラス
    public class PropertyComparer<T> : IComparer<T>
    {
        public PropertyComparer(PropertyDescriptor propertyName, ListSortDirection direction)
        {
            this.name = propertyName;
            sortDirection = (direction == ListSortDirection.Ascending) ? 1 : -1;
        }

        private PropertyDescriptor name;
        private int sortDirection;

        #region IComparer<T> メンバー

        public int Compare(T x, T y)
        {
            IComparable left = name.GetValue(x) as IComparable;
            IComparable right = name.GetValue(y) as IComparable;

            int result;

            if (left != null)
                result = left.CompareTo(right);
            else if (right == null)
                result = 0;
            else
                result = -1;

            return result * sortDirection;
        }
        #endregion
        //呼び出し用拡張メソッド
        //public static class BindingListExtensions
        //{
        //    public static SortableBindingList<T> ToSortableBindingList<T>(this List<T> list)
        //    {
        //        return SortableBindingList<T>.ToSortableBindingList(list);
        //    }
        //}

    }
#endif
    }
