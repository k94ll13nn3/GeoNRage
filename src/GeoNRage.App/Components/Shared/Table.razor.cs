using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Components;

namespace GeoNRage.App.Components.Shared
{
    public partial class Table<T>
    {
        private IEnumerable<T> _items = null!;
        private string _filter = string.Empty;
        private string _sortColumn = string.Empty;
        private bool _sortAscending = true;

        [Parameter]
        public IEnumerable<T> Items { get; set; } = null!;

        [Parameter]
        public RenderFragment<T> RowContent { get; set; } = null!;

        [Parameter]
        public RenderFragment? FooterContent { get; set; }

        [Parameter]
        public IEnumerable<TableHeader> Headers { get; set; } = null!;

        [Parameter]
        public Func<IEnumerable<T>, string, bool, IEnumerable<T>>? SortFunction { get; set; }

        [Parameter]
        public Func<IEnumerable<T>, string, IEnumerable<T>>? FilterFunction { get; set; }

        [Parameter]
        public bool Paginate { get; set; }

        [Parameter]
        public int PageSize { get; set; } = 10;

        [Parameter]
        public bool CanSearch { get; set; }

        public ICollection<T> DisplayedItems { get; } = new List<T>();

        public int PageCount { get; private set; }

        public int CurrentPage { get; private set; } = 1;

        protected override void OnInitialized()
        {
            RefreshItems();
        }

        private void ChangePage(int newPage)
        {
            if (newPage >= 1 && newPage <= PageCount)
            {
                CurrentPage = newPage;

                DisplayedItems.Clear();
                foreach (T item in _items.Take(CurrentPage * PageSize).Skip((CurrentPage - 1) * PageSize))
                {
                    DisplayedItems.Add(item);
                }

                StateHasChanged();
            }
        }
        private void Sort(string column, bool ascending)
        {
            _sortColumn = column;
            _sortAscending = ascending;
            RefreshItems();
        }

        private void OnFilter(ChangeEventArgs args)
        {
            _filter = args?.Value as string ?? string.Empty;
            RefreshItems();
        }

        private void RefreshItems()
        {
            IEnumerable<T> items = Items.ToList();
            if (SortFunction is not null && !string.IsNullOrWhiteSpace(_sortColumn))
            {
                items = SortFunction(items, _sortColumn, _sortAscending);
            }

            if (FilterFunction is not null && !string.IsNullOrWhiteSpace(_filter))
            {
                items = FilterFunction(items, _filter);
            }

            _items = items;

            if (Paginate)
            {
                PageCount = (int)Math.Ceiling(1.0 * _items.Count() / PageSize);
                ChangePage(Math.Clamp(CurrentPage, 1, PageCount));
            }
            else
            {
                DisplayedItems.Clear();
                foreach (T game in _items)
                {
                    DisplayedItems.Add(game);
                }
            }

            StateHasChanged();
        }
    }

    public record TableHeader(string Title, bool CanSort, string Property);
}
