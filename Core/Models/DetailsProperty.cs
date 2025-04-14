using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    public class DetailsProperty
    {
        [Key]
        public string Id { get; set; }
        [ForeignKey("PropertyId")]
        public Property Property { get; set; }
        public string PropertyId { get; set; }



    }
}
