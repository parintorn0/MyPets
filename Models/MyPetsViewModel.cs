using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MyPets.Models
{
    public class MyPetsViewModel
    {
        [Key]
        [Column("Id")] //refer to real name of database table.
        [DisplayName("ID")] //the one displaying on web page
        public int Id { get; set; }
        [Column("PetName")] //refer to real name of database table.
        [DisplayName("Pet Name")] //the one displaying on web page
        public string? PetName { get; set; }
        [Column("Gender")] //refer to real name of database table.
        [DisplayName("Gender")] //the one displaying on web page
        public string? Gender { get; set; }
        [Column("DateOfBirth")] //refer to real name of database table.
        [DisplayName("Date Of Birth")] //the one displaying on web page
        public string? DateOfBirth {get; set; }
        [Column("PhotoFileName")] //refer to real name of database table.
        public string? PhotoFileName { get; set; }
    }
}