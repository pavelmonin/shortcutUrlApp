using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ShortcutUrlApp.Domain
{

    public class Url
    {
        public int Id { get; set; }

        [Display(Name = "Длинный URL")]
        [Url, Required]
        public string Original { get; set; }

        [Display(Name = "Сокращенный URL")]
        [Required]
        [RegularExpression(@"\b[a-z,A-Z,0-9]{6}\b$")]
        public string Shortened { get; set; }

        [Display(Name = "Кол-во переходов")]
        public int ConversionCount { get; set; }

        [Display(Name = "Дата создания")]
        public DateTime Created { get; set; }
    }
}
