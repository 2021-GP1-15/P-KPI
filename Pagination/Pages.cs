using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IP_KPI.Pagination
{
    public class Pages
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public Pages()
        {
            this.PageNumber = 1;
            this.PageSize = 10;
        }
        public Pages(int pageNumber, int pageSize)
        {
            this.PageNumber = Math.Abs(pageNumber) < 1 ? 1 : Math.Abs(pageNumber);
            this.PageSize = pageSize > 10 ? 10 : Math.Abs(pageSize);
        }
    }
}
