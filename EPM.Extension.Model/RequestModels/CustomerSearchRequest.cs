using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EPM.Extension.Model.Common;

namespace EPM.Extension.Model.RequestModels
{
    public class CustomerSearchRequest:GetPagedListRequest
    {
        public string Param { get; set; }
        public Guid CustomerId { get; set; }
        /// <summary>
        /// Product Order By
        /// </summary>
        public CustomerColumnBy OrderBy
        {
            get
            {
                return (CustomerColumnBy)SortBy;
            }
            set
            {
                SortBy = (short)value;
            }
        }
    }
}
