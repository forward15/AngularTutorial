using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations

namespace AngularTutorial.Models
{
    public class Todo
    {
        public int Id { get; set; }
        [MaxLength(800)]
        public string Text { get; set; }
        public DateTime? DueDate { get; set; }
        public int Priority { get; set; }
    }
}