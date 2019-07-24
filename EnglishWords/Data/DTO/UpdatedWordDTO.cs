using EnglishWords.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telerik.Windows.Controls.GridView;

namespace EnglishWords.Data.DTO
{
    public class UpdatedWordDTO
    {
        public UpdatedWordDTO()
        {

        }
        public UpdatedWordDTO(OperationType operationType, Word word)
        {
            OperationType = operationType;
            Word = word;
        }
        public OperationType OperationType { get; set; }
        public Word Word { get; set; }
    }

    public enum OperationType
    {
        Edit,
        Add,
        Remove
    }
}
