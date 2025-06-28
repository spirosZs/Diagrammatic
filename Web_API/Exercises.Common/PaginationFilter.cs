using System.ComponentModel.DataAnnotations;

namespace Exercises.Common
{
    public abstract class PaginationFilter
    {
        private int _pageSize = 10;
        
        /// <summary>
        /// Specifies the number of items returned in each page. Cannot be greater than 20 items per page, 
        /// </summary>
        [Range(0, 20, ErrorMessage = "Page size cannot be greater than 20.")]
        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = value;
        }

        /// <summary>
        /// Specifies the current page offset.
        /// </summary>
        [Range(0, int.MaxValue, ErrorMessage = "Page number is not valid.")]
        public int PageNumber { get; set; } = 1;
    }
}