using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Components;

namespace GeoNRage.App.Components.Shared
{
    public partial class Table<T>
    {
        private IEnumerable<T> _items = null!;

        [Parameter]
        public IEnumerable<T> Items { get; set; } = null!;

        [Parameter]
        public RenderFragment<T> RowContent { get; set; } = null!;

        [Parameter]
        public RenderFragment? FooterContent { get; set; }

        [Parameter]
        public IEnumerable<TableHeader> Headers { get; set; } = null!;

        [Parameter]
        public Func<IEnumerable<T>, string, bool, IEnumerable<T>> SortFunction { get; set; } = null!;

        [Parameter]
        public bool Paginate { get; set; }

        [Parameter]
        public int PageSize { get; set; } = 10;

        public ICollection<T> DisplayedItems { get; } = new List<T>();

        public int PageCount { get; private set; }

        public int CurrentPage { get; private set; } = 1;

        protected override void OnInitialized()
        {
            _items = Items.ToList();
            if (Paginate)
            {
                PageCount = (int)Math.Ceiling(1.0 * Items.Count() / PageSize);
                DisplayedItems.Clear();
                foreach (T game in _items.Take(PageSize))
                {
                    DisplayedItems.Add(game);
                }
            }
            else
            {
                DisplayedItems.Clear();
                foreach (T game in _items)
                {
                    DisplayedItems.Add(game);
                }
            }
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
            if (SortFunction is not null)
            {
                _items = SortFunction(_items, column, ascending);
                if (Paginate)
                {
                    ChangePage(CurrentPage);
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
    }

    public record TableHeader(string Title, bool CanSort, string Property);
}
