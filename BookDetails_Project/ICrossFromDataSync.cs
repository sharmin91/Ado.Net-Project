using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookDetails_Project
{
    public interface ICrossFromDataSync
    {
        void ReloadData(List<Book> books);
        void UpdateBook(Book book);
        void RemoveBook(int id);
    }
}
