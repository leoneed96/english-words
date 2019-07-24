using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnglishWords.Data.Entities
{
    public interface IEntity
    {
        int ID { get; set; }
        byte[] RowVersion { get; set; }
    }

    public class BaseEntity : IEntity
    {
        [Index(IsUnique = true)]
        public int ID { get; set; }
        [Timestamp]
        [Display(AutoGenerateField = false)]
        public byte[] RowVersion { get; set; }

    }
}
